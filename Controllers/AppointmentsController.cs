using HospitalClinicMS.DataAccess;
using HospitalClinicMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospitalClinicMS.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly AppointmentRepository _repo;
        private readonly PatientRepository _patientRepo;
        private readonly DoctorRepository _doctorRepo;

        public AppointmentsController(string connectionString)
        {
            _repo = new AppointmentRepository(connectionString);
            _patientRepo = new PatientRepository(connectionString);
            _doctorRepo = new DoctorRepository(connectionString);
        }

        // View appointments by date, doctor, or patient (module 5.3)
        public IActionResult Index(DateTime? date, int? doctorId, int? patientId)
        {
            List<Appointment> appointments;
            if (doctorId.HasValue) appointments = _repo.GetByDoctor(doctorId.Value);
            else if (patientId.HasValue) appointments = _repo.GetByPatient(patientId.Value);
            else if (date.HasValue) appointments = _repo.GetByDate(date.Value);
            else appointments = _repo.GetAll();

            return View(appointments);
        }

        public IActionResult Create()
        {
            PopulateDropdowns();
            return View(new Appointment { AppointmentDate = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Appointment appointment)
        {
            // FR-04: prevent double-booking of the same doctor/time slot
            if (_repo.IsSlotTaken(appointment.DoctorID, appointment.AppointmentDate, appointment.AppointmentTime))
            {
                ModelState.AddModelError("", "This doctor already has an appointment at that date and time.");
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(appointment);
            }

            _repo.Add(appointment);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var appointment = _repo.GetById(id);
            if (appointment == null) return NotFound();
            PopulateDropdowns();
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Appointment appointment)
        {
            if (_repo.IsSlotTaken(appointment.DoctorID, appointment.AppointmentDate, appointment.AppointmentTime, appointment.AppointmentID))
            {
                ModelState.AddModelError("", "This doctor already has an appointment at that date and time.");
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(appointment);
            }

            _repo.Update(appointment);
            return RedirectToAction(nameof(Index));
        }

        // Update appointment status (Scheduled / Completed / Cancelled)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, string status)
        {
            _repo.UpdateStatus(id, status);
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns()
        {
            ViewBag.Patients = _patientRepo.GetAll();
            ViewBag.Doctors = _doctorRepo.GetAll();
        }
    }
}
