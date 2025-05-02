using Microsoft.AspNetCore.Mvc;
using APBD_CW_API2.Services;
namespace APBD_CW_API2.Controllers;


[ApiController]
[Route("api/[Controller]")]
public class TripsController(IDbService dbService) : ControllerBase
{
    
    [HttpGet]
    public async Task<IActionResult> GetAllTrips()
    {
        return Ok(await dbService.GetAllTripsAsync());
    }
}