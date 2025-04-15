namespace StealAllTheCats.Api.Features.Cats.Shared;

public record CatDto(int Id,
                     string CatId,
                     int Width,
                     int Height,
                     byte[] Image,
                     List<string> Tags,
                     DateTime Created);