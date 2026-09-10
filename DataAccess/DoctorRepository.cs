using HospitalClinicMS.Models;
using Microsoft.Data.SqlClient;

namespace HospitalClinicMS.DataAccess
{
    public class DoctorRepository
    {
        private readonly string _connectionString;

        public DoctorRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Doctor> GetAll(string? specializationFilter = null)
        {
            var doctors = new List<Doctor>();
            using var conn = new SqlConnection(_connectionString);
            var sql = "SELECT DoctorID, FullName, Specialization, ContactNumber, Email, AvailableDays, ConsultationFee, Status FROM Doctors";
            if (!string.IsNullOrWhiteSpace(specializationFilter))
                sql += " WHERE Specialization = @spec";
            sql += " ORDER BY FullName";

            using var cmd = new SqlCommand(sql, conn);
            if (!string.IsNullOrWhiteSpace(specializationFilter))
                cmd.Parameters.AddWithValue("@spec", specializationFilter);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) doctors.Add(Map(reader));
            return doctors;
        }

        public Doctor? GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "SELECT DoctorID, FullName, Specialization, ContactNumber, Email, AvailableDays, ConsultationFee, Status " +
                "FROM Doctors WHERE DoctorID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public int Add(Doctor d)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "INSERT INTO Doctors (FullName, Specialization, ContactNumber, Email, AvailableDays, ConsultationFee, Status) " +
                "OUTPUT INSERTED.DoctorID " +
                "VALUES (@FullName, @Specialization, @ContactNumber, @Email, @AvailableDays, @ConsultationFee, @Status)", conn);
            AddCommonParams(cmd, d);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public bool Update(Doctor d)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "UPDATE Doctors SET FullName=@FullName, Specialization=@Specialization, ContactNumber=@ContactNumber, " +
                "Email=@Email, AvailableDays=@AvailableDays, ConsultationFee=@ConsultationFee, Status=@Status " +
                "WHERE DoctorID=@DoctorID", conn);
            AddCommonParams(cmd, d);
            cmd.Parameters.AddWithValue("@DoctorID", d.DoctorID);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        // Deactivate rather than hard-delete, per module spec (5.2)
        public bool Deactivate(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("UPDATE Doctors SET Status = 'Inactive' WHERE DoctorID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        private static void AddCommonParams(SqlCommand cmd, Doctor d)
        {
            cmd.Parameters.AddWithValue("@FullName", d.FullName);
            cmd.Parameters.AddWithValue("@Specialization", d.Specialization);
            cmd.Parameters.AddWithValue("@ContactNumber", d.ContactNumber);
            cmd.Parameters.AddWithValue("@Email", (object?)d.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AvailableDays", (object?)d.AvailableDays ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ConsultationFee", d.ConsultationFee);
            cmd.Parameters.AddWithValue("@Status", d.Status);
        }

        private static Doctor Map(SqlDataReader reader) => new()
        {
            DoctorID = reader.GetInt32(0),
            FullName = reader.GetString(1),
            Specialization = reader.GetString(2),
            ContactNumber = reader.GetString(3),
            Email = reader.IsDBNull(4) ? null : reader.GetString(4),
            AvailableDays = reader.IsDBNull(5) ? null : reader.GetString(5),
            ConsultationFee = reader.GetDecimal(6),
            Status = reader.GetString(7)
        };
    }
}
