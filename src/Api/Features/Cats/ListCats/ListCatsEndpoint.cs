using Hangfire;
using Microsoft.AspNetCore.Mvc;
using StealAllTheCats.Api.Features.Cats.Shared;
using Swashbuckle.AspNetCore.Annotations;

namespace StealAllTheCats.Api.Features.Cats.ListCats;

[ApiController]
[Route("api/cats")]
[Produces("application/json")]
public class ListCatsEndpoint(IListCatsService catService) : ControllerBase
{
    /// <summary>
    /// Retrieves a paginated list of cats.
    /// </summary>
    /// <param name="tag">Optional tag to filter cats.</param>
    /// <param name="page">The page number to retrieve (default is 1).</param>
    /// <param name="pageSize">The number of items per page (default is 10).</param>
    /// <returns>A paginated list of cats.</returns>
    [HttpGet]
    [SwaggerOperation(Tags = ["Cats"], Summary = "Get a paginated list of cats", Description = "Retrieves a paginated list of cats, optionally filtered by a tag.")]
    [SwaggerResponse(StatusCodes.Status200OK, "A paginated list of cats.", typeof(PaginatedList<CatDto>))]
    public async Task<IActionResult> Handle([FromQuery] string? tag, int page = 1, int pageSize = 10)
    {
        PaginatedList<CatDto> cats = await catService.GetCatsAsync(tag, page, pageSize);
        return Ok(cats);
    }
}