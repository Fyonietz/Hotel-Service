using Hotel.Models;
using Dapper;

namespace Hotel.Services
{
    public class TypeService
    {
        private readonly Database _db;

        public TypeService(Database db) => _db = db;

        public async Task<IEnumerable<Types>> GetAll()
        {
            using var conn = _db.GetConnection();
            string sql = "SELECT * FROM Types";
            return await conn.QueryAsync<Types>(sql);
        }

        public async Task<Types?> GetById(int id)
        {
            using var conn = _db.GetConnection();
            string sql = "SELECT * FROM Types WHERE Id = @Id";
            return await conn.QueryFirstOrDefaultAsync<Types>(sql, new { Id = id });
        }

        public async Task<bool> Create(Types type)
        {
            using var conn = _db.GetConnection();
            string sql = @"INSERT INTO Types (Name) VALUES (@Name)";
            return await conn.ExecuteAsync(sql, type) > 0;
        }

        public async Task<bool> Delete(int id)
        {
            using var conn = _db.GetConnection();
            string sql = "DELETE FROM Types WHERE Id = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id }) > 0;
        }
    }
}