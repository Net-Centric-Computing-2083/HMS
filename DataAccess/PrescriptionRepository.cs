using HospitalClinicMS.Models;
using Microsoft.Data.SqlClient;

namespace HospitalClinicMS.DataAccess
{
    public class PrescriptionRepository
    {
        private readonly string _connectionString;

        public PrescriptionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private const string BaseSelect =
            "SELECT pr.PrescriptionID, pr.AppointmentID, pr.PatientID, pr.DoctorID, pr.Medicines, pr.Dosage, pr.DateIssued, " +
            "p.FirstName + ' ' + p.LastName AS PatientName, d.FullName AS DoctorName " +
            "FROM Prescriptions pr " +
            "JOIN Patients p ON pr.PatientID = p.PatientID " +
            "JOIN Doctors d ON pr.DoctorID = d.DoctorID ";

        // FR-08 / view+print prescription history for a patient
        public List<Prescription> GetByPatient(int patientId)
        {
            var list = new List<Prescription>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(BaseSelect + "WHERE pr.PatientID = @patientId ORDER BY pr.DateIssued DESC", conn);
            cmd.Parameters.AddWithValue("@patientId", patientId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(Map(reader));
            return list;
        }

        public Prescription? GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(BaseSelect + "WHERE pr.PrescriptionID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public int Add(Prescription p)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "INSERT INTO Prescriptions (AppointmentID, PatientID, DoctorID, Medicines, Dosage) " +
                "OUTPUT INSERTED.PrescriptionID " +
                "VALUES (@AppointmentID, @PatientID, @DoctorID, @Medicines, @Dosage)", conn);
            cmd.Parameters.AddWithValue("@AppointmentID", p.AppointmentID);
            cmd.Parameters.AddWithValue("@PatientID", p.PatientID);
            cmd.Parameters.AddWithValue("@DoctorID", p.DoctorID);
            cmd.Parameters.AddWithValue("@Medicines", p.Medicines);
            cmd.Parameters.AddWithValue("@Dosage", (object?)p.Dosage ?? DBNull.Value);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        private static Prescription Map(SqlDataReader reader) => new()
        {
            PrescriptionID = reader.GetInt32(0),
            AppointmentID = reader.GetInt32(1),
            PatientID = reader.GetInt32(2),
            DoctorID = reader.GetInt32(3),
            Medicines = reader.GetString(4),
            Dosage = reader.IsDBNull(5) ? null : reader.GetString(5),
            DateIssued = reader.GetDateTime(6),
            PatientName = reader.GetString(7),
            DoctorName = reader.GetString(8)
        };
    }
}
