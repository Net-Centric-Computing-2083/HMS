using HospitalClinicMS.Models;

namespace HospitalClinicMS.Models
{
    // Not a database entity — just packages the numbers/lists the
    // dashboard needs to display, gathered from several repositories.
    public class DashboardViewModel
    {
        public int TotalPatients { get; set; }
        public int ActiveDoctors { get; set; }
        public List<Appointment> TodaysAppointments { get; set; } = new();
        public int UnpaidBillsCount { get; set; }
        public decimal TotalOutstanding { get; set; }
    }
}