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

Local `dotnet`, `docker`, and `git` commands were not available on PATH in this environment, so the project was prepared as source files rather than scaffolded and built from the SDK.
