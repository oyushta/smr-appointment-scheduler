# SMR Scheduler Project Reference

## Repository

GitHub repo: https://github.com/oyushta/smr-appointment-scheduler

Local folder:

```text
C:\Users\oyaal\Documents\Codex\2026-05-04\files-mentioned-by-the-user-ai
```

## What Was Built

Blazor Server .NET 8 application for the SMR Appointment Scheduler technical task.

Main features:

- Home page showing today's schedule across mechanics.
- Booking page with available slots for the next 7 days.
- Filters by service type and branch.
- Booking form with customer name, phone, vehicle registration, service slot, and notes.
- Unique booking reference number.
- Double-booking prevention using a unique database index on `Booking.AppointmentSlotId`.
- Mechanic view with "act as" dropdown.
- Mechanic appointments for today and tomorrow.
- Appointment detail page with customer, vehicle, notes, status updates, and timestamped work notes.
- SQL Server database with EF Core and startup seed data.

## Key Files

- `Program.cs` - app startup, EF setup, database creation and seeding.
- `Data/Models.cs` - EF entities and appointment status enum.
- `Data/SmrDbContext.cs` - indexes and relationship configuration.
- `Data/SeedData.cs` - seed branches, mechanics, service types, slots, and sample booking.
- `Services/SchedulingService.cs` - application operations for booking, schedules, notes, and status updates.
- `Components/Pages/Home.razor` - shared daily schedule.
- `Components/Pages/Book.razor` - customer booking flow.
- `Components/Pages/Mechanic.razor` - mechanic list view.
- `Components/Pages/AppointmentDetails.razor` - mechanic appointment detail workflow.
- `docker-compose.yml` - SQL Server container.
- `README.md` - submission README.
- `AI_PROMPTS.md` - AI usage notes.

## How To Run

Open PowerShell:

```powershell
cd C:\Users\oyaal\Documents\Codex\2026-05-04\files-mentioned-by-the-user-ai
& "C:\Program Files\Docker\Docker\resources\bin\docker.exe" compose up -d
& "C:\Program Files\dotnet\dotnet.exe" run --urls http://localhost:5077
```

Then open:

```text
http://localhost:5077
```

If `dotnet` and `docker` are on PATH, shorter commands also work:

```powershell
docker compose up -d
dotnet run
```

## Verification Completed

Verified locally:

```text
dotnet restore: passed
dotnet build --no-restore: passed, 0 warnings, 0 errors
Docker SQL Server: running
App startup: passed
Homepage response: HTTP 200
```

Important fix made during verification:

- SQL Server rejected multiple cascade delete paths in the EF model.
- Fixed by setting `AppointmentSlot` relationships to `DeleteBehavior.NoAction` in `Data/SmrDbContext.cs`.

## Suggested Submission Email

Subject:

```text
SMR Appointment Scheduler Technical Task
```

Body:

```text
Hi,

Please find my completed technical task here:

https://github.com/oyushta/smr-appointment-scheduler

I used Codex as my AI coding assistant and included notes about the process in the README and AI_PROMPTS.md files.

I have also run the project locally with Docker SQL Server and verified that restore/build pass and the app starts successfully.

Best regards,
Oya
```

## Interview Prep

Be ready to show:

- `/` - today's schedule.
- `/book` - booking flow.
- `/mechanic` - mechanic workflow.
- Appointment detail page - status changes and work notes.

Likely questions:

- Why Blazor Server?
- How does double-booking prevention work?
- How does seed data work?
- Why use `EnsureCreatedAsync` instead of migrations?
- What would you improve with more time?
- How did you use AI?

Good talking points:

- Blazor Server kept the MVP compact in one .NET 8 project.
- Double-booking is protected both in app logic and at the database level with a unique index.
- Seed data makes the application usable immediately after first run.
- `EnsureCreatedAsync` was chosen for a quick interview MVP; EF migrations would be better for production.
- Codex helped read the brief, plan scope, generate the first implementation, fix build/runtime issues, and document the work.
- You reviewed AI output by running restore, build, Docker SQL Server, and app startup locally.

Possible live-change examples:

- Add another appointment status.
- Add branch/mechanic filters.
- Add a booking field.
- Change the mechanic query from 2 days to 3 days.
- Improve validation.

Suggested live-change approach:

```text
I will find where the data is queried, update the model/service/UI as needed, then run the build and test the changed flow.
```

## Known Tradeoffs

- No authentication; the mechanic page uses an "act as" dropdown.
- No email/SMS notifications.
- No rescheduling/cancellation flow.
- No payments/invoicing.
- Uses startup schema creation rather than checked-in EF migrations.
- No automated tests yet.

