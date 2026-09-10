using HospitalClinicMS.Models;
using Microsoft.Data.SqlClient;

namespace HospitalClinicMS.DataAccess
{
    public class BillRepository
    {
        private readonly string _connectionString;

        public BillRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private const string BaseSelect =
            "SELECT b.BillID, b.PatientID, b.AppointmentID, b.Amount, b.PaymentStatus, b.PaymentMethod, b.BillDate, " +
            "p.FirstName + ' ' + p.LastName AS PatientName " +
            "FROM Bills b " +
            "JOIN Patients p ON b.PatientID = p.PatientID ";

        public List<Bill> GetAll()
        {
            var list = new List<Bill>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(BaseSelect + "ORDER BY b.BillDate DESC", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(Map(reader));
            return list;
        }

        // FR: view outstanding/paid bills per patient
        public List<Bill> GetByPatient(int patientId)
        {
            var list = new List<Bill>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(BaseSelect + "WHERE b.PatientID = @patientId ORDER BY b.BillDate DESC", conn);
            cmd.Parameters.AddWithValue("@patientId", patientId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(Map(reader));
            return list;
        }

        public Bill? GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(BaseSelect + "WHERE b.BillID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        // FR-09: generate a bill linked to an appointment
        public int Add(Bill b)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "INSERT INTO Bills (PatientID, AppointmentID, Amount, PaymentStatus, PaymentMethod) " +
                "OUTPUT INSERTED.BillID " +
                "VALUES (@PatientID, @AppointmentID, @Amount, @PaymentStatus, @PaymentMethod)", conn);
            cmd.Parameters.AddWithValue("@PatientID", b.PatientID);
            cmd.Parameters.AddWithValue("@AppointmentID", b.AppointmentID);
            cmd.Parameters.AddWithValue("@Amount", b.Amount);
            cmd.Parameters.AddWithValue("@PaymentStatus", b.PaymentStatus);
            cmd.Parameters.AddWithValue("@PaymentMethod", (object?)b.PaymentMethod ?? DBNull.Value);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        // FR-10: update payment status of a bill
        public bool UpdatePaymentStatus(int id, string status, string? method)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "UPDATE Bills SET PaymentStatus = @status, PaymentMethod = @method WHERE BillID = @id", conn);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@method", (object?)method ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        // Simple billing summary/report (5.6)
        public (decimal totalBilled, decimal totalPaid, decimal totalOutstanding) GetSummary()
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "SELECT ISNULL(SUM(Amount),0), " +
                "ISNULL(SUM(CASE WHEN PaymentStatus = 'Paid' THEN Amount ELSE 0 END),0), " +
                "ISNULL(SUM(CASE WHEN PaymentStatus <> 'Paid' THEN Amount ELSE 0 END),0) " +
                "FROM Bills", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            reader.Read();
            return (reader.GetDecimal(0), reader.GetDecimal(1), reader.GetDecimal(2));
        }

        private static Bill Map(SqlDataReader reader) => new()
        {
            BillID = reader.GetInt32(0),
            PatientID = reader.GetInt32(1),
            AppointmentID = reader.GetInt32(2),
            Amount = reader.GetDecimal(3),
            PaymentStatus = reader.GetString(4),
            PaymentMethod = reader.IsDBNull(5) ? null : reader.GetString(5),
            BillDate = reader.GetDateTime(6),
            PatientName = reader.GetString(7)
        };
    }
}
