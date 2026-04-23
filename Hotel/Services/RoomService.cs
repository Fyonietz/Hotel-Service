using Hotel.Models;
using Dapper;
namespace Hotel.Services
{
    public class RoomService
    {
        private readonly Database _db;

        public RoomService(Database db) => _db = db;

        public async Task<IEnumerable<Room>> GetAll()
        {
            using var conn = _db.GetConnection();
            string sql = "SELECT * FROM Room";
            return await conn.QueryAsync<Room>(sql);
        }

        public async Task<Room?> GetById(int id)
        {
            using var conn = _db.GetConnection();
            string sql = "SELECT * FROM Room WHERE Id = @Id";
            return await conn.QueryFirstOrDefaultAsync<Room>(sql, new { Id = id });
        }

        public async Task<bool> Create(Room room)
        {
            using var conn = _db.GetConnection();
            string sql = @"INSERT INTO Room (TypeId, Name, PricePerNight) 
                           VALUES (@TypeId, @Name, @PricePerNight)";
            return await conn.ExecuteAsync(sql, room) > 0;
        }

        public async Task<bool> Delete(int id)
        {
            using var conn = _db.GetConnection();
            string sql = "DELETE FROM Room WHERE Id = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id }) > 0;
        }

        public async Task<bool> Update(int id, Room updatedRoom)
        {
            using var conn = _db.GetConnection();

            const string sql = @"
        UPDATE Room
        SET Name = @Name,
            PricePerNight = @PricePerNight,
            TypeId = @TypeId
        WHERE Id = @Id;
    ";

            var result = await conn.ExecuteAsync(sql, new
            {
                Id = id,
                updatedRoom.Name,
                updatedRoom.PricePerNight,
                updatedRoom.TypeId
            });

            return result > 0;
        }
    }
}