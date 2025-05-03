using APBD_CW_API2.Exceptions;
using APBD_CW_API2.Models.DTOs;
using APBD_CW_API2.Services;
using Microsoft.AspNetCore.Mvc;
namespace APBD_CW_API2.Controllers;

// Kontroler obsługujący operacje związane z klientami i ich wycieczkami
[ApiController]
[Route("api/[Controller]")]
public class ClientsController(IDbService dbService) : ControllerBase
{
    // Zwraca listę wycieczek, w których uczestniczy dany klient
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetTripsByClientId([FromRoute] int id)
    {
        try
        {
            return Ok(await dbService.GetTripsByClientIdAsync(id));
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }   
    }

    // Tworzy nowego klienta w systemie
    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] ClientCreateDTO body)
    {
        try
        {
            var client = await dbService.CreateClientAsync(body);
            return Created($"clients/{client.IdClient}", client);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // Dodaje istniejącego klienta do wycieczki
    [HttpPut("{id}/trips/{tripId}")]
    public async Task<IActionResult> AddClientToTrip([FromRoute] int id, [FromRoute] int tripId)
    {
        try
        {
            await dbService.AddClientToTripAsync(id, tripId);
            return Ok(new { Message = $"Dodano klienta {id} do wycieczki {tripId}." });
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch(NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    // Usuwa klienta z przypisanej wycieczki
    [HttpDelete("{id}/trips/{tripId}")]
    public async Task<IActionResult> RemoveClientFromTrip([FromRoute] int id, [FromRoute] int tripId)
    {
        try
        {
            await dbService.RemoveClientFromTripAsync(id, tripId);
            return Ok("Usunięto klienta z wycieczki.");
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}
