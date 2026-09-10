using HospitalClinicMS.DataAccess;
using HospitalClinicMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospitalClinicMS.Controllers
{
    public class BillsController : Controller
    {
        private readonly BillRepository _repo;
        private readonly PatientRepository _patientRepo;

        public BillsController(string connectionString)
        {
            _repo = new BillRepository(connectionString);
            _patientRepo = new PatientRepository(connectionString);
        }

        // View outstanding/paid bills; optional per-patient filter
        public IActionResult Index(int? patientId)
        {
            if (patientId.HasValue)
            {
                ViewBag.Patient = _patientRepo.GetById(patientId.Value);
                return View(_repo.GetByPatient(patientId.Value));
            }
            return View(_repo.GetAll());
        }

        // Simple billing summary/report (module 5.6)
        public IActionResult Summary()
        {
            var (totalBilled, totalPaid, totalOutstanding) = _repo.GetSummary();
            ViewBag.TotalBilled = totalBilled;
            ViewBag.TotalPaid = totalPaid;
            ViewBag.TotalOutstanding = totalOutstanding;
            return View();
        }

        // FR-09: generate a bill linked to an appointment
        public IActionResult Create(int appointmentId, int patientId)
        {
            return View(new Bill { AppointmentID = appointmentId, PatientID = patientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Bill bill)
        {
            if (!ModelState.IsValid) return View(bill);
            _repo.Add(bill);
            return RedirectToAction(nameof(Index), new { patientId = bill.PatientID });
        }

        // FR-10: update payment status of a bill
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePaymentStatus(int id, string status, string? method)
        {
            _repo.UpdatePaymentStatus(id, status, method);
            return RedirectToAction(nameof(Index));
        }
    }
}
