using BookingApi.DTOs;
using BookingApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;

namespace BookingApi.Controllers;

[Authorize]
[ApiController]
[Route("api/bookings")]
public class BookingsController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<BookingsController> _logger;
    public BookingsController(IConfiguration configuration , ILogger<BookingsController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("new")]
    [SwaggerOperation(Summary = "Create New Booking")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        try {
            if (request.SlotId == 0 || request.PatientId == 0)
            {
                return BadRequest(new {error = new { code = "VALIDATION_ERROR",message = "Invalid booking data."}});
            }
            var response = new BookingResponse();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand("dbo.CreateBooking", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@SlotId", SqlDbType.BigInt).Value = request.SlotId;
            command.Parameters.Add("@PatientId", SqlDbType.BigInt).Value = request.PatientId;
            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                response.Success = reader.GetBoolean(reader.GetOrdinal("Success"));
                int bookingIdOrdinal = reader.GetOrdinal("BookingId");
                int bookingRefOrdinal = reader.GetOrdinal("BookingRef");
                response.Id = reader.IsDBNull(bookingIdOrdinal) ? 0 : reader.GetInt64(bookingIdOrdinal);
                response.BookingRef = reader.IsDBNull(bookingRefOrdinal) ? null : reader.GetGuid(bookingRefOrdinal);
                response.Message = reader.GetString(reader.GetOrdinal("Message"));
            }

            if (!response.Success)
                return Conflict(response);
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error while CreateBooking ");
            return StatusCode(500, new { success = false, message = "An unexpected error occurred." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while CreateBooking ");
            return StatusCode(500, new { success = false, message = "An unexpected error occurred." });
        }
    }
    
    [HttpPost("{bookingRef:guid}/cancel")]
    [SwaggerOperation(Summary = "Cancel Booking")]
    public async Task<IActionResult> CancelBooking(Guid bookingRef)
{
    try
    {
        await using var connection =new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await using var command = new SqlCommand("dbo.CancelBooking", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@BookingRef", SqlDbType.UniqueIdentifier).Value = bookingRef;
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return StatusCode(500, new
            {
                success = false,
                message = "The database returned an unexpected response."
            });
        }
        bool success = Convert.ToBoolean(reader["Success"]);
        string message = Convert.ToString(reader["Message"]) ?? "";
        if (!success)
        {
            if (message.Contains("not found",StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new{success = false,message});
            }
            return Conflict(new{success = false,message});
        }
        return Ok(new {success = true,bookingRef,message});
    }
    catch (SqlException ex)
    {
        _logger.LogError( ex, "Database error while cancelling booking {BookingRef}",bookingRef);
        return StatusCode(500, new {success = false,message = "A database error occurred while cancelling the booking."});
    }
    catch (Exception ex)
    {
        _logger.LogError(ex,"Unexpected error while cancelling booking {BookingRef}",bookingRef);
        return StatusCode(500, new{success = false,message = "An unexpected error occurred."});
    }
    }
    
}