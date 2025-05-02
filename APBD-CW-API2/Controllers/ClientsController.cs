using APBD_CW_API2.Services;
using Microsoft.AspNetCore.Mvc;
namespace APBD_CW_API2.Controllers;

[ApiController]
[Route("[Controller]")]
public class ClientsController(IDbService dbService) : ControllerBase
{
    [HttpGet ("{id}")]
    public async Task<IActionResult> GetClientTripsByClientId(
        [FromRoute] int id)
    {
     return Ok(await dbService.GetTripsByClientIdAsync(id));
    }
}