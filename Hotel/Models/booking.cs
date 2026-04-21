namespace Hotel.Models
{
    public class booking
    {
        public int id { get; set; }
        public int id_pelanggan { get; set; }
        public int id_tipe { get; set; }
        public int id_kamar { get; set; }
        public int id_status { get; set; }
        public DateTime tanggal_pesan { get; set; }
        public DateTime tanggal_akhir { get; set; }
        public decimal harga { get; set; }
        public string? catatan { get; set; }
    }
}
