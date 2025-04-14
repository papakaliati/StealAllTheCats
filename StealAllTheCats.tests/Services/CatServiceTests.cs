using global::StealAllTheCats.Infrastructure.Repositories;
using global::StealAllTheCats.Integrations.TheCatApi;
using global::StealAllTheCats.Services;
using Hangfire;
using Moq;
using StealAllTheCats.External.TheCatApi.Models;

namespace StealAllTheCats.tests.Services
{
    public class CatServiceTests
    {
        private MockRepository mockRepository;

        private Mock<ITagRepository> mockTagRepository;
        private Mock<ICatApiClient> mockCatApiClient;
        private Mock<ICatRepository> mockCatRepository;
        private Mock<IFileDownloader> mockFileDownloader;
        private Mock<IBackgroundJobClient> mockBackgroundJobClient;

        public CatServiceTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockTagRepository = this.mockRepository.Create<ITagRepository>();
            this.mockCatApiClient = this.mockRepository.Create<ICatApiClient>();
            mockCatApiClient
                .Setup(client => client.GetCatsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync([CreateMockedBaseCatApiResponse()]);
            mockCatApiClient
                .Setup(client => client.GetImageInfoByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateMockedCatApiResponse());

            this.mockCatRepository = this.mockRepository.Create<ICatRepository>();

            this.mockCatRepository
                .Setup(repo => repo.GetCatAsync("123"))
                .ReturnsAsync(CreateMockedCatEntity());
            this.mockCatRepository
                .Setup(repo => repo.GetCatAsync("12"))
                .ReturnsAsync((CatEntity)null);
            this.mockCatRepository
                .Setup(repo => repo.GetCatsAsync())
                .Returns(CreateMockCatEntities());

            this.mockFileDownloader = this.mockRepository.Create<IFileDownloader>();
            this.mockBackgroundJobClient = this.mockRepository.Create<IBackgroundJobClient>();
        }

        private IQueryable<CatEntity> CreateMockCatEntities() 
            => new List<CatEntity>
                {
                    new CatEntity
                    {
                        Id = 1,
                        CatId = "123",
                        Width = 500,
                        Height = 400,
                        Image = [1, 2, 3],
                        Created = System.DateTime.UtcNow,
                        CatTags =
                        [
                            new CatTag { Tag = new TagEntity { Name = "Cute" } }
                        ]
                    }
                }.AsQueryable().BuildMoq();

        private static BaseCatApiResponse CreateMockedBaseCatApiResponse() 
            => new BaseCatApiResponse
                {
                    Id = "123",
                    Url = "123",
                    Width = 500,
                    Height = 400,
                };

        private static CatApiResponse CreateMockedCatApiResponse()
         => new CatApiResponse
             {
                 Id = "123",
                 Url = "123",
                 Width = 500,
                 Height = 400,
                 Breeds = [
                     new Breed
                     {
                         Temperament = "Friendly"
                     }
                 ]
             };

        private static CatEntity CreateMockedCatEntity()
        {
            return new CatEntity
            {
                Id = 1,
                CatId = "123",
                Width = 500,
                Height = 400,
                Image = [1, 2, 3],
                Created = System.DateTime.UtcNow,
                CatTags =
                    [
                        new CatTag { Tag = new TagEntity { Name = "Cute" } }
                    ]
            };
        }


        private CatService CreateService()
        {
            return new CatService(
                this.mockCatApiClient.Object,
                this.mockCatRepository.Object,
                this.mockTagRepository.Object,
                this.mockFileDownloader.Object,
                this.mockBackgroundJobClient.Object);
        }

        [Fact]
        public async Task GetCatByIdAsync_ReturnsCatDto_WhenCatExists()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetCatByIdAsync("123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("123", result.CatId);
            Assert.Contains("Cute", result.Tags);
            //this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetCatByIdAsync_ReturnsNull_WhenCatDoesNotExist()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetCatByIdAsync("12");

            // Assert
            Assert.Null(result);
        }

        //[Fact]
        //public async Task GetCatsAsync_ReturnsPaginatedList_WhenCatsExist()
        //{
        //    // Arrange
        //    var service = this.CreateService();
           
        //    // Act
        //    var result = await service.GetCatsAsync("Cute", 1, 10);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Single(result.Items);
        //    Assert.Equal("123", result.Items.First().CatId);
        //}
    }
}