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

    [HttpPut("{id}/trips/{tripId}")]
    public async Task<IActionResult> AddClientToTrip([FromRoute] int id, [FromRoute] int tripId)
    {
        await dbService.AddClientToTripAsync(id, tripId);
        return Created($"/clients/{id}/trips/{tripId}", 
            new { Message = $"Dodano klienta {id} do wycieczki {tripId}." });
    }

}