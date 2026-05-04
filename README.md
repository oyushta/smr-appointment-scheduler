# SMR Appointment Scheduler

Small internal scheduling app for Service, Maintenance & Repair teams. It supports customer booking, mechanic appointment handling, and a shared daily schedule.

## How to run

Start SQL Server with `docker compose up -d`, then run the app with `dotnet run`. The app creates and seeds its schema on first startup.

## Stack choice

Blazor Server on .NET 8 with Entity Framework Core and SQL Server. I chose it to keep the interview task compact in one project while still using the requested .NET/SQL stack.

## What's done

Available slot list for the next 7 days, service and branch filtering, booking with unique references, double-booking prevention, mechanic today/tomorrow view, appointment details, timestamped work notes, status updates, and a shared home schedule.

## What's not done

Authentication, notifications, rescheduling, cancellation, recurring appointments, payments, and mobile-specific UI are intentionally out of scope. With more time I would add generated EF migrations, stronger validation, integration tests, and a fuller audit trail for status changes.

## Known rough edges

The app uses `EnsureCreatedAsync` plus seed data as the startup schema path so it is easy to run in a clean interview environment. In production I would replace that with checked-in EF migrations.

## AI tools used

Codex was used to read the brief, plan the implementation, generate the initial Blazor/EF code, and write this README. I reviewed the code structure and kept the scope to the MVP described in the assignment.

## Planning

I chose the simplest architecture that shows the required flows: EF entities for branches, mechanics, slots, bookings and work notes; one scheduling service for application operations; Blazor pages for admin, booking, mechanic list, and mechanic detail.

## Manual decisions

I deliberately kept authentication out, used a dropdown to act as a mechanic, made slot booking unique at the database level, and used seeded slots rather than a slot-generation UI.
