using HospitalClinicMS.Models;
using Microsoft.Data.SqlClient;

namespace HospitalClinicMS.DataAccess
{
    public class AppointmentRepository
    {
        private readonly string _connectionString;

        public AppointmentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private const string BaseSelect =
            "SELECT a.AppointmentID, a.PatientID, a.DoctorID, a.AppointmentDate, a.AppointmentTime, " +
            "a.Reason, a.Status, a.CreatedDate, " +
            "p.FirstName + ' ' + p.LastName AS PatientName, d.FullName AS DoctorName " +
            "FROM Appointments a " +
            "JOIN Patients p ON a.PatientID = p.PatientID " +
            "JOIN Doctors d ON a.DoctorID = d.DoctorID ";

        public List<Appointment> GetAll()
        {
            var list = new List<Appointment>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(BaseSelect + "ORDER BY a.AppointmentDate DESC, a.AppointmentTime", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(Map(reader));
            return list;
        }

        public List<Appointment> GetByDate(DateTime date)
        {
            var list = new List<Appointment>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(BaseSelect + "WHERE a.AppointmentDate = @date ORDER BY a.AppointmentTime", conn);
            cmd.Parameters.AddWithValue("@date", date.Date);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(Map(reader));
            return list;
        }
        
        public List<Appointment> GetCompletedUnbilled()
        {
            var list = new List<Appointment>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                BaseSelect +
                "WHERE a.Status = 'Completed' " +
                "AND NOT EXISTS (SELECT 1 FROM Bills b WHERE b.AppointmentID = a.AppointmentID) " +
                "ORDER BY a.AppointmentDate DESC", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(Map(reader));
            return list;
        }

        public List<Appointment> GetByDoctor(int doctorId)
        {
            var list = new List<Appointment>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(BaseSelect + "WHERE a.DoctorID = @doctorId ORDER BY a.AppointmentDate DESC", conn);
            cmd.Parameters.AddWithValue("@doctorId", doctorId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(Map(reader));
            return list;
        }

        public List<Appointment> GetByPatient(int patientId)
        {
            var list = new List<Appointment>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(BaseSelect + "WHERE a.PatientID = @patientId ORDER BY a.AppointmentDate DESC", conn);
            cmd.Parameters.AddWithValue("@patientId", patientId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(Map(reader));
            return list;
        }

        public Appointment? GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(BaseSelect + "WHERE a.AppointmentID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        // FR-04: prevent double-booking of the same doctor/time slot
        public bool IsSlotTaken(int doctorId, DateTime date, TimeSpan time, int? excludeAppointmentId = null)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "SELECT COUNT(*) FROM Appointments WHERE DoctorID = @doctorId AND AppointmentDate = @date " +
                      "AND AppointmentTime = @time AND Status <> 'Cancelled'";
            if (excludeAppointmentId.HasValue) sql += " AND AppointmentID <> @excludeId";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@doctorId", doctorId);
            cmd.Parameters.AddWithValue("@date", date.Date);
            cmd.Parameters.AddWithValue("@time", time);
            if (excludeAppointmentId.HasValue) cmd.Parameters.AddWithValue("@excludeId", excludeAppointmentId.Value);

            conn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }

        public int Add(Appointment a)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "INSERT INTO Appointments (PatientID, DoctorID, AppointmentDate, AppointmentTime, Reason, Status) " +
                "OUTPUT INSERTED.AppointmentID " +
                "VALUES (@PatientID, @DoctorID, @AppointmentDate, @AppointmentTime, @Reason, @Status)", conn);
            AddCommonParams(cmd, a);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public bool Update(Appointment a)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "UPDATE Appointments SET PatientID=@PatientID, DoctorID=@DoctorID, AppointmentDate=@AppointmentDate, " +
                "AppointmentTime=@AppointmentTime, Reason=@Reason, Status=@Status WHERE AppointmentID=@AppointmentID", conn);
            AddCommonParams(cmd, a);
            cmd.Parameters.AddWithValue("@AppointmentID", a.AppointmentID);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool UpdateStatus(int id, string status)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("UPDATE Appointments SET Status = @status WHERE AppointmentID = @id", conn);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        private static void AddCommonParams(SqlCommand cmd, Appointment a)
        {
            cmd.Parameters.AddWithValue("@PatientID", a.PatientID);
            cmd.Parameters.AddWithValue("@DoctorID", a.DoctorID);
            cmd.Parameters.AddWithValue("@AppointmentDate", a.AppointmentDate.Date);
            cmd.Parameters.AddWithValue("@AppointmentTime", a.AppointmentTime);
            cmd.Parameters.AddWithValue("@Reason", (object?)a.Reason ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", a.Status);
        }

        private static Appointment Map(SqlDataReader reader) => new()
        {
            AppointmentID = reader.GetInt32(0),
            PatientID = reader.GetInt32(1),
            DoctorID = reader.GetInt32(2),
            AppointmentDate = reader.GetDateTime(3),
            AppointmentTime = reader.GetTimeSpan(4),
            Reason = reader.IsDBNull(5) ? null : reader.GetString(5),
            Status = reader.GetString(6),
            CreatedDate = reader.GetDateTime(7),
            PatientName = reader.GetString(8),
            DoctorName = reader.GetString(9)
        };
    }
}
