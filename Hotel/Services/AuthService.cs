using Hotel.Models;

namespace Hotel.Services
{
    public class AuthService
    {
        private readonly Database db;

        public AuthService(Database _db) => db = _db;

        public async Task<User?> Login(string email, string password)
        {
            using var conn = db.GetConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Users WHERE Email = @Email AND Password = @Password LIMIT 1";

            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"]?.ToString(),
                    Email = reader["Email"]?.ToString(),
                    RoleId = Convert.ToInt32(reader["RoleId"])
                };
            }

            return null;
        }
    }
}