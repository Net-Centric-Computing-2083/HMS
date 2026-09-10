using System.ComponentModel.DataAnnotations;

namespace HospitalClinicMS.Models
{
    public class MedicalHistory
    {
        public int HistoryID { get; set; }

        [Required]
        public int PatientID { get; set; }

        [Required]
        public int DoctorID { get; set; }

        public int? AppointmentID { get; set; }

        [Required, DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/2000", "12/31/2100", ErrorMessage = "Please select a valid visit date.")]
        public DateTime VisitDate { get; set; }

        [StringLength(300)]
        public string? Diagnosis { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
    }
}
