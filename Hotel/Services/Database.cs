using Microsoft.Data.Sqlite;


namespace Hotel.Services
{
    public class Database
    {
        private string _connectionstring = "Data Source=Data.db";
        public SqliteConnection GetConnection()
        {
            var connection = new SqliteConnection(_connectionstring);
            connection.Open();
            return connection;
        }
    }
}
