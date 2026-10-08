using BookingApi.Data;
using BookingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
namespace BookingApi.Controllers;

[ApiController]
[Route("slots")]
public class SlotsController : ControllerBase
{
    private readonly AppDbContext _context;

    public SlotsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("validSlotes")] 
    [SwaggerOperation(Summary = "Get Avaliable Slots")]
    public async Task<IActionResult> GetAvailableSlots()
    {
        

       try
        {
            var slots = await _context.Slots
                 .Where(s => !s.Booking.Any(b => b.Status == BookingStatus.Active))
                .OrderBy(s => s.startsAt)
                .ThenBy(s => s.ID)
                .Select(s => new
                {
                    s.ID,
                    s.startsAt,
                    s.endsAt,
                    s.Code
                })
                .ToListAsync();

            return Ok(slots);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }
}