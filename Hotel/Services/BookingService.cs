using Hotel.Models;
using Microsoft.Data.Sqlite;

namespace Hotel.Services
{
    public class BookingService
    {
        private readonly Database _db;

        public BookingService(Database db) => _db = db;

        public async Task<IEnumerable<Booking>> GetAll()
        {
            var list = new List<Booking>();

            using var conn = _db.GetConnection();
            await conn.OpenAsync();

            string sql = @"
            SELECT b.*, 
                   u.Name as UserName,
                   r.Name as RoomName
            FROM Booking b
            JOIN Users u ON b.UserId = u.Id
            JOIN Room r ON b.RoomId = r.Id";

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var bookingDate = DateOnly.Parse(reader.GetString(reader.GetOrdinal("BookingDate")));
                var endDate = DateOnly.Parse(reader.GetString(reader.GetOrdinal("EndDate")));

                var booking = new Booking
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    RoomId = reader.GetInt32(reader.GetOrdinal("RoomId")),
                    StatusId = reader.GetInt32(reader.GetOrdinal("StatusId")),
                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),

                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Notes")),

                    BookingDate = bookingDate,
                    EndDate = endDate,
                    Days = endDate.DayNumber - bookingDate.DayNumber,

                    UserName = reader.GetString(reader.GetOrdinal("UserName")),
                    RoomName = reader.GetString(reader.GetOrdinal("RoomName"))
                };

