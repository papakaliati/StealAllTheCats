namespace StealAllTheCats.Api.Features.Cats.Shared;

public record PaginatedList<T>(List<T> Items, int TotalCount);