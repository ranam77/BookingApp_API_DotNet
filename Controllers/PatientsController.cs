
using BookingApi.Controllers;
using BookingApi.Data;
using BookingApi.DTOs;
using BookingApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;
using System.Security.Cryptography;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PatientsController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PatientsController> _logger;
    public PatientsController(IConfiguration configuration, ILogger<PatientsController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("new")]
    [SwaggerOperation(Summary = "Create New Patient")]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequest request)
    {
        try {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand("dbo.CreatePatient", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@firstName", SqlDbType.NVarChar, 200).Value = request.firstName;
            command.Parameters.Add("@lastName", SqlDbType.NVarChar, 200).Value = request.lastName;
            command.Parameters.Add("@username", SqlDbType.NVarChar, 200).Value = request.username; 
            command.Parameters.Add("@password", SqlDbType.NVarChar, 200).Value = AuthController.GetMD5(request.password); 
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 256).Value = (object?)request.Email ?? DBNull.Value;
            command.Parameters.Add("@Phone", SqlDbType.NVarChar, 30).Value = request.Phone;
            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return StatusCode(500, new { message = "No result returned." });
            bool success = reader.GetBoolean(reader.GetOrdinal("Success"));
            long patientId = reader.IsDBNull(reader.GetOrdinal("PatientId")) ? 0 : reader.GetInt64(reader.GetOrdinal("PatientId"));
            string message = reader.GetString(reader.GetOrdinal("Message"));
            if (!success)
            {
                return Conflict(new CreateResult
                {
                    Success = false,
                    Id = patientId,
                    Message = message
                });
            }

            return Created($"/api/patients/{patientId}",
                new CreateResult
                {
                    Success = true,
                    Id = patientId,
                    Message = message
                });
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error while CreatePatient ");
            return StatusCode(500, new { success = false, message = "An unexpected error occurred." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while CreatePatient ");
            return StatusCode(500, new { success = false, message = "An unexpected error occurred." });
        }

    }

    [HttpPost("bookings")]
    [SwaggerOperation(Summary = "Get Patient Current Bookings")]
    public async Task<IActionResult> GetPatientBookings([FromBody] PatientBookingRequest requesr)
    {
        var bookings = new List<Booking>();
        Patient patient = null;
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand("dbo.GetPatientBookings", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@PatientID", SqlDbType.BigInt).Value = requesr.PatientId;
            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (patient == null)
                {
                    patient = new Patient
                    {
                        Id = reader.GetInt64(reader.GetOrdinal("PatientId")),
                        firstName = reader.GetString(reader.GetOrdinal("PatientFirstName")),
                        lastName = reader.IsDBNull(reader.GetOrdinal("PatientLastName")) ? null : reader.GetString(reader.GetOrdinal("PatientLastName")),
                        Phone = reader.GetString(reader.GetOrdinal("PatientPhone")),
                        Email = reader.IsDBNull(reader.GetOrdinal("PatientEmail")) ? null : reader.GetString(reader.GetOrdinal("PatientEmail"))
                    };
                }
                var booking = new Booking
                {
                    ID = reader.GetInt64(reader.GetOrdinal("BookingId")),
                    SlotId = reader.GetInt64(reader.GetOrdinal("SlotId")),
                    BookingRef = reader.GetGuid(reader.GetOrdinal("BookingRef")),
                    Status = Enum.Parse<BookingStatus>(
                        reader.GetString(reader.GetOrdinal("BookingStatus")),
                        true
                    )
                    ,
                    Slot = new Slots
                    {
                        Id = reader.GetInt64(reader.GetOrdinal("SlotId")),
                        Code = reader.GetString(reader.GetOrdinal("SlotCode")),
                        startsAt = reader.GetDateTime(reader.GetOrdinal("StartsAt")),
                        endsAt = reader.GetDateTime(reader.GetOrdinal("EndsAt"))
                    },


                    doctor = new Doctor
                    {
                        Id = reader.GetInt64(reader.GetOrdinal("DoctorId")),
                        firstName = reader.GetString(reader.GetOrdinal("DoctorFirstName")),
                        lastName = reader.GetString(reader.GetOrdinal("DoctorLastName"))
                    },

                    clinic = new Clinic
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("ClinicId")),
                        Name = reader.GetString(reader.GetOrdinal("ClinicName"))
                    },

                    speciality = new Speciality
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("SpecialtyId")),
                        Name = reader.GetString(reader.GetOrdinal("SpecialtyName"))
                    }
                };
                
                bookings.Add(booking);
                patient.Bookings = bookings;
            }
          
            return Ok(patient);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database Error retrieving bookings for patient {PatientId}", requesr.PatientId);
            return StatusCode(500, new { message = "An error occurred while retrieving patient bookings." });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving bookings for patient {PatientId}", requesr.PatientId);
            return StatusCode(500, new { message = "An error occurred while retrieving patient bookings." });
        }
    }
}
