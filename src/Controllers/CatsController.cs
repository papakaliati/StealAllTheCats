using Microsoft.AspNetCore.Mvc;
using StealAllTheCats.DTOs;
using StealAllTheCats.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace StealAllTheCats.Controllers;

[ApiController]
[Route("api/cats")]
[Produces("application/json")]
public class CatsController(ICatService catService) : ControllerBase
{
    /// <summary>
    /// Retrieves a paginated list of cats.
    /// </summary>
    /// <param name="tag">Optional tag to filter cats.</param>
    /// <param name="page">The page number to retrieve (default is 1).</param>
    /// <param name="pageSize">The number of items per page (default is 10).</param>
    /// <returns>A paginated list of cats.</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Get a paginated list of cats", Description = "Retrieves a paginated list of cats, optionally filtered by a tag.")]
    [SwaggerResponse(StatusCodes.Status200OK, "A paginated list of cats.", typeof(PaginatedList<CatDto>))]
    public async Task<IActionResult> Handle([FromQuery] string? tag, int page = 1, int pageSize = 10)
    {
        PaginatedList<CatDto> cats = await catService.GetCatsAsync(tag, page, pageSize);
        return Ok(cats);
    }

    /// <summary>
    /// Retrieves a specific cat by its ID.
    /// </summary>
    /// <param name="id">The ID of the cat to retrieve.</param>
    /// <returns>The cat with the specified ID, or a 404 if not found.</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get a specific cat by ID", Description = "Retrieves a specific cat by its ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "The cat with the specified ID.", typeof(CatDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Cat not found.")]
    public async Task<IActionResult> Handle(string id)
    {
        CatDto? cat = await catService.GetCatByIdAsync(id);

        if (cat == null)
        {
            return NotFound(new
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Cat not found",
                Detail = $"No cat found with id {id}."
            });
        }

        return Ok(cat);
    }

    public record FetchCatchResult(string JobId);

    /// <summary>
    /// Enqueues a job to fetch cats from an external source.
    /// </summary>
    /// <returns>The ID of the enqueued job.</returns>
    [HttpPost("fetch")]
    [SwaggerOperation(Summary = "Enqueue a job to fetch cats", Description = "Enqueues a job to fetch cats from an external source.")]
    [SwaggerResponse(StatusCodes.Status200OK, "The ID of the enqueued job.", typeof(FetchCatchResult))]
    public async Task<IActionResult> FetchCatsAsync()
    {
        string jobId = catService.EnqueueCatFetchJob();
        return Ok(new FetchCatchResult(jobId));
    }
}
