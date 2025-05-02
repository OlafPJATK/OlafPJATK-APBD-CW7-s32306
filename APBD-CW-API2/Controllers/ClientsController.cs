using APBD_CW_API2.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_CW_API2.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientsController(IDbService dbService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllClients()
    {
     return Ok(await dbService.GetMyClientsAsync());
    }
}