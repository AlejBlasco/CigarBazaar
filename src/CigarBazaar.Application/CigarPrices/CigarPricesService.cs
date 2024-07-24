using CigarBazaar.Shared.Models;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using PuppeteerSharp.BrowserData;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CigarBazaar.Application.CigarPrices;

public class CigarPricesService : ICigarPricesService
{
    private readonly CigarPricesConfiguration cigarPricesConfiguration;
    private readonly LaunchOptions launchOptions;
    private readonly InstalledBrowser installedBrowser;

    public CigarPricesService(CigarPricesConfiguration cigarPricesConfiguration)
    {
        this.cigarPricesConfiguration = cigarPricesConfiguration;

        launchOptions = new LaunchOptions
        {
            Headless = false,
            IgnoredDefaultArgs = new[] { "--disable-extensions" },
            Args = new string[] { }
        };

        if (string.IsNullOrWhiteSpace(this.cigarPricesConfiguration.UrlToScrap))
            throw new ArgumentException(nameof(this.cigarPricesConfiguration));

        installedBrowser = installedBrowser
            ?? new BrowserFetcher().DownloadAsync().Result;
    }

    public async Task<DateTime?> GetPricesUpdateDate()
    {
        DateTime? pricesUpdateDate = null;

        using (var browser = await Puppeteer.LaunchAsync(launchOptions))
        using (var page = await browser.NewPageAsync())
        {
            await page.GoToAsync(cigarPricesConfiguration.UrlToScrap);

            // Wait for date and get it.
            pricesUpdateDate = await GetLastUpdateDate(page) ?? DateTime.Now;

            browser.Disconnect();
        }

        return pricesUpdateDate;
    }

    public async Task<CigarPriceList> GetPriceListAsync()
    {
        CigarPriceList cigarPriceList = new CigarPriceList();

        using (var browser = await Puppeteer.LaunchAsync(launchOptions))
        using (var page = await browser.NewPageAsync())
        {
            await page.GoToAsync(cigarPricesConfiguration.UrlToScrap);

            // Wait for date and get it.
            cigarPriceList.PricesUpdateDate = await GetLastUpdateDate(page) ?? DateTime.Now;

            // Filter cigar list and get it complete.
            cigarPriceList.Cigars = await GetAllCigarPricesAsync(page);

            browser.Disconnect();
        }

        return cigarPriceList;
    }

    private async Task<DateTime?> GetLastUpdateDate(IPage page)
    {
        DateTime? lastUpdate = null;

        var dateSelector = "div.txtResolucion p";
        await page.WaitForSelectorAsync(dateSelector);

        var textDate = await page.EvaluateExpressionAsync<string>($"document.querySelector('{dateSelector}').outerHTML");

        if (!string.IsNullOrEmpty(textDate))
        {
            textDate = Regex.Replace(textDate, @"<\/?p>", "", RegexOptions.IgnoreCase);
            lastUpdate = DateTime.ParseExact(textDate.Substring(textDate.Length - 10), "dd-MM-yyyy", CultureInfo.InvariantCulture);
        }

        return lastUpdate;
    }

    private async Task<List<Cigar>> GetAllCigarPricesAsync(IPage page)
    {
        List<Cigar> cigars = new List<Cigar>();

        // Only Peninsula prices
        await page.SelectAsync("#zonas", "Península e Illes Balears");

        // Only Cigars prices
        await page.SelectAsync("#labores", "Cigarros");

        // Submit and wait
        await page.ClickAsync("#filtrarSin");
        await page.WaitForNetworkIdleAsync();

        // Click on see all
        await page.WaitForSelectorAsync("#verTodo");
        await page.ClickAsync("#verTodo");
        await page.WaitForNetworkIdleAsync();

        // Get Data.
        var tableSelector = "table.tabla tbody";
        await page.WaitForSelectorAsync(tableSelector);
        var textTable = await page.EvaluateExpressionAsync<string>($"document.querySelector('{tableSelector}').outerHTML");
        if (!string.IsNullOrEmpty(textTable))
            cigars = ParseTableToCigar(textTable);


        return cigars;
    }

    private List<Cigar> ParseTableToCigar(string tbody)
    {
        List<Cigar> cigars = new List<Cigar>();

        string trPattern = @"<tr>(.*?)<\/tr>";
        string tdPattern = @"<td[^>]*>(.*?)<\/td>";
        MatchCollection trMatches = Regex.Matches(tbody, trPattern, RegexOptions.Singleline);

        foreach (Match trMatch in trMatches)
        {
            string trContent = trMatch.Groups[1].Value;

            MatchCollection tdMatches = Regex.Matches(trContent, tdPattern, RegexOptions.Singleline);
            if (tdMatches.Count == 3)
            {
                string name = Regex.Replace(tdMatches[0].Value, @"<[^>]*>", "")
                    .Replace("Marca", "").Trim();
                string textPrice = Regex.Match(tdMatches[1].Value, @"<td[^>]*>.*?<\/span>([^<]+)<\/td>", RegexOptions.Singleline).Groups[1].Value.Trim();
                string textChargedPrice = Regex.Match(tdMatches[2].Value, @"<td[^>]*>.*?<\/span>([^<]+)<\/td>", RegexOptions.Singleline).Groups[1].Value.Trim();

                decimal price;
                decimal chargedPrice;

                cigars.Add(new Cigar
                {
                    Name = name,
                    Price = decimal.TryParse(textPrice, NumberStyles.Number, CultureInfo.InvariantCulture, out price) ? price : decimal.Zero,
                    ChargedPrice = decimal.TryParse(textChargedPrice, NumberStyles.Number, CultureInfo.InvariantCulture, out chargedPrice) ? chargedPrice : decimal.Zero,
                });
            }
        }

        return cigars;
    }
}
