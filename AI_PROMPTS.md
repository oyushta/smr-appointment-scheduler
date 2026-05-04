# AI Prompt Log

## Initial prompt

User provided `AI_Coding_Interview_Assignment.pdf` and asked: "I want you to prepare the technical task written in this doc".

## Codex actions and prompts

- Extract the PDF text and identify the requirements.
- Choose a compact implementation path suitable for a three-hour interview task.
- Build a Blazor Server .NET 8 app with EF Core entities for branches, mechanics, service types, slots, bookings, and work notes.
- Add customer/booking, mechanic, and admin/shared flows.
- Add Docker Compose for SQL Server and write concise deliverable notes.

## Course corrections

Local `dotnet`, `docker`, and `git` commands were not available on PATH at first, so the project was initially prepared as source files rather than scaffolded from the SDK. After installing .NET 8, `dotnet restore` and `dotnet build --no-restore` were run successfully. App startup still requires SQL Server on `localhost,1433`.
