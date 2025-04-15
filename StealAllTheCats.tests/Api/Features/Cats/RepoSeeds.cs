using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using StealAllTheCats.Infrastructure.Data.Repositories;
using StealAllTheCats.Integrations.TheCatApi.Models;

namespace StealAllTheCats.tests.Api.Features.Cats;

public static class RepoSeeds
{
    public static List<BaseCatApiResponse> CreateMockBaseCatApiResponses() 
        => [
        new BaseCatApiResponse
        {
            Id = "cat123",
            Width = 500,
            Height = 400,
            Url = "https://example.com/cat123.jpg"
        },
        new BaseCatApiResponse
        {
            Id = "cat456",
            Width = 600,
            Height = 450,
            Url = "https://example.com/cat456.jpg"
        },
        new BaseCatApiResponse
        {
            Id = "cat789",
            Width = 300,
            Height = 300,
            Url = "https://example.com/cat789.jpg"
        },
        new BaseCatApiResponse
        {
            Id = "cat101",
            Width = 800,
            Height = 600,
            Url = "https://example.com/cat101.jpg"
        },
        new BaseCatApiResponse
        {
            Id = "cat202",
            Width = 400,
            Height = 400,
            Url = "https://example.com/cat202.jpg"
        }
    ];

    public static IEnumerable<TagEntity> CreateMockTagEntities()
       =>
        [
           new TagEntity { Id = 1, Name = "Playful"},
           new TagEntity { Id = 2, Name = "Friendly" },
           new TagEntity { Id = 3, Name = "Friendly" },
           new TagEntity { Id = 4, Name = "Adventurous"},
           new TagEntity { Id = 5, Name = "Lazy"},
       ];

    public static IEnumerable<CatEntity> CreateMockCatEntities()
        => [
            new CatEntity
            {
                Id = 1,
                CatId = "cat123",
                Width = 500,
                Height = 400,
                Image = "image1",
                Created = DateTime.UtcNow.AddDays(-1),
                CatTags =
                [
                    new CatTag {
                        CatEntityId = 1,
                        Cat = new CatEntity(),
                        TagEntityId = 1,
                        Tag = new TagEntity { Name = "Playful"},
                    }
                ]
            },
            new CatEntity
            {
                Id = 2,
                CatId = "cat456",
                Width = 600,
                Height = 450,
                Image = "image2",
                Created = DateTime.UtcNow.AddDays(-2),
                CatTags =
                [
                    new CatTag {
                        CatEntityId = 2,
                        Cat = new CatEntity(),
                        TagEntityId = 2,
                        Tag = new TagEntity { Name = "Friendly" }
                    }
                ]
            },
            new CatEntity
            {
                Id = 3,
                CatId = "cat789",
                Width = 300,
                Height = 300,
                Image = "image3",
                Created = DateTime.UtcNow.AddDays(-3),
                CatTags =
                [
                    new CatTag {
                        CatEntityId = 3,
                        Cat = new CatEntity(),
                        TagEntityId = 2,
                        Tag = new TagEntity { Name = "Friendly" }
                    },
                    new CatTag {
                        CatEntityId = 3,
                        Cat = new CatEntity(),
                        TagEntityId = 4,
                        Tag = new TagEntity { Name = "Adventurous"} 
                    },
                    new CatTag {
                        CatEntityId = 3,
                        Cat = new CatEntity(),
                        TagEntityId = 1,
                        Tag = new TagEntity { Name = "Playful"},
                    }
                ]
            },
            new CatEntity
            {
                Id = 4,
                CatId = "cat101",
                Width = 800,
                Height = 600,
                Image = "image4",
                Created = DateTime.UtcNow.AddDays(-4),
                CatTags =
                [
                    new CatTag {
                        CatEntityId = 4,
                        Cat = new CatEntity(),
                        TagEntityId = 5,
                        Tag = new TagEntity { Name = "Lazy"} 
                    },
                    new CatTag {
                        CatEntityId = 4,
                        Cat = new CatEntity(),
                        TagEntityId = 1,
                        Tag = new TagEntity { Name = "Playful"},
                    }
                ]
            }
        ];

    public static void SeedCatRepository(this Mock<ICatRepository> catRepository)
    {
        catRepository.Setup(x => x.ExistsAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) => CreateMockCatEntities().Any(c => c.Id.Equals(id)));

        Mock<DbSet<CatEntity>> catList = CreateMockCatEntities().AsQueryable().BuildMockDbSet();

        catRepository.Setup(x => x.GetCatsAsync())
            .Returns(catList.Object);

        catRepository.Setup(x => x.GetCatAsync(It.IsAny<string>()))
          .ReturnsAsync((string id) => CreateMockCatEntities().FirstOrDefault(c => c.Id.ToString().Equals(id)));
    }

    public static void SeedTagRepository(this Mock<ITagRepository> tagRepository)
    {
        tagRepository.Setup(x => x.FindTagByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((string name) => CreateMockTagEntities()
                                            .ToList()
                                            .FirstOrDefault(x => x.Name.Equals(name)));
    }
}
