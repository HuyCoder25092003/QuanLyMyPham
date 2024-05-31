namespace FrontEnd.Models
{
    public class SanPhamVM
    {
       public SanPhamVM(int maSp, string? tenSp, double? gia, string tenLoaiSp, string? hinhAnh)
        {
            this.maSp = maSp;
            TenSp = tenSp;
            Gia = gia;
            TenLoaiSp = tenLoaiSp;
            HinhAnh = hinhAnh;
        }


        public int maSp { get; set; }
        public string? TenSp { get; set; }
        public double? Gia { get; set; }
        public string TenLoaiSp { get; set; }
        public string? HinhAnh { get; set; }
    }
}
