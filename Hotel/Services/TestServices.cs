using Hotel.Models;

namespace Hotel.Services
{
    public class TestServices
    {
        private readonly Database db;

        public TestServices(Database _db) =>db=_db;

        public async Task<Test> GetData()
        {
            using var conn = db.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Test";
            using var reader = cmd.ExecuteReader();
            var data = new Test();
            if (reader.Read())
            {
                data = new Test
                {
                    Data = reader.GetString(0)
                };
            }

            return await Task.FromResult(data);
        }
    }
}
