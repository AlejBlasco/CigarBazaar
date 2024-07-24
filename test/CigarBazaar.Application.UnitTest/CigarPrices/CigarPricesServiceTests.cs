using CigarBazaar.Application.CigarPrices;
using CigarBazaar.Shared.Models;
using FluentAssertions;

namespace CigarBazaar.Application.UnitTest.CigarPrices;

public class CigarPricesServiceTests
{
    [Fact]
    public void CigarPricesService_Constructor_Should_Not_Fail_If_Configuration_Is_Valid()
    {
        // Arrange
        CigarPricesService? service = null;
        CigarPricesConfiguration configuration = new CigarPricesConfiguration
        {
            UrlToScrap = "https://dummy.es"
        };

        // Act
        var act = () =>
        {
            service = new CigarPricesService(configuration);
        };

        // Assert
        act.Should().NotThrow<Exception>();
        service.Should().NotBeNull();
    }

    [Fact]
    public void CigarPricesService_Constructor_Should_Fail_If_Url_Is_Empty()
    {
        // Arrange
        CigarPricesService? service = null;
        CigarPricesConfiguration configuration = new CigarPricesConfiguration
        {
            UrlToScrap = string.Empty
        };

        // Act
        var act = () =>
        {
            service = new CigarPricesService(configuration);
        };

        // Assert
        act.Should().Throw<ArgumentException>();
        service.Should().BeNull();
    }

    [Fact(Skip = "Not setted yet")]
    public async Task GetPricesUpdateDate_Should_Return_Valid_Date()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact(Skip = "Not setted yet")]
    public async Task GetPriceListAsync_Should_Return_Valid_CigarPriceList()
    {
        // Arrange

        // Act

        // Assert
    }

}
