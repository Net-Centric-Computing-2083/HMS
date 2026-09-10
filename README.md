# Hospital / Clinic Management System

ASP.NET Core MVC + ADO.NET | Group 4

## What's in this repo

- `Database/schema.sql` — run this against SQL Server first. Creates the
  `HospitalClinicDB` database, all 6 tables, foreign keys, the unique
  constraint that prevents double-booking, indexes, and a little seed data.
- `Models/` — plain C# POCOs for the 6 entities (Patient, Doctor,
  Appointment, MedicalHistory, Prescription, Bill).
- `DataAccess/` — one repository class per entity. All SQL is parameterized
  (`SqlCommand` + `Parameters.AddWithValue`), no raw string concatenation of
  user input anywhere.
- `Controllers/` — one controller per module, matching the 7 modules in the
  SRDS (Patients, Doctors, Appointments, MedicalHistory, Prescriptions,
  Bills, plus Home).
- `Views/` — Razor views. **Patients** module has a complete, polished
  CRUD set (Index/Create/Edit/Details/Delete) meant as the template for the
  other modules' styling.
- `TASKS.md` — the 4-way task split for Group 4.

## Getting it running (Visual Studio 2022)

1. Install SQL Server Express (or use an existing instance) and run
   `Database/schema.sql` against it (SSMS or `sqlcmd`).
2. Update the connection string in `appsettings.json` to point at your
   SQL Server instance.
3. Open `HospitalClinicMS.csproj` in Visual Studio 2022.
4. Restore NuGet packages (`Microsoft.Data.SqlClient` is the only external
   dependency) — VS does this automatically on build.
5. `F5` to run. Home page is at `/`.

> This project was scaffolded in an environment without the .NET SDK
> installed, so it hasn't been compiled/run here — do a build in Visual
> Studio first thing to catch anything that needs adjusting (e.g. NuGet
> package versions, minor Razor syntax issues) before splitting up work.

## Known gaps to close (see TASKS.md for who owns what)

- `Patient.Delete` is a hard delete — will violate FK constraints once a
  patient has appointments/bills. Should become a soft-delete
  (`IsActive` flag) like `Doctor.Status`.
- Client-side validation scripts aren't wired into the Create/Edit views yet.
- No authentication/login — the SRDS marks this as optional ("if
  authentication is added"), currently not implemented.
- ER diagram not yet drawn (Member 3).
