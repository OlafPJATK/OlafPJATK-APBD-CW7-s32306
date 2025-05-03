using Microsoft.AspNetCore.Mvc;
using APBD_CW_API2.Services;
namespace APBD_CW_API2.Controllers;

// Kontroler odpowiedzialny za operacje na wycieczkach
[ApiController]
[Route("api/[controller]")]
public class TripsController(IDbService dbService) : ControllerBase
{
    // Zwraca wszystkie dostępne wycieczki wraz z informacjami o krajach
    [HttpGet]
    public async Task<IActionResult> GetAllTrips()
    {
        return Ok(await dbService.GetAllTripsAsync());
    }
}
