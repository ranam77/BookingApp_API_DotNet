using BookingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Slots.AnyAsync())
            return;

        var slots = new List<Slots>
        {
            new Slots
            {
                ID = 1,
                startsAt = new DateTime(2030, 1, 15, 9, 0, 0, DateTimeKind.Utc),
                endsAt = new DateTime(2030, 1, 15, 9, 30, 0, DateTimeKind.Utc),
                Code = "A-6542"
            },
            new Slots
            {
                ID = 2,
                startsAt = new DateTime(2030, 1, 15, 10, 0, 0, DateTimeKind.Utc),
                endsAt = new DateTime(2030, 1, 15, 10, 30, 0, DateTimeKind.Utc),
                Code = "A-6544"
            },
            new Slots
            {
                ID = 3,
                startsAt = new DateTime(2030, 1, 15, 11, 0, 0, DateTimeKind.Utc),
                endsAt = new DateTime(2030, 1, 15, 11, 30, 0, DateTimeKind.Utc),
                Code = "A-6540"
            },
            new Slots
            {
                ID =4,
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