                list.Add(booking);
            }

            return list;
        }

        
        public async Task<Booking?> GetById(int id)
        {
            using var conn = _db.GetConnection();
            await conn.OpenAsync();

            string sql = @"
            SELECT b.*, 
                   u.Name as UserName,
                   r.Name as RoomName
            FROM Booking b
            JOIN Users u ON b.UserId = u.Id
            JOIN Room r ON b.RoomId = r.Id
            WHERE b.Id = @Id";

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            var bookingDate = DateOnly.Parse(reader.GetString(reader.GetOrdinal("BookingDate")));
            var endDate = DateOnly.Parse(reader.GetString(reader.GetOrdinal("EndDate")));

            return new Booking
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                RoomId = reader.GetInt32(reader.GetOrdinal("RoomId")),
                StatusId = reader.GetInt32(reader.GetOrdinal("StatusId")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),

                Notes = reader.IsDBNull(reader.GetOrdinal("Notes"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Notes")),

                BookingDate = bookingDate,
                EndDate = endDate,
                Days = endDate.DayNumber - bookingDate.DayNumber,

                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                RoomName = reader.GetString(reader.GetOrdinal("RoomName"))
            };
        }

         
        public decimal Cost(int days, int type)
        {
            if (days <= 0)
                throw new ArgumentException("Days harus > 0");

            decimal price = type switch
            {
                1 => 200_000,
                2 => 350_000,
                3 => 500_000,
                4 => 750_000,
                _ => throw new ArgumentException("Type tidak valid")
            };

            return days * price;
        }

          
        public async Task<bool> Create(Booking booking)
        {
            using var conn = _db.GetConnection();
            await conn.OpenAsync();

            int days = booking.EndDate.DayNumber - booking.BookingDate.DayNumber;

            if (days <= 0)
                throw new Exception("EndDate harus lebih besar dari BookingDate");

            // Guard: Cek ketersediaan kamar
            if (!await IsRoomAvailable(conn, booking.RoomId, booking.BookingDate, booking.EndDate))
                throw new Exception("Kamar sudah dipesan untuk tanggal tersebut");

            decimal pricePerNight;

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT PricePerNight FROM Room WHERE Id = @RoomId";
                cmd.Parameters.AddWithValue("@RoomId", booking.RoomId);

                using var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                    throw new Exception("Room tidak ditemukan");

                pricePerNight = reader.GetDecimal(0);
            }

            booking.Price = days * pricePerNight;

            using var insertCmd = conn.CreateCommand();
            insertCmd.CommandText = @"
            INSERT INTO Booking 
            (UserId, RoomId, StatusId, BookingDate, EndDate, Price, Notes)
            VALUES 
            (@UserId, @RoomId, @StatusId, @BookingDate, @EndDate, @Price, @Notes)";

            insertCmd.Parameters.AddWithValue("@UserId", booking.UserId);
            insertCmd.Parameters.AddWithValue("@RoomId", booking.RoomId);
            insertCmd.Parameters.AddWithValue("@StatusId", booking.StatusId);
            insertCmd.Parameters.AddWithValue("@BookingDate", booking.BookingDate.ToString("yyyy-MM-dd"));
            insertCmd.Parameters.AddWithValue("@EndDate", booking.EndDate.ToString("yyyy-MM-dd"));
            insertCmd.Parameters.AddWithValue("@Price", booking.Price);
            insertCmd.Parameters.AddWithValue("@Notes", (object?)booking.Notes ?? DBNull.Value);

            return await insertCmd.ExecuteNonQueryAsync() > 0;
        }

        private async Task<bool> IsRoomAvailable(SqliteConnection conn, int roomId, DateOnly startDate, DateOnly endDate, int? excludeId = null)
        {
            // Cek apakah ada booking yang overlap dengan tanggal yang diminta
            // Overlap terjadi jika: newStart < existingEnd AND newEnd > existingStart
            using var cmd = conn.CreateCommand();
            
            if (excludeId.HasValue)
            {
                cmd.CommandText = @"
                    SELECT COUNT(*) FROM Booking 
                    WHERE RoomId = @RoomId 
                    AND Id != @ExcludeId
                    AND StatusId != 3  -- Bukan cancelled
                    AND BookingDate <= @EndDate 
                    AND EndDate >= @StartDate";
                cmd.Parameters.AddWithValue("@ExcludeId", excludeId.Value);
            }
            else
            {
                cmd.CommandText = @"
                    SELECT COUNT(*) FROM Booking 
                    WHERE RoomId = @RoomId 
                    AND StatusId != 3  -- Bukan cancelled
                    AND BookingDate <= @EndDate 
                    AND EndDate >= @StartDate";
            }

            cmd.Parameters.AddWithValue("@RoomId", roomId);
            cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));

            var result = await cmd.ExecuteScalarAsync();
            var count = Convert.ToInt32(result);

            return count == 0;
        }

        public async Task<bool> Update(int id, Booking booking)
        {
            using var conn = _db.GetConnection();
            await conn.OpenAsync();

            int days = booking.EndDate.DayNumber - booking.BookingDate.DayNumber;

            if (days <= 0)
                throw new Exception("EndDate harus lebih besar dari BookingDate");

            // Guard: Cek ketersediaan kamar (exclude booking saat ini)
            if (!await IsRoomAvailable(conn, booking.RoomId, booking.BookingDate, booking.EndDate, id))
                throw new Exception("Kamar sudah dipesan untuk tanggal tersebut");

            decimal pricePerNight;

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT PricePerNight FROM Room WHERE Id = @RoomId";
                cmd.Parameters.AddWithValue("@RoomId", booking.RoomId);

                using var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                    throw new Exception("Room tidak ditemukan");

                pricePerNight = reader.GetDecimal(0);
            }

            booking.Price = days * pricePerNight;

            using var updateCmd = conn.CreateCommand();
            updateCmd.CommandText = @"
            UPDATE Booking
            SET UserId = @UserId,
                RoomId = @RoomId,
                StatusId = @StatusId,
                BookingDate = @BookingDate,
                EndDate = @EndDate,
                Price = @Price,
                Notes = @Notes
            WHERE Id = @Id";

            updateCmd.Parameters.AddWithValue("@Id", id);
            updateCmd.Parameters.AddWithValue("@UserId", booking.UserId);
            updateCmd.Parameters.AddWithValue("@RoomId", booking.RoomId);
            updateCmd.Parameters.AddWithValue("@StatusId", 1);
            updateCmd.Parameters.AddWithValue("@BookingDate", booking.BookingDate.ToString("yyyy-MM-dd"));
            updateCmd.Parameters.AddWithValue("@EndDate", booking.EndDate.ToString("yyyy-MM-dd"));
            updateCmd.Parameters.AddWithValue("@Price", booking.Price);
            updateCmd.Parameters.AddWithValue("@Notes", (object?)booking.Notes ?? DBNull.Value);

            return await updateCmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> Delete(int id)
        {
            using var conn = _db.GetConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Booking WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}