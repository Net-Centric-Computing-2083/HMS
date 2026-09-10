using System.ComponentModel.DataAnnotations;

namespace HospitalClinicMS.Models
{
    public class Appointment
    {
        public int AppointmentID { get; set; }

        [Required]
        public int PatientID { get; set; }

        [Required]
        public int DoctorID { get; set; }

        [Required, DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/2000", "12/31/2100", ErrorMessage = "Please select a valid appointment date.")]
        public DateTime AppointmentDate { get; set; }

        [Required, DataType(DataType.Time)]
        public TimeSpan AppointmentTime { get; set; }

        [StringLength(200)]
        public string? Reason { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Scheduled"; // Scheduled / Completed / Cancelled

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Populated by joins for display purposes only, not persisted directly
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
    }
}
