using System.ComponentModel.DataAnnotations;

namespace HospitalClinicMS.Models
{
    public class Doctor
    {
        public int DoctorID { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Specialization { get; set; } = string.Empty;

        [Required, StringLength(15)]
        public string ContactNumber { get; set; } = string.Empty;

        [StringLength(100), EmailAddress]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? AvailableDays { get; set; }

        [Required, Range(0, 100000)]
        public decimal ConsultationFee { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Active";
    }
}
