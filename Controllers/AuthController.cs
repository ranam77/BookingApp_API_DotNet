using BookingApi.Data;
using BookingApi.DTOs;
using BookingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration , AppDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpPost("token")]
    public async Task<IActionResult> CreateToken([FromBody] CreateTokenRequest request)
    {
        var key = _configuration["Jwt:Key"]!;
        var issuer = _configuration["Jwt:Issuer"]!;
        var audience = _configuration["Jwt:Audience"]!; 
        var isExist = await _context.projects.FirstOrDefaultAsync(u => u.clientusername == request.username && u.clientpassword == GetMD5(request.password));
        if (isExist == null)
        {
            return NotFound(new
            {
                error = new
                {
                    code = "USER_NOT_FOUND",
                    message = "Enter Correct username and password."
                }
            });
        }
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, request.username)
        };

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials);

        return Ok(new
        {
            access_token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
    public static string GetMD5(string input)
    {
        byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(input));

        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}