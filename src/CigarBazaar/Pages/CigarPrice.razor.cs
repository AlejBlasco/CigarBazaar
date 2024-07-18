using CigarBazaar.Application.CigarPrices;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System.Net.Http.Json;
using static CigarBazaar.Pages.Weather;
using static System.Net.WebRequestMethods;
using Models = CigarBazaar.Shared.Models;

namespace CigarBazaar.Pages;

public partial class CigarPrice
{
    [Inject]
    HttpClient Http { get; set; }   

    private readonly string gridName = "cigar-grid";
    private RadzenDataGrid<Models.CigarPrice> grid = new RadzenDataGrid<Models.CigarPrice>();

    private bool isLoading;
    private int pageSize = 10;
    private IEnumerable<Models.CigarPrice> prices = new List<Models.CigarPrice>();

    private IList<Models.CigarPrice> selectedPrices = new List<Models.CigarPrice>();
    private int count => prices.Count();

    ICigarPricesService service = new CigarPricesService();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await ShowLoading();

        prices = (await Http.GetFromJsonAsync<List<Models.CigarPrice>>("sample-data/prices.json"))
            .AsQueryable()
            .Take(pageSize);
    }

    async Task ShowLoading()
    {
        isLoading = true;

        await Task.Yield();
        
        isLoading = false;
    }

    async Task LoadData(LoadDataArgs args)
    {
        isLoading = true;

        prices = await Http.GetFromJsonAsync<List<Models.CigarPrice>>("sample-data/prices.json");
        prices = prices.AsQueryable().Skip(args.Skip.Value).Take(args.Top.Value).ToList();

        isLoading = false;
    }

}
