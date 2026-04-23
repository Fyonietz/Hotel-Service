namespace Hotel.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public int? StatusId { get; set; } 
        public DateOnly BookingDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal Price { get; set; }
        public string? Notes { get; set; }
        public int Days { get; set; }
        
        // Join properties
        public string? UserName { get; set; }
        public string? RoomName { get; set; }
    }
}

