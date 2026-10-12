using BookingApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BookingApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {

        Project testuser = new Project();
        testuser.businessname = "Alfa Clinics";
        testuser.clientname = "Dr. Rana Mustafa"; 
        testuser.clientusername = "admin";
        testuser.clientpassword = AuthController.GetMD5("admin");
        if (!await context.projects.AnyAsync())
        {
            context.projects.Add(testuser);
            await context.SaveChangesAsync();
        }

        if (await context.Slots.AnyAsync())
            return;

        var slots = new List<Slots>
        {
            new Slots
            {
                startsAt = new DateTime(2030, 1, 15, 9, 0, 0, DateTimeKind.Utc),
                endsAt = new DateTime(2030, 1, 15, 9, 30, 0, DateTimeKind.Utc),
                Code = "A-6542"
            },
            new Slots
            {
                startsAt = new DateTime(2030, 1, 15, 10, 0, 0, DateTimeKind.Utc),
                endsAt = new DateTime(2030, 1, 15, 10, 30, 0, DateTimeKind.Utc),
                Code = "A-6544"
            },
            new Slots
            {
                startsAt = new DateTime(2030, 1, 15, 11, 0, 0, DateTimeKind.Utc),
                endsAt = new DateTime(2030, 1, 15, 11, 30, 0, DateTimeKind.Utc),
                Code = "A-6540"
            },
            new Slots
            {
                startsAt = new DateTime(2030, 1, 15, 13, 0, 0, DateTimeKind.Utc),
                endsAt = new DateTime(2030, 1, 15, 13, 30, 0, DateTimeKind.Utc),
                Code = "A-6546"
            }
        };

        if (!await context.Slots.AnyAsync())
        {
            context.Slots.AddRange(slots);
            await context.SaveChangesAsync();
        }

        
    }
}