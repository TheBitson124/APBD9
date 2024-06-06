using Zadanie9.Data;

namespace Zadanie9.Controllers;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
[Route("api/[controller]")]
[ApiController]
public class TripController : ControllerBase
{
    private readonly EntityFrameworkContext _context;
    public TripController(EntityFrameworkContext context) 
    { 
        _context = context; 
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        
        return Ok(result);
    }
}