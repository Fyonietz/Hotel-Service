using Hotel.Models;
using Dapper;

namespace Hotel.Services
{
        public class UserService
        {
            private readonly Database _db;
            public UserService(Database db) => _db = db;

            public async Task<IEnumerable<User>> GetByRole(int roleId)
            {
                using var conn = _db.GetConnection();
                string sql = @"SELECT u.*, r.Name as RoleName FROM Users u JOIN Roles r ON u.RoleId = r.Id WHERE u.RoleId = @RoleId";
                return await conn.QueryAsync<User>(sql, new { RoleId = roleId });
            }

            public async Task<bool> Create(User user)
            {
                using var conn = _db.GetConnection();
                string sql = "INSERT INTO Users (Name, Password, Email, RoleId) VALUES (@Name, @Password, @Email, @RoleId)";
                return await conn.ExecuteAsync(sql, user) > 0;
            }

            public async Task<bool> Delete(int id)
            {
                using var conn = _db.GetConnection();
                return await conn.ExecuteAsync("DELETE FROM Users WHERE Id = @Id", new { Id = id }) > 0;
            }

            public async Task<IEnumerable<User>> GetAll()
            {
                using var conn = _db.GetConnection();
                string sql = @"SELECT u.*, r.Name as RoleName 
                   FROM Users u 
                   JOIN Roles r ON u.RoleId = r.Id";
                return await conn.QueryAsync<User>(sql);
            }

        public async Task<bool> Update(int id, User updatedUser)
        {
            using var conn = _db.GetConnection();

            string sql = @"
        UPDATE Users
        SET Name = @Name,
            Password = @Password,
            Email = @Email,
            RoleId = @RoleId
        WHERE Id = @Id";

            var result = await conn.ExecuteAsync(sql, new
            {
                Id = id,
                updatedUser.Name,
                updatedUser.Password,
                updatedUser.Email,
                updatedUser.RoleId
            });

            return result > 0;
        }
    }
    }
