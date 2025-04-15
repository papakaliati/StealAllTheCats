using Microsoft.AspNetCore.Mvc;
using StealAllTheCats.Api.Features.Cats.Shared;
using Swashbuckle.AspNetCore.Annotations;

namespace StealAllTheCats.Api.Features.Cats.GetbyID;

[ApiController]
[Route("api/cats")]
[Produces("application/json")]
public class GetCatsByIdEndpoint(IGetCatByIdService getCatByIdService) : ControllerBase
{
    /// <summary>
    /// Retrieves a specific cat by its ID.
    /// </summary>
    /// <param name="id">The ID of the cat to retrieve.</param>
    /// <returns>The cat with the specified ID, or a 404 if not found.</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Tags = ["Cats"], Summary = "Get a specific cat by ID", Description = "Retrieves a specific cat by its ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "The cat with the specified ID.", typeof(CatDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Cat not found.")]
    public async Task<IActionResult> Handle(string id)
    {
        CatDto? cat = await getCatByIdService.GetCatByIdAsync(id);

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
}
