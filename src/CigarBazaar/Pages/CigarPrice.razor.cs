using Radzen;
using Radzen.Blazor;
using Models = CigarBazaar.Shared.Models;

namespace CigarBazaar.Pages;

public partial class CigarPrice
{
    private readonly string gridName = "cigar-grid";
    private RadzenDataGrid<Models.CigarPrice> grid = new RadzenDataGrid<Models.CigarPrice>();

    
    private bool isLoading;
    private IList<Models.CigarPrice> prices = new List<Models.CigarPrice>();
    private IList<Models.CigarPrice> selectedPrices = new List<Models.CigarPrice>();
    private int count => prices.Count();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        //TODO: Set INPUT.
        prices = new List<Models.CigarPrice>();
    }

    async Task LoadData(LoadDataArgs args)
    {
        isLoading = true;

        //TODO: Set INPUT.
        prices = new List<Models.CigarPrice>();

        isLoading = false;
    }

}
