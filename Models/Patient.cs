using System.ComponentModel.DataAnnotations;

namespace HospitalClinicMS.Models
{
    public class Patient
    {
        public int PatientID { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/1900", "12/31/2100", ErrorMessage = "Please enter a valid date of birth.")]
        public DateTime DateOfBirth { get; set; }

        [Required, StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [Required, StringLength(15)]
        public string ContactNumber { get; set; } = string.Empty;

        [StringLength(100), EmailAddress]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(5)]
        public string? BloodGroup { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        // Convenience, not persisted
        public string FullName => $"{FirstName} {LastName}";
    }
}
