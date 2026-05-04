using Microsoft.EntityFrameworkCore;

namespace SmrScheduler.Data;

public static class SeedData
{
    public static async Task EnsureSeededAsync(SmrDbContext db)
    {
        if (await db.Branches.AnyAsync())
        {
            return;
        }

        var branches = new[]
        {
            new Branch { Name = "Bristol Central", Address = "Temple Gate, Bristol" },
            new Branch { Name = "Manchester North", Address = "Oldham Road, Manchester" }
        };

        var services = new[]
        {
            new ServiceType { Name = "Inspection", DurationMinutes = 45 },
            new ServiceType { Name = "Service", DurationMinutes = 90 },
            new ServiceType { Name = "Repair", DurationMinutes = 120 },
            new ServiceType { Name = "Diagnostics", DurationMinutes = 60 }
        };

        var mechanics = new[]
        {
            new Mechanic { Name = "Amira Patel", Branch = branches[0] },
            new Mechanic { Name = "Ben Hughes", Branch = branches[0] },
            new Mechanic { Name = "Cara Morgan", Branch = branches[1] },
            new Mechanic { Name = "Dan Wilson", Branch = branches[1] }
        };

        db.AddRange(branches);
        db.AddRange(services);
        db.AddRange(mechanics);
        await db.SaveChangesAsync();

        var today = DateTime.Today;
        var startTimes = new[] { new TimeSpan(8, 30, 0), new TimeSpan(10, 30, 0), new TimeSpan(13, 00, 0), new TimeSpan(15, 00, 0) };
        var slots = new List<AppointmentSlot>();

        for (var day = 0; day < 7; day++)
        {
            foreach (var mechanic in mechanics)
            {
                for (var i = 0; i < startTimes.Length; i++)
                {
                    var service = services[(day + i + mechanic.Id) % services.Length];
                    var startsAt = today.AddDays(day).Add(startTimes[i]);
                    slots.Add(new AppointmentSlot
                    {
                        StartsAt = startsAt,
                        EndsAt = startsAt.AddMinutes(service.DurationMinutes),
                        BayNumber = (mechanic.Id % 2) + 1,
                        BranchId = mechanic.BranchId,
                        MechanicId = mechanic.Id,
                        ServiceTypeId = service.Id
                    });
                }
            }
        }

        db.AppointmentSlots.AddRange(slots);
        await db.SaveChangesAsync();

        var firstSlot = slots.First(slot => slot.StartsAt.Date == today && slot.BranchId == branches[0].Id);
        db.Bookings.Add(new Booking
        {
            ReferenceNumber = "SMR-1001",
            CustomerName = "Jamie Carter",
            CustomerPhone = "07123 456789",
            VehicleRegistration = "AB12 CDE",
            CustomerNotes = "Intermittent warning light and rough idle.",
            AppointmentSlotId = firstSlot.Id
        });

        await db.SaveChangesAsync();
    }
}
