using System.ComponentModel.DataAnnotations;

namespace SmrScheduler.Data;

public enum AppointmentStatus
{
    Scheduled = 0,
    InProgress = 1,
    Completed = 2,
    NoShow = 3
}

public sealed class Branch
{
    public int Id { get; set; }
    [MaxLength(120)] public required string Name { get; set; }
    [MaxLength(160)] public required string Address { get; set; }
    public List<Mechanic> Mechanics { get; set; } = [];
    public List<AppointmentSlot> Slots { get; set; } = [];
}

public sealed class Mechanic
{
    public int Id { get; set; }
    [MaxLength(100)] public required string Name { get; set; }
    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
    public List<AppointmentSlot> Slots { get; set; } = [];
}

public sealed class ServiceType
{
    public int Id { get; set; }
    [MaxLength(80)] public required string Name { get; set; }
    public int DurationMinutes { get; set; }
    public List<AppointmentSlot> Slots { get; set; } = [];
}

public sealed class AppointmentSlot
{
    public int Id { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public int BayNumber { get; set; }
    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
    public int MechanicId { get; set; }
    public Mechanic? Mechanic { get; set; }
    public int ServiceTypeId { get; set; }
    public ServiceType? ServiceType { get; set; }
    public Booking? Booking { get; set; }
}

public sealed class Booking
{
    public int Id { get; set; }
    [MaxLength(16)] public required string ReferenceNumber { get; set; }
    [MaxLength(120)] public required string CustomerName { get; set; }
    [MaxLength(40)] public required string CustomerPhone { get; set; }
    [MaxLength(20)] public required string VehicleRegistration { get; set; }
    [MaxLength(1000)] public string? CustomerNotes { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int AppointmentSlotId { get; set; }
    public AppointmentSlot? AppointmentSlot { get; set; }
    public List<WorkNote> WorkNotes { get; set; } = [];
}

public sealed class WorkNote
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [MaxLength(1000)] public required string Note { get; set; }
}
