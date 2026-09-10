using HospitalClinicMS.DataAccess;
using HospitalClinicMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospitalClinicMS.Controllers
{
    public class PrescriptionsController : Controller
    {
        private readonly PrescriptionRepository _repo;
        private readonly PatientRepository _patientRepo;
        private readonly AppointmentRepository _appointmentRepo;

        public PrescriptionsController(string connectionString)
        {
            _repo = new PrescriptionRepository(connectionString);
            _patientRepo = new PatientRepository(connectionString);
            _appointmentRepo = new AppointmentRepository(connectionString);
        }

        // View/print prescription history for a patient
        public IActionResult Index(int patientId)
        {
            ViewBag.Patient = _patientRepo.GetById(patientId);
            return View(_repo.GetByPatient(patientId));
        }

        // Create a prescription linked to an appointment
        public IActionResult Create(int appointmentId)
        {
            var appointment = _appointmentRepo.GetById(appointmentId);
            if (appointment == null) return NotFound();

            return View(new Prescription
            {
                AppointmentID = appointment.AppointmentID,
                PatientID = appointment.PatientID,
                DoctorID = appointment.DoctorID
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Prescription prescription)
        {
            if (!ModelState.IsValid) return View(prescription);
            _repo.Add(prescription);
            return RedirectToAction(nameof(Index), new { patientId = prescription.PatientID });
        }
    }
}
