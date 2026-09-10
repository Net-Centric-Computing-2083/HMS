using System.ComponentModel.DataAnnotations;

namespace HospitalClinicMS.Models
{
    public class Bill
    {
        public int BillID { get; set; }

        [Required]
        public int PatientID { get; set; }

        [Required]
        public int AppointmentID { get; set; }

        [Required, Range(0, 1000000)]
        public decimal Amount { get; set; }

        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Unpaid"; // Paid / Unpaid / Partially Paid

        [StringLength(30)]
        public string? PaymentMethod { get; set; }

        public DateTime BillDate { get; set; } = DateTime.Now;

        public string? PatientName { get; set; }
    }
}
