using APBD_CW_API2.Models.DTOs;
using APBD_CW_API2.Services;
using Microsoft.AspNetCore.Mvc;
namespace APBD_CW_API2.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class ClientsController(IDbService dbService) : ControllerBase
{
    [HttpGet ("{id}")]
    public async Task<IActionResult> GetClientTripsByClientId(
        [FromRoute] int id)
    {
     return Ok(await dbService.GetTripsByClientIdAsync(id));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] ClientCreateDTO body)
    {
        var client = await dbService.CreateClientAsync(body);
        return Created($"clients/{client.IdClient}", client);
    }
}