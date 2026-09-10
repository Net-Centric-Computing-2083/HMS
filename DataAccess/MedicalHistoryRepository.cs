using HospitalClinicMS.Models;
using Microsoft.Data.SqlClient;

namespace HospitalClinicMS.DataAccess
{
    public class MedicalHistoryRepository
    {
        private readonly string _connectionString;

        public MedicalHistoryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private const string BaseSelect =
            "SELECT h.HistoryID, h.PatientID, h.DoctorID, h.AppointmentID, h.VisitDate, h.Diagnosis, h.Notes, " +
            "p.FirstName + ' ' + p.LastName AS PatientName, d.FullName AS DoctorName " +
            "FROM MedicalHistory h " +
            "JOIN Patients p ON h.PatientID = p.PatientID " +
            "JOIN Doctors d ON h.DoctorID = d.DoctorID ";

        // FR-07: full chronological history for a patient
        public List<MedicalHistory> GetByPatient(int patientId)
        {
            var list = new List<MedicalHistory>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(BaseSelect + "WHERE h.PatientID = @patientId ORDER BY h.VisitDate DESC", conn);
            cmd.Parameters.AddWithValue("@patientId", patientId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(Map(reader));
            return list;
        }

        public MedicalHistory? GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(BaseSelect + "WHERE h.HistoryID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        // FR-06: record a diagnosis/notes entry for a visit
        public int Add(MedicalHistory h)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "INSERT INTO MedicalHistory (PatientID, DoctorID, AppointmentID, VisitDate, Diagnosis, Notes) " +
                "OUTPUT INSERTED.HistoryID " +
                "VALUES (@PatientID, @DoctorID, @AppointmentID, @VisitDate, @Diagnosis, @Notes)", conn);
            cmd.Parameters.AddWithValue("@PatientID", h.PatientID);
            cmd.Parameters.AddWithValue("@DoctorID", h.DoctorID);
            cmd.Parameters.AddWithValue("@AppointmentID", (object?)h.AppointmentID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@VisitDate", h.VisitDate.Date);
            cmd.Parameters.AddWithValue("@Diagnosis", (object?)h.Diagnosis ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", (object?)h.Notes ?? DBNull.Value);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        private static MedicalHistory Map(SqlDataReader reader) => new()
        {
            HistoryID = reader.GetInt32(0),
            PatientID = reader.GetInt32(1),
            DoctorID = reader.GetInt32(2),
            AppointmentID = reader.IsDBNull(3) ? null : reader.GetInt32(3),
            VisitDate = reader.GetDateTime(4),
            Diagnosis = reader.IsDBNull(5) ? null : reader.GetString(5),
            Notes = reader.IsDBNull(6) ? null : reader.GetString(6),
            PatientName = reader.GetString(7),
            DoctorName = reader.GetString(8)
        };
    }
}
