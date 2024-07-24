using CigarBazaar.Application.CigarPrices;
using CigarBazaar.Shared.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace CigarBazaar.Application.UnitTest.CigarPrices;

public class CigarPricesServiceTests
{
    private readonly IConfiguration config;

    public CigarPricesServiceTests()
    {
        config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
            .Build();
    }

    [Fact]
    public void CigarPricesService_Constructor_Should_Not_Fail_If_Configuration_Is_Valid()
    {
        // Arrange
        CigarPricesService? service = null;
        CigarPricesConfiguration configuration = new CigarPricesConfiguration
        {
            UrlToScrap = config["CigarPricesConfiguration:UrlToScrap"]
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

    [Fact, Trait("GithubAction", "GithubAction_Disabled")]
    public async Task GetPricesUpdateDate_Should_Return_Valid_Date()
    {
        // Arrange
        DateTime? date = null;  
        CigarPricesService? service = null;
        CigarPricesConfiguration configuration = new CigarPricesConfiguration
        {
            UrlToScrap = config["CigarPricesConfiguration:UrlToScrap"]
        };

        // Act
        var act = async () =>
        {
            service = new CigarPricesService(configuration);
            date = await service.GetPricesUpdateDate();
        };

        // Assert
        await act.Should().NotThrowAsync<Exception>();
        
        service.Should().NotBeNull();

        date.Should().NotBeNull();
    }

    [Fact, Trait("GithubAction", "GithubAction_Disabled")]
    public async Task GetPriceListAsync_Should_Return_Valid_CigarPriceList()
    {
        // Arrange
        CigarPriceList? cigarPriceList = null;
        CigarPricesService? service = null;
        CigarPricesConfiguration configuration = new CigarPricesConfiguration
        {
            UrlToScrap = config["CigarPricesConfiguration:UrlToScrap"]
        };

        // Act
        var act = async () =>
        {
            service = new CigarPricesService(configuration);
            cigarPriceList = await service.GetPriceListAsync();
        };

        // Assert
        await act.Should().NotThrowAsync<Exception>();

        service.Should().NotBeNull();

        cigarPriceList.Should().NotBeNull();
        cigarPriceList?.PricesUpdateDate.Should().NotBeAfter(DateTime.UtcNow);
        cigarPriceList?.Cigars.Should().NotBeNull();
    }

}
