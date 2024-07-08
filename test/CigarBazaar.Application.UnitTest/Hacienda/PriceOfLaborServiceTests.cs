using CigarBazaar.Application.Hacienda;
using CigarBazaar.Shared.Models;
using FluentAssertions;

namespace CigarBazaar.Application.UnitTest.Hacienda;

public class PriceOfLaborServiceTests
{
    private readonly string urlToScrap = "https://www.hacienda.gob.es/es-ES/Areas%20Tematicas/CMTabacos/Paginas/PreciosLabores.aspx";

    [Fact]
    public void PriceOfLaborService_Constructor_should_throw_exception_if_argument_is_null()
    {
        // Arrange
        var emptyUrlToScrap = string.Empty;

        // Act
        var act = () =>
        {
            IPriceOfLaborService service = new PriceOfLaborService(emptyUrlToScrap);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task GetLastUpdateDateAsync_shoud_return_valid_datetime()
    {
        // Arrange
        DateTime? response = null;
        IPriceOfLaborService service = new PriceOfLaborService(urlToScrap);

        // Act
        var act = async () => 
        { 
            response = await service.GetLastUpdateDateAsync();
        };

        // Assert
        await act.Should().NotThrowAsync<Exception>();
        response.Should().NotBeNull();
        response!.GetType().Should().Be(typeof(DateTime));  
    }

    [Fact]
    public async Task GetPriceOfLaborListAsync_should_return_valid_list()
    {
        // Arrange
        IList<CigarPrice> response = new List<CigarPrice>();
        IPriceOfLaborService service = new PriceOfLaborService(urlToScrap);

        // Act
        var act = async () =>
        {
            response = await service.GetPriceOfLaborListAsync();
        };

        // Assert
        await act.Should().NotThrowAsync<Exception>();
        response.Should().NotBeNullOrEmpty();
    }
}
