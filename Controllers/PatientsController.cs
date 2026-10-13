
using BookingApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public PatientsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("New")]
    [SwaggerOperation(Summary = "Create New Patient")]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequest request)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

        await using var connection = new SqlConnection(connectionString);

        await using var command = new SqlCommand("dbo.CreatePatient", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@firstName", SqlDbType.NVarChar, 200).Value = request.firstName;
        command.Parameters.Add("@lastName", SqlDbType.NVarChar, 200).Value = request.lastName;
        command.Parameters.Add("@Email", SqlDbType.NVarChar, 256).Value = (object?)request.Email ?? DBNull.Value;
        command.Parameters.Add("@Phone", SqlDbType.NVarChar, 30).Value = request.Phone;
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return StatusCode(500, new { message = "No result returned." });
        bool success = reader.GetBoolean(reader.GetOrdinal("Success"));
        Int64 patientId = reader.IsDBNull(reader.GetOrdinal("PatientId")) ? 0 : reader.GetInt64(reader.GetOrdinal("PatientId"));
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
}
