using CigarBazaar.Application.CigarPrices;
using CigarBazaar.Application.Cigars;
using CigarBazaar.Shared.Models;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace CigarBazaar.Application.UnitTest.Cigars;

public class CigarServiceTests
{
    [Fact]
    public void CigarService_Constructor_Should_Not_Fail_If_Params_Are_Valid()
    {
        // Arrange
        CigarService? service = null;

        Mock<IMemoryCache> mockMemoryCache = new Mock<IMemoryCache>(behavior: MockBehavior.Loose);
        Mock<ICigarPricesService> mockCigarPricesService = new Mock<ICigarPricesService>(behavior: MockBehavior.Loose);

        // Act
        var act = () =>
        {
            service = new CigarService(memoryCache: mockMemoryCache.Object,
                 cigarPricesService: mockCigarPricesService.Object);
        };

        // Assert
        act.Should().NotThrow<Exception>();
        service.Should().NotBeNull();
    }

    [Fact]
    public void CigarService_Constructor_Should_Throw_Exception_If_MemorCache_Is_Null()
    {
        // Arrange
        CigarService? service = null;

        Mock<ICigarPricesService> mockCigarPricesService = new Mock<ICigarPricesService>(behavior: MockBehavior.Loose);
        

        // Act
        var act = () =>
        {
            service = new CigarService(memoryCache: null,
                 cigarPricesService: mockCigarPricesService.Object);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>();
        service.Should().BeNull();
    }

    [Fact]
    public void CigarService_Constructor_Should_Throw_Exception_If_ICigarPricesService_Is_Null()
    {
        // Arrange
        CigarService? service = null;

        Mock<IMemoryCache> mockMemoryCache = new Mock<IMemoryCache>(behavior: MockBehavior.Loose);

        // Act
        var act = () =>
        {
            service = new CigarService(memoryCache: mockMemoryCache.Object,
                 cigarPricesService: null);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>();
        service.Should().BeNull();
    }

    [Fact(Skip = "Unable to mock IMemoryCache")]
    public void GetCigarsAsync_Should_Return_List_If_No_Cache()
    {
        // Arrange
        CigarService? service = null;
        IList<Cigar>? response = null;

        DateTime? dateTime = null;
        Mock<IMemoryCache> mockMemoryCache = new Mock<IMemoryCache>();
        mockMemoryCache
            .Setup(x => x.TryGetValue("PricesUpdateDate", out dateTime))
            .Returns(true);
        mockMemoryCache
            .Setup(x => x.Set<IList<Cigar>>("CigarsList", It.IsAny<List<Cigar>>(), It.IsAny<MemoryCacheEntryOptions>()))
            .Returns(new List<Cigar>());

        Mock<ICigarPricesService> mockCigarPricesService = new Mock<ICigarPricesService>();
        mockCigarPricesService
            .Setup(x => x.GetPriceListAsync())
            .ReturnsAsync(new CigarPriceList()
            {
                Cigars = new List<Cigar>(),
                PricesUpdateDate = DateTime.Now,
            });

        // Act
        var act = async () =>
        {
            service = new CigarService(memoryCache: mockMemoryCache.Object,
                 cigarPricesService: mockCigarPricesService.Object);

            response = await service.GetCigarsAsync();
        };

        // Assert
        act.Should().NotThrowAsync<Exception>();
        service.Should().NotBeNull();
        response.Should().NotBeNullOrEmpty();
    }


    [Fact(Skip = "Unable to mock IMemoryCache")]
    public void GetCigarsAsync_Should_Return_List_If_Cache_Is_Valid()
    {
        // Arrange
        CigarService? service = null;
        IList<Cigar>? response = null;

        DateTime dateTime = DateTime.Now;
        List<Cigar>? cigarList = [];
        Mock<IMemoryCache> mockMemoryCache = new Mock<IMemoryCache>();
        mockMemoryCache
            .Setup(x => x.TryGetValue("PricesUpdateDate", out dateTime))
            .Returns(true);
        mockMemoryCache
            .Setup(x => x.TryGetValue("CigarsList", out cigarList))
            .Returns(true);

        Mock<ICigarPricesService> mockCigarPricesService = new Mock<ICigarPricesService>();
        mockCigarPricesService
            .Setup(x => x.GetPricesUpdateDate())
            .ReturnsAsync(DateTime.Now.AddDays(-1));

        // Act
        var act = async () =>
        {
            service = new CigarService(memoryCache: mockMemoryCache.Object,
                 cigarPricesService: mockCigarPricesService.Object);

            response = await service.GetCigarsAsync();
        };

        // Assert
        act.Should().NotThrowAsync<Exception>();
        service.Should().NotBeNull();
        response.Should().NotBeNullOrEmpty();
    }

    [Fact(Skip = "Unable to mock IMemoryCache")]
    public void GetCigarsAsync_Should_Return_List_If_Cache_Is_Old()
    {
        // Arrange
        CigarService? service = null;
        IList<Cigar>? response = null;

        DateTime dateTime = DateTime.Now.AddDays(-1);
        Mock<IMemoryCache> mockMemoryCache = new Mock<IMemoryCache>();
        mockMemoryCache
            .Setup(x => x.TryGetValue("PricesUpdateDate", out dateTime))
            .Returns(true);
        mockMemoryCache
            .Setup(x => x.Set<IList<Cigar>>("CigarsList", It.IsAny<List<Cigar>>(), It.IsAny<MemoryCacheEntryOptions>()))
            .Returns(new List<Cigar>());

        Mock<ICigarPricesService> mockCigarPricesService = new Mock<ICigarPricesService>();
        mockCigarPricesService
            .Setup(x => x.GetPricesUpdateDate())
            .ReturnsAsync(DateTime.Now);
        mockCigarPricesService
            .Setup(x => x.GetPriceListAsync())
            .ReturnsAsync(new CigarPriceList()
            {
                Cigars = new List<Cigar>(),
                PricesUpdateDate = DateTime.Now,
            });

        // Act
        var act = async () =>
        {
            service = new CigarService(memoryCache: mockMemoryCache.Object,
                 cigarPricesService: mockCigarPricesService.Object);

            response = await service.GetCigarsAsync();
        };

        // Assert
        act.Should().NotThrowAsync<Exception>();
        service.Should().NotBeNull();
        response.Should().NotBeNullOrEmpty();
    }

}
