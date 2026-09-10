using HospitalClinicMS.Models;
using Microsoft.Data.SqlClient;

namespace HospitalClinicMS.DataAccess
{
    // Data Access Layer for Patients — all commands are parameterized (FR / NFR-Security).
    public class PatientRepository
    {
        private readonly string _connectionString;

        public PatientRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Patient> GetAll()
        {
            var patients = new List<Patient>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "SELECT PatientID, FirstName, LastName, DateOfBirth, Gender, ContactNumber, " +
                "Email, Address, BloodGroup, RegistrationDate FROM Patients ORDER BY LastName, FirstName", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                patients.Add(Map(reader));
            }
            return patients;
        }

        public Patient? GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "SELECT PatientID, FirstName, LastName, DateOfBirth, Gender, ContactNumber, " +
                "Email, Address, BloodGroup, RegistrationDate FROM Patients WHERE PatientID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        // FR-11: search by ID, name, or contact number (partial/wildcard name search)
        public List<Patient> Search(string term)
        {
            var patients = new List<Patient>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "SELECT PatientID, FirstName, LastName, DateOfBirth, Gender, ContactNumber, " +
                "Email, Address, BloodGroup, RegistrationDate FROM Patients " +
                "WHERE CAST(PatientID AS NVARCHAR(20)) = @term " +
                "   OR FirstName LIKE @wildTerm OR LastName LIKE @wildTerm " +
                "   OR ContactNumber LIKE @wildTerm " +
                "ORDER BY LastName, FirstName", conn);
            cmd.Parameters.AddWithValue("@term", term);
            cmd.Parameters.AddWithValue("@wildTerm", $"%{term}%");
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                patients.Add(Map(reader));
            }
            return patients;
        }

        public int Add(Patient p)
        {
            if (p.DateOfBirth < new DateTime(1753, 1, 1))
                throw new ArgumentException("Date of birth is missing or invalid.");
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "INSERT INTO Patients (FirstName, LastName, DateOfBirth, Gender, ContactNumber, Email, Address, BloodGroup) " +
                "OUTPUT INSERTED.PatientID " +
                "VALUES (@FirstName, @LastName, @DateOfBirth, @Gender, @ContactNumber, @Email, @Address, @BloodGroup)", conn);
            AddCommonParams(cmd, p);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public bool Update(Patient p)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "UPDATE Patients SET FirstName=@FirstName, LastName=@LastName, DateOfBirth=@DateOfBirth, " +
                "Gender=@Gender, ContactNumber=@ContactNumber, Email=@Email, Address=@Address, BloodGroup=@BloodGroup " +
                "WHERE PatientID=@PatientID", conn);
            AddCommonParams(cmd, p);
            cmd.Parameters.AddWithValue("@PatientID", p.PatientID);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("DELETE FROM Patients WHERE PatientID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        private static void AddCommonParams(SqlCommand cmd, Patient p)
        {
            cmd.Parameters.AddWithValue("@FirstName", p.FirstName);
            cmd.Parameters.AddWithValue("@LastName", p.LastName);
            cmd.Parameters.AddWithValue("@DateOfBirth", p.DateOfBirth);
            cmd.Parameters.AddWithValue("@Gender", p.Gender);
            cmd.Parameters.AddWithValue("@ContactNumber", p.ContactNumber);
            cmd.Parameters.AddWithValue("@Email", (object?)p.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", (object?)p.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BloodGroup", (object?)p.BloodGroup ?? DBNull.Value);
        }

        private static Patient Map(SqlDataReader reader) => new()
        {
            PatientID = reader.GetInt32(0),
            FirstName = reader.GetString(1),
            LastName = reader.GetString(2),
            DateOfBirth = reader.GetDateTime(3),
            Gender = reader.GetString(4),
            ContactNumber = reader.GetString(5),
            Email = reader.IsDBNull(6) ? null : reader.GetString(6),
            Address = reader.IsDBNull(7) ? null : reader.GetString(7),
            BloodGroup = reader.IsDBNull(8) ? null : reader.GetString(8),
            RegistrationDate = reader.GetDateTime(9)
        };
    }
}
