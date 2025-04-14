namespace StealAllTheCats.DTOs;

public record PaginatedList<T>(List<T> Items, int TotalCount);