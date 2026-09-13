using HospitalClinicMS.DataAccess;
using HospitalClinicMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospitalClinicMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly PatientRepository _patientRepo;
        private readonly DoctorRepository _doctorRepo;
        private readonly AppointmentRepository _appointmentRepo;
        private readonly BillRepository _billRepo;

        public HomeController(string connectionString)
        {
            _patientRepo = new PatientRepository(connectionString);
            _doctorRepo = new DoctorRepository(connectionString);
            _appointmentRepo = new AppointmentRepository(connectionString);
            _billRepo = new BillRepository(connectionString);
        }

        // Pulls a small snapshot from each module so the dashboard reflects
        // real, current data rather than static placeholder numbers.
        public IActionResult Index()
        {
            var (_, _, totalOutstanding) = _billRepo.GetSummary();
            var allBills = _billRepo.GetAll();

            var dashboard = new DashboardViewModel
            {
                TotalPatients = _patientRepo.GetAll().Count,
                ActiveDoctors = _doctorRepo.GetAll().Count(d => d.Status == "Active"),
                TodaysAppointments = _appointmentRepo.GetByDate(DateTime.Today),
                UnpaidBillsCount = allBills.Count(b => b.PaymentStatus != "Paid"),
                TotalOutstanding = totalOutstanding
            };

            return View(dashboard);
        }

        public IActionResult Error() => View();
    }
}