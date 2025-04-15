using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace StealAllTheCats.Api.Features.Jobs.GetbyID;

[ApiController]
[Route("api/jobs")]
[Produces("application/json")]
public class GetJobStatusByI : ControllerBase
{
    /// <summary>
    /// Retrieves the status of a specific job by its ID.
    /// </summary>
    /// <param name="id">The ID of the job to retrieve the status for.</param>
    /// <returns>The status of the job, or "NotFound" if the job does not exist.</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Tags = ["Jobs"], Summary = "Get job status", Description = "Retrieves the status of a specific job by its ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "The status of the job.", typeof(object))]
    public IActionResult GetStatus(string id)
    {
        string? status = JobStorage.Current.GetMonitoringApi().JobDetails(id)?.History?.FirstOrDefault()?.StateName;
        return Ok(new { JobId = id, Status = status ?? "NotFound" });
    }
}
