namespace Hotel.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int TypeId { get; set; }
        public int RoomId { get; set; }
        public int StatusId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Price { get; set; }
        public string? Notes { get; set; }

    }
}

