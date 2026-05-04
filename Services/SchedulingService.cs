using Microsoft.EntityFrameworkCore;
using SmrScheduler.Data;

namespace SmrScheduler.Services;

public sealed class SchedulingService(IDbContextFactory<SmrDbContext> dbFactory)
{
    public async Task<List<Branch>> GetBranchesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Branches.OrderBy(branch => branch.Name).ToListAsync();
    }

    public async Task<List<ServiceType>> GetServiceTypesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.ServiceTypes.OrderBy(service => service.Name).ToListAsync();
    }

    public async Task<List<Mechanic>> GetMechanicsAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Mechanics.Include(mechanic => mechanic.Branch).OrderBy(mechanic => mechanic.Name).ToListAsync();
    }

    public async Task<List<AppointmentSlot>> GetAvailableSlotsAsync(int? serviceTypeId, int? branchId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var today = DateTime.Today;
        var end = today.AddDays(7);

        var query = db.AppointmentSlots
            .Include(slot => slot.Branch)
            .Include(slot => slot.Mechanic)
            .Include(slot => slot.ServiceType)
            .Include(slot => slot.Booking)
            .Where(slot => slot.StartsAt >= today && slot.StartsAt < end && slot.Booking == null);

        if (serviceTypeId is > 0)
        {
            query = query.Where(slot => slot.ServiceTypeId == serviceTypeId.Value);
        }

        if (branchId is > 0)
        {
            query = query.Where(slot => slot.BranchId == branchId.Value);
        }

        return await query.OrderBy(slot => slot.StartsAt).ThenBy(slot => slot.Branch!.Name).ToListAsync();
    }

    public async Task<BookingResult> BookAsync(BookingRequest request)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var slot = await db.AppointmentSlots
            .Include(appointmentSlot => appointmentSlot.Booking)
            .FirstOrDefaultAsync(appointmentSlot => appointmentSlot.Id == request.AppointmentSlotId);

        if (slot is null)
        {
            return BookingResult.Failed("The selected slot no longer exists.");
        }

        if (slot.Booking is not null)
        {
            return BookingResult.Failed("That appointment slot has already been booked.");
        }

        var booking = new Booking
        {
            ReferenceNumber = await GenerateReferenceAsync(db),
            CustomerName = request.CustomerName.Trim(),
            CustomerPhone = request.CustomerPhone.Trim(),
            VehicleRegistration = request.VehicleRegistration.Trim().ToUpperInvariant(),
            CustomerNotes = request.CustomerNotes?.Trim(),
            AppointmentSlotId = request.AppointmentSlotId
        };

        db.Bookings.Add(booking);

        try
        {
            await db.SaveChangesAsync();
            return BookingResult.Succeeded(booking.ReferenceNumber);
        }
        catch (DbUpdateException)
        {
            return BookingResult.Failed("That appointment slot has just been taken. Please choose another slot.");
        }
    }

    public async Task<List<Booking>> GetMechanicAppointmentsAsync(int mechanicId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var today = DateTime.Today;
        var end = today.AddDays(2);

        return await db.Bookings
            .Include(booking => booking.AppointmentSlot)!.ThenInclude(slot => slot!.Branch)
            .Include(booking => booking.AppointmentSlot)!.ThenInclude(slot => slot!.Mechanic)
            .Include(booking => booking.AppointmentSlot)!.ThenInclude(slot => slot!.ServiceType)
            .Where(booking => booking.AppointmentSlot!.MechanicId == mechanicId
                && booking.AppointmentSlot.StartsAt >= today
                && booking.AppointmentSlot.StartsAt < end)
            .OrderBy(booking => booking.AppointmentSlot!.StartsAt)
            .ToListAsync();
    }

    public async Task<Booking?> GetBookingAsync(int bookingId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Bookings
            .Include(booking => booking.AppointmentSlot)!.ThenInclude(slot => slot!.Branch)
            .Include(booking => booking.AppointmentSlot)!.ThenInclude(slot => slot!.Mechanic)
            .Include(booking => booking.AppointmentSlot)!.ThenInclude(slot => slot!.ServiceType)
            .Include(booking => booking.WorkNotes.OrderByDescending(note => note.CreatedAt))
            .FirstOrDefaultAsync(booking => booking.Id == bookingId);
    }

    public async Task<List<Booking>> GetTodaysScheduleAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        return await db.Bookings
            .Include(booking => booking.AppointmentSlot)!.ThenInclude(slot => slot!.Branch)
            .Include(booking => booking.AppointmentSlot)!.ThenInclude(slot => slot!.Mechanic)
            .Include(booking => booking.AppointmentSlot)!.ThenInclude(slot => slot!.ServiceType)
            .Where(booking => booking.AppointmentSlot!.StartsAt >= today && booking.AppointmentSlot.StartsAt < tomorrow)
            .OrderBy(booking => booking.AppointmentSlot!.StartsAt)
            .ThenBy(booking => booking.AppointmentSlot!.Mechanic!.Name)
            .ToListAsync();
    }

    public async Task UpdateStatusAsync(int bookingId, AppointmentStatus status)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var booking = await db.Bookings.FindAsync(bookingId);
        if (booking is null)
        {
            return;
        }

        booking.Status = status;
        await db.SaveChangesAsync();
    }

    public async Task AddWorkNoteAsync(int bookingId, string noteText)
    {
        if (string.IsNullOrWhiteSpace(noteText))
        {
            return;
        }

        await using var db = await dbFactory.CreateDbContextAsync();
        db.WorkNotes.Add(new WorkNote
        {
            BookingId = bookingId,
            Note = noteText.Trim()
        });
        await db.SaveChangesAsync();
    }

    private static async Task<string> GenerateReferenceAsync(SmrDbContext db)
    {
        var nextId = await db.Bookings.CountAsync() + 1001;
        string reference;
        do
        {
            reference = $"SMR-{nextId++}";
        } while (await db.Bookings.AnyAsync(booking => booking.ReferenceNumber == reference));

        return reference;
    }
}

public sealed record BookingRequest(
    int AppointmentSlotId,
    string CustomerName,
    string CustomerPhone,
    string VehicleRegistration,
    string? CustomerNotes);

public sealed record BookingResult(bool Success, string Message, string? ReferenceNumber)
{
    public static BookingResult Succeeded(string referenceNumber) =>
        new(true, "Booking confirmed.", referenceNumber);

    public static BookingResult Failed(string message) =>
        new(false, message, null);
}
