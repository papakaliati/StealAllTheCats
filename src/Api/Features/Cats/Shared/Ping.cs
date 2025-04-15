using Microsoft.AspNetCore.Mvc;

namespace StealAllTheCats.Api.Features.Cats.Shared;

[ApiController]
[Route("api/ping")]
[Produces("application/json")]
public class Ping() : ControllerBase
{
    [HttpGet()]
    public async Task<IActionResult> Handle() => Ok(new {message= "OK!" });
}
