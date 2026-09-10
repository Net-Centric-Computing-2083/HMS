# Group 4 — Task Breakdown
Hospital/Clinic Management System | ASP.NET Core MVC + ADO.NET

The skeleton (models, repositories, controllers, and views for **Patients**
as a fully worked example) is already in this repo. Each member's job is to
extend the same pattern to their assigned modules — copy the Patients
files and adapt them.

---

## Member 1 — Team Lead / Backend (Data Access Layer)
**Owns:** `/DataAccess/*`, connection/config wiring, GitHub repo setup

- [x] `PatientRepository.cs` and `DoctorRepository.cs` are done — use these as the reference pattern.
- [ ] Finish wiring dependency injection in `Program.cs` so controllers receive `string connectionString` cleanly (currently a bare singleton — consider a small `IDbConnectionFactory` wrapper if the team wants it cleaner).
- [ ] Review `AppointmentRepository.cs`'s `IsSlotTaken` method and confirm it matches how the team wants double-booking handled (currently: same doctor + same date + same time = blocked).
- [ ] Set up the GitHub repository, `.gitignore` for a .NET project (`bin/`, `obj/`, `.vs/`), and branch protection / PR flow for the other 3 members.
- [ ] Run `Database/schema.sql` against a local SQL Server / SQL Express instance and confirm the app connects (update `appsettings.json` connection string to match each dev machine).

## Member 2 — Frontend / Views
**Owns:** `/Views/*`, `/wwwroot/css/site.css`, shared layout

- [x] `_Layout.cshtml`, `_ViewImports.cshtml`, `Home/Index.cshtml`, and the full **Patients** CRUD views (Index/Create/Edit/Details/Delete) are done — this is the pattern to copy.
- [ ] Polish `Doctors`, `Appointments`, `MedicalHistory`, `Prescriptions`, `Bills` views (already scaffolded functionally — needs visual polish, consistent spacing, responsive check on mobile widths).
- [ ] Add client-side validation feedback (jQuery Validation is pulled in automatically by Data Annotations if you add `_ValidationScriptsPartial.cshtml` — currently omitted, add it to each Create/Edit view's `@section Scripts`).
- [ ] Style the navbar and dashboard cards on `Home/Index.cshtml` beyond the current Bootstrap defaults.
- [ ] Add a simple "no results" empty-state message on `Patients/Index.cshtml` when a search returns nothing.

## Member 3 — Database Designer
**Owns:** `Database/schema.sql`, ER diagram, data integrity

- [x] `schema.sql` is done — 6 tables, FKs, the `UQ_Doctor_DateTime` unique constraint enforcing FR-04, indexes for search, and sample seed data.
- [ ] Draw the ER diagram (dbdiagram.io, Lucidchart, or SSMS's built-in diagrammer) from this schema for the documentation appendix.
- [ ] Decide & document a soft-delete strategy for Patients (spec says "deactivate/remove" — currently `PatientRepository.Delete()` does a hard `DELETE`, which will fail once a patient has appointments/bills due to FK constraints; consider adding an `IsActive` column instead, mirroring the `Doctors.Status` pattern).
- [ ] Add any missing constraints the team agrees on (e.g. `CHECK` constraint on `Gender`, `PaymentStatus` enums).
- [ ] Verify indexes are sufficient once real data volumes are tested (NFR: search/list under 2 seconds).

## Member 4 — QA / Documentation
**Owns:** Testing, this documentation, GitHub upload polish

- [ ] Write a test pass for each FR-01 through FR-11 (see the original SRDS doc, Section 7) — manual click-through checklist is fine for a course project.
- [ ] Specifically test FR-04 (double-booking prevention) and FR-11 (search by ID/name/contact, including partial match) since those have the trickiest logic.
- [ ] Fill in the placeholders in the original documentation: date, team member names/roles table (Section 11), and confirm the "future enhancements" list is still accurate.
- [ ] Take screenshots of each working module for the deliverables checklist (Section 10.1).
- [ ] Write the README's setup section once Member 1 confirms the local dev connection string works, and do the final GitHub upload with a descriptive commit history.

---

## Suggested order of work (maps to the SRDS's 10 phases)
1. Member 3 finalizes schema + Member 1 confirms it runs locally (Phases 1–3, already scaffolded here).
2. Member 1 finishes Doctor repository review; Member 2 polishes Doctor views. *(Phase 6 — Patients already done as Phase 5.)*
3. Member 1 + Member 2 pair on Appointments (conflict-checking logic is already wired — needs testing).
4. Split Prescriptions + Billing across Members 1 & 2; Member 4 starts writing FR test cases in parallel.
5. Patient Search — already implemented in `Patients/Index` via `?term=`; Member 4 verifies edge cases (partial match, numeric ID, phone number).
6. Member 4 leads final testing pass + GitHub upload (Phase 10).
