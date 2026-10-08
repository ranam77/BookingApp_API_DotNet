using BookingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Slots> Slots => Set<Slots>();

    public DbSet<Booking> Booking  => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Slot)
            .WithMany(s => s.Booking)
            .HasForeignKey(b => b.SlotId);

        // Only one ACTIVE booking per slot.
        modelBuilder.Entity<Booking>()
            .HasIndex(b => b.SlotId)
            .IsUnique()
            .HasFilter("\"Status\" = 0");

        
    }
}