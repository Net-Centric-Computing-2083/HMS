using HospitalClinicMS.DataAccess;
using HospitalClinicMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospitalClinicMS.Controllers
{
    public class MedicalHistoryController : Controller
    {
        private readonly MedicalHistoryRepository _repo;
        private readonly PatientRepository _patientRepo;
        private readonly DoctorRepository _doctorRepo;

        public MedicalHistoryController(string connectionString)
        {
            _repo = new MedicalHistoryRepository(connectionString);
            _patientRepo = new PatientRepository(connectionString);
            _doctorRepo = new DoctorRepository(connectionString);
        }

        // FR-07: full medical history of a selected patient
        public IActionResult Index(int patientId)
        {
            ViewBag.Patient = _patientRepo.GetById(patientId);
            return View(_repo.GetByPatient(patientId));
        }

        public IActionResult Create(int patientId)
        {
            ViewBag.Doctors = _doctorRepo.GetAll();
            return View(new MedicalHistory { PatientID = patientId, VisitDate = DateTime.Today });
        }

        // FR-06: record a diagnosis/notes entry for a visit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MedicalHistory history)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Doctors = _doctorRepo.GetAll();
                return View(history);
            }
            _repo.Add(history);
            return RedirectToAction(nameof(Index), new { patientId = history.PatientID });
        }
    }
}
