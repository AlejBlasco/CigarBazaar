using CigarBazaar.Shared.Models;
using HtmlAgilityPack;
using PuppeteerSharp;
using System.Globalization;

namespace CigarBazaar.Application.Hacienda;

public class PriceOfLaborService : IPriceOfLaborService
{
    private readonly string urlToScrap;
    private readonly string format = "dd-MM-yyyy";
    private readonly string dateSelector = "div.txtResolucion p";
    private readonly string seeAllSelector = "input[type=\"submit\"][name=\"verTodo\"]";
    private readonly string tableSelector = "table tbody";
    private readonly LaunchOptions options = new LaunchOptions
    {
        Headless = true
    };

    public PriceOfLaborService(string urlToScrap)
    {
        this.urlToScrap = urlToScrap;
        if (string.IsNullOrEmpty(urlToScrap))
            throw new ArgumentNullException(nameof(urlToScrap));
    }

    public async Task<DateTime?> GetLastUpdateDateAsync()
    {
        DateTime? response = null;

        await new BrowserFetcher().
            DownloadAsync();

        IBrowser? browser = await Puppeteer.LaunchAsync(options);

        IPage? page = null;
        if (browser != null)
            page = await browser!.NewPageAsync();

        if (page != null)
        {
            await page.GoToAsync(urlToScrap);
            await page.WaitForSelectorAsync(dateSelector);

            var textDate = await page.EvaluateExpressionAsync<string>($"document.querySelector('{dateSelector}').outerHTML");

            if (!string.IsNullOrWhiteSpace(textDate))
            {
                textDate = textDate.Replace("<p>", "")
                    .Replace("</p>", "")
                    .Trim();

                response = DateTime.ParseExact(textDate.Substring(textDate.Length - 10), format, CultureInfo.InvariantCulture);
            }
        }

        return response;
    }

    public async Task<IList<CigarPrice>> GetPriceOfLaborListAsync()
    {
        List<CigarPrice> response = new List<CigarPrice>();

        await new BrowserFetcher().
            DownloadAsync();

        IBrowser? browser = await Puppeteer.LaunchAsync(options);

        IPage? page = null;
        if (browser != null)
            page = await browser!.NewPageAsync();

        if (page != null)
        {
            await page.GoToAsync(urlToScrap);

            await page.SelectAsync("select[name=\"MinPortalTabacosLabor\"]", "Cigarros");
            await page.ClickAsync("input[type=\"submit\"][name=\"filtrar\"]");
            await page.WaitForSelectorAsync(tableSelector);

            await page.ClickAsync(seeAllSelector);
            await page.WaitForSelectorAsync(tableSelector);

            var textTable = await page.EvaluateExpressionAsync<string>($"document.querySelector('{tableSelector}').outerHTML");
            if (!string.IsNullOrWhiteSpace(textTable))
                response = ParseTableItems(textTable);
        }

        return response;
    }

    private static List<CigarPrice> ParseTableItems(string tbodyHtml)
    {
        var tableItems = new List<CigarPrice>();

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(tbodyHtml);

        var rows = htmlDocument.DocumentNode.SelectNodes("//tr");
        if (rows != null)
        {
            foreach (var row in rows)
            {
                var columns = row.SelectNodes("td");
                if (columns != null && columns.Count >= 3)
                {
                    try
                    {
                        var tableItem = new CigarPrice
                        {
                            Name = columns[0].InnerText.Trim(),
                            Price = Decimal.Parse(columns[1].InnerText.Trim().Replace('.', ',')),
                            SurchargedPrice = Decimal.Parse(columns[2].InnerText.Trim().Replace('.', ','))
                        };
                        tableItems.Add(tableItem);
                    }
                    catch { /* Sometimes, something go wrong with data. */}
                }
            }
        }

        return tableItems;
    }
}
