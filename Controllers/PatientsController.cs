using HospitalClinicMS.DataAccess;
using HospitalClinicMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospitalClinicMS.Controllers
{
    public class PatientsController : Controller
    {
        private readonly PatientRepository _repo;

        public PatientsController(string connectionString)
        {
            _repo = new PatientRepository(connectionString);
        }

        // GET: /Patients  (also handles FR-11 search via ?term=)
        public IActionResult Index(string? term)
        {
            var patients = string.IsNullOrWhiteSpace(term) ? _repo.GetAll() : _repo.Search(term);
            ViewBag.SearchTerm = term;
            return View(patients);
        }

        public IActionResult Details(int id)
        {
            var patient = _repo.GetById(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        public IActionResult Create() => View(new Patient());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Patient patient)
        {
            if (!ModelState.IsValid) return View(patient);
            _repo.Add(patient);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var patient = _repo.GetById(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Patient patient)
        {
            if (!ModelState.IsValid) return View(patient);
            _repo.Update(patient);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var patient = _repo.GetById(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
