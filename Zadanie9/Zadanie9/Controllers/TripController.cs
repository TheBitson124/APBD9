using Microsoft.EntityFrameworkCore;
using Zadanie9.Data;
using Zadanie9.DTOs;

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
        var query  = _context.Trips.OrderByDescending(t => t.DateFrom).Include(t => t.IdCountries)
            .Skip((page - 1) * pageSize).Take(pageSize);
        var count = await _context.Trips.CountAsync();
        List<Trip> trips = await query.ToListAsync();
        var result = new
        {
            pageNum = page,
            pageSize = pageSize,
            allPages = (int)Math.Ceiling(count/(double)pageSize),
            trips = trips.Select((Trip t) => new
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries.Select(x => new { x.Name }),
                Clients = _context.ClientTrips.Include(x => x.IdClientNavigation)
                    .Where(x => x.IdTrip == t.IdTrip).Select(x => new
                    {
                        FirstName = x.IdClientNavigation.FirstName, LastName = x.IdClientNavigation.LastName
                    })
            })
        };
        return Ok(result);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCLient(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
        {
            return NotFound($"Client with id :{id} doesnt exist");
        }

        var query = await _context.ClientTrips.AnyAsync(trip => trip.IdClient == id);
        if (query)
        {
            return NotFound("Client has Trips");
        }

        _context.Clients.Remove(client);
        return Ok("Client deleted");
    }

    [HttpPost("{idTrip:int}/clients")]
    public async Task<IActionResult> AddClientToTrip(int idTrip, ClientToTripDTO addClient)
    {
        var trip = await _context.Trips.FindAsync(idTrip);
        if (trip == null)
        {
            return NotFound($"Trip with id :{idTrip} doesnt exist");

        }
        if (trip.DateFrom < DateTime.Now)
        {
            return NotFound($"Trip with id :{idTrip} has DateFrom not in the future");
        }
        var client = await _context.Clients.AnyAsync(c => c.Pesel == addClient.Pesel);
        var trips = await _context.ClientTrips.AnyAsync(ct => ct.IdClientNavigation.Pesel == addClient.Pesel && ct.IdTrip == idTrip);
        if (trips)
        {
            return NotFound($"Client with Pesel :{addClient.Pesel} has trip with id : {idTrip} already");
        }

        var klient = new Client()
        {
            Email = addClient.Email,
            FirstName = addClient.FirstName,
            LastName = addClient.LastName,
            Pesel = addClient.Pesel,
            Telephone = addClient.Telephone,
        };
        var klientTrip = new ClientTrip()
        {
            IdClientNavigation = klient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = addClient.PaymentDate
        };
        if (!client)
        {
            await _context.Clients.AddAsync(klient);
        }
        await _context.ClientTrips.AddAsync(klientTrip);
        await _context.SaveChangesAsync();

        return Ok("Client added");
    }
}