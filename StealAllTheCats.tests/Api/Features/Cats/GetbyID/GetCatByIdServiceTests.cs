using Moq;
using Xunit;
using StealAllTheCats.Api.Features.Cats.GetbyID;
using StealAllTheCats.Infrastructure.Data.Repositories;
using StealAllTheCats.Api.Features.Cats.Shared;
using System.Threading.Tasks;
using System.Collections.Generic;
using StealAllTheCats.tests.Api.Features.Cats;

namespace StealAllTheCats.tests.Api.Features.Cats.GetbyID;

public class GetCatByIdServiceTests
{
    private readonly MockRepository mockRepository;
    private readonly Mock<ICatRepository> mockCatRepository;
    private readonly GetCatByIdService service;

    public GetCatByIdServiceTests()
    {
        this.mockRepository = new MockRepository(MockBehavior.Strict);

        this.mockCatRepository = mockRepository.Create<ICatRepository>();
        mockCatRepository.SeedCatRepository();

        service = new GetCatByIdService(mockCatRepository.Object);
    }

    [Fact]
    public async Task GetCatByIdAsync_ReturnsCatDto_WhenCatExists()
    {
        // Arrange
        string catId = "1";

        CatDto mockCatEntity = new CatDto (
            Id: 1,
            CatId: "cat123",
            Width: 500,
            Height: 400,
            Image: "image1",
            Created: DateTime.UtcNow.AddDays(-1),
            Tags: ["Playful"]
        );

        // Act
        CatDto? result = await service.GetCatByIdAsync(catId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockCatEntity.Id, result.Id);
        Assert.Equal(mockCatEntity.CatId, result.CatId);
        Assert.Equal(mockCatEntity.Width, result.Width);
        Assert.Equal(mockCatEntity.Height, result.Height);
        Assert.Contains("Playful", result.Tags);
    }

    [Fact]
    public async Task GetCatByIdAsync_ReturnsNull_WhenCatDoesNotExist()
    {
        // Arrange
        string catId = "11233";

        // Act
        var result = await service.GetCatByIdAsync(catId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCatByIdAsync_ReturnsCatDto_WithMultipleTags()
    {
        // Arrange
        var catId = "3";

        CatDto mockCatEntity = new CatDto(
            Id: 3,
            CatId: "cat789",
            Width: 300,
            Height: 300,
            Image: "image3",
            Created: DateTime.UtcNow.AddDays(-3),
            Tags: [ "Friendly", "Adventurous"]
        );

        // Act
        var result = await service.GetCatByIdAsync(catId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockCatEntity.Id, result.Id);
        Assert.Equal(mockCatEntity.CatId, result.CatId);
        Assert.Equal(mockCatEntity.Width, result.Width);
        Assert.Equal(mockCatEntity.Height, result.Height);
        Assert.Contains("Friendly", result.Tags);
        Assert.Contains("Adventurous", result.Tags);
    }
}
