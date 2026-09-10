using HospitalClinicMS.DataAccess;
using HospitalClinicMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospitalClinicMS.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly DoctorRepository _repo;

        public DoctorsController(string connectionString)
        {
            _repo = new DoctorRepository(connectionString);
        }

        public IActionResult Index(string? specialization)
        {
            ViewBag.SpecializationFilter = specialization;
            return View(_repo.GetAll(specialization));
        }

        public IActionResult Details(int id)
        {
            var doctor = _repo.GetById(id);
            if (doctor == null) return NotFound();
            return View(doctor);
        }

        public IActionResult Create() => View(new Doctor());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Doctor doctor)
        {
            if (!ModelState.IsValid) return View(doctor);
            _repo.Add(doctor);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var doctor = _repo.GetById(id);
            if (doctor == null) return NotFound();
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Doctor doctor)
        {
            if (!ModelState.IsValid) return View(doctor);
            _repo.Update(doctor);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deactivate(int id)
        {
            _repo.Deactivate(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
