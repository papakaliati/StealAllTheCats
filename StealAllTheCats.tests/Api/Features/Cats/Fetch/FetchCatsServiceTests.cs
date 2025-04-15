using global::StealAllTheCats.Api.Features.Cats.Fetch;
using global::StealAllTheCats.Application.Interfaces;
using global::StealAllTheCats.Infrastructure.Data.Repositories;
using global::StealAllTheCats.Integrations.TheCatApi;
using Hangfire;
using Moq;

namespace StealAllTheCats.tests.Api.Features.Cats.Fetch;

public class FetchCatsServiceTests
{
    private MockRepository mockRepository;

    private Mock<ICatApiClient> mockCatApiClient;
    private Mock<ICatRepository> mockCatRepository;
    private Mock<ITagRepository> mockTagRepository;
    private Mock<IFileDownloader> mockFileDownloader;
    private Mock<IFileStorageService> mockFileStorageService;
    private Mock<IBackgroundJobClient> mockBackgroundJobClient;

    public FetchCatsServiceTests()
    {
        this.mockRepository = new MockRepository(MockBehavior.Loose);

        this.mockCatRepository = this.mockRepository.Create<ICatRepository>();
        mockCatRepository.SeedCatRepository();
        this.mockTagRepository = this.mockRepository.Create<ITagRepository>();
        mockTagRepository.SeedTagRepository();

        this.mockCatApiClient = this.mockRepository.Create<ICatApiClient>();
        mockCatApiClient.Setup(x => x.GetCatsAsync(It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync((int limit, int has_breeds = 1) =>
                            [.. RepoSeeds.CreateMockBaseCatApiResponses().Take(limit)] // Simulate API response
                        );
        this.mockFileDownloader = this.mockRepository.Create<IFileDownloader>();
        this.mockFileStorageService = this.mockRepository.Create<IFileStorageService>();
        this.mockBackgroundJobClient = this.mockRepository.Create<IBackgroundJobClient>();
    }

    private FetchCatsService CreateService()
    {
        return new FetchCatsService(
            this.mockCatApiClient.Object,
            this.mockCatRepository.Object,
            this.mockTagRepository.Object,
            this.mockFileDownloader.Object,
            this.mockFileStorageService.Object,
            this.mockBackgroundJobClient.Object);
    }

    [Fact]
    public async Task FetchAndStoreCatsAsync_StateUnderTest_ExpectedBehavior()
    {
        // Arrange
        var service = this.CreateService();

        // Act
        await service.FetchAndStoreCatsAsync();

        // Assert
        Assert.True(true);
    }
}
