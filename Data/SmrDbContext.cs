using Microsoft.EntityFrameworkCore;

namespace SmrScheduler.Data;

public sealed class SmrDbContext(DbContextOptions<SmrDbContext> options) : DbContext(options)
{
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Mechanic> Mechanics => Set<Mechanic>();
    public DbSet<ServiceType> ServiceTypes => Set<ServiceType>();
    public DbSet<AppointmentSlot> AppointmentSlots => Set<AppointmentSlot>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<WorkNote> WorkNotes => Set<WorkNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>()
            .HasIndex(booking => booking.ReferenceNumber)
            .IsUnique();

        modelBuilder.Entity<Booking>()
            .HasIndex(booking => booking.AppointmentSlotId)
            .IsUnique();

        modelBuilder.Entity<AppointmentSlot>()
            .HasIndex(slot => new { slot.BranchId, slot.StartsAt, slot.BayNumber })
            .IsUnique();

        modelBuilder.Entity<WorkNote>()
            .Property(note => note.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        modelBuilder.Entity<Booking>()
            .Property(booking => booking.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");
    }
}
