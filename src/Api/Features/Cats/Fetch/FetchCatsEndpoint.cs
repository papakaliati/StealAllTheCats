using Microsoft.AspNetCore.Mvc;
using StealAllTheCats.Api.Features.Cats.Fetch;
using Swashbuckle.AspNetCore.Annotations;

namespace StealAllTheCats.Api.Features.Cats.GetbyID;

[ApiController]
[Route("api/cats")]
[Produces("application/json")]
public class FetchCatsEndpoint(IFetchCatsService fetchCatsService) : ControllerBase
{
    public record FetchCatchResult(string JobId);

    /// <summary>
    /// Enqueues a job to fetch cats from an external source.
    /// </summary>
    /// <returns>The ID of the enqueued job.</returns>
    [HttpPost("fetch")]
    [SwaggerOperation(Tags = ["Cats"], Summary = "Enqueue a job to fetch cats", Description = "Enqueues a job to fetch cats from an external source.")]
    [SwaggerResponse(StatusCodes.Status200OK, "The ID of the enqueued job. You inquire it's state using the Get job status endpoint (/api/jobs/{id}).", typeof(FetchCatchResult))]
    public IActionResult FetchCats()
    {
        string jobId = fetchCatsService.EnqueueCatFetchJob();
        return Ok(new FetchCatchResult(jobId));
    }
}
