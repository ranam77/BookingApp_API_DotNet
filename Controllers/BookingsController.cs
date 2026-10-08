using BookingApi.Data;
using BookingApi.DTOs;
using BookingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace BookingApi.Controllers;

[ApiController]
[Route("bookings")]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public BookingsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("New")]
    [SwaggerOperation(Summary = "Create New Booking")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        request.CustomerName = request.CustomerName?.Trim() ?? ""; 

        if (string.IsNullOrWhiteSpace(request.slotCode) || string.IsNullOrWhiteSpace(request.CustomerName))
        {
            return BadRequest(new
            {
                error = new
                {
                    code = "VALIDATION_ERROR",
                    message = "Invalid booking data."
                }
            });
        }

        var slot = await _context.Slots.FirstOrDefaultAsync(s => s.Code == request.slotCode);
        if (slot == null )
        {
            return NotFound(new
            {
                error = new
                {
                    code = "SLOT_NOT_FOUND",
                    message = "The requested slot does not exist."
                }
            });
        }

        var booking = new Booking
        {
            SlotId = slot.ID,
            CustomerName = request.CustomerName, 
            Status = BookingStatus.Active, customerEmail = request.CustomerEmail
        };

        _context.Booking.Add(booking);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                error = new
                {
                    code = "SLOT_UNAVAILABLE",
                    message = "This slot already has an active booking."
                }
            });
        }

        return StatusCode(201, new
        {
            booking = new BookingResponse
            {
                SlotCode = slot.Code,
                CustomerName = booking.CustomerName,
                CustomerEmail = booking.customerEmail,
                Status = "active"
            }
        });
    }
    [HttpDelete("CancelBooking/{bookingId:int}")]
    [SwaggerOperation(Summary = "Cancel Existing Booking")]
    public async Task<IActionResult> CancelBooking(Int32 bookingID)
    {
        var booking = await _context.Booking.FirstOrDefaultAsync(b => b.ID == bookingID);

        if (booking == null)
        {
            return NotFound(new
            {
                error = new
                {
                    code = "BOOKING_NOT_FOUND",
                    message = "The requested booking does not exist."
                }
            });
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            return Ok(new
            {
                booking = new BookingResponse
                {
                    ID = booking.ID,
                    CustomerName = booking.CustomerName ,
                    Status = "cancelled"
                }
            });
        }
        booking.Status = BookingStatus.Cancelled;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            booking = new BookingResponse
            {
                ID = booking.ID,
                CustomerName = booking.CustomerName,
                Status = "cancelled"
            }
        });
    }
}