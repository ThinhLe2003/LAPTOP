namespace LAPTOP.Models
{
    public class GioHangItem
    {
        public string MaSp { get; set; }         
        public string TenSp { get; set; }       
        public decimal Gia { get; set; }
        public int SoLuong { get; set; }
        public string HinhAnh { get; set; }
        public decimal ThanhTien => Gia * SoLuong;
    }
}
