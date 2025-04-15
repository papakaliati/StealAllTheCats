using Moq;
using Xunit;
using StealAllTheCats.Api.Features.Cats.ListCats;
using StealAllTheCats.Api.Features.Cats.Shared;
using StealAllTheCats.Infrastructure.Data.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StealAllTheCats.tests.Api.Features.Cats;

namespace StealAllTheCats.tests.Api.Features.Cats.ListCats;

public class ListCatsServiceTests
{
    private readonly MockRepository mockRepository;
    private readonly Mock<ICatRepository> mockCatRepository;
    private readonly ListCatsService service;

    public ListCatsServiceTests()
    {
        this.mockRepository = new MockRepository(MockBehavior.Strict);
        this.mockCatRepository = this.mockRepository.Create<ICatRepository>();
        
        mockCatRepository.SeedCatRepository();
        service = new ListCatsService(mockCatRepository.Object);
    }

    [Fact]
    public async Task GetCatsAsync_ReturnsPaginatedList_WhenCatsExist()
    {
        // Arrange
        var tag = "Playful";
        var page = 1;
        var pageSize = 2;

        // Act
        var result = await service.GetCatsAsync(tag, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount); // Total count of cats matching the tag
        Assert.All(result.Items, cat => Assert.Contains(tag, cat.Tags));
    }

    [Fact]
    public async Task GetCatsAsync_ReturnsEmptyPaginatedList_WhenNoCatsExist()
    {
        // Arrange
        var tag = "NonExistentTag";
        var page = 1;
        var pageSize = 2;

        // Act
        var result = await service.GetCatsAsync(tag, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetCatsAsync_ReturnsPaginatedList_WhenNoTagIsProvided()
    {
        // Arrange
        string? tag = null;
        var page = 1;
        var pageSize = 2;

        // Act
        var result = await service.GetCatsAsync(tag, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count); // Page size
        Assert.Equal(4, result.TotalCount); // Total count of all cats
    }

    [Fact]
    public async Task GetCatsAsync_HandlesPaginationCorrectly()
    {
        // Arrange
        var tag = "Playful";
        var page = 2;
        var pageSize = 2;

        // Act
        var result = await service.GetCatsAsync(tag, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items); // Only one cat on the second page
        Assert.Equal(3, result.TotalCount); // Total count of all cats
    }
}
