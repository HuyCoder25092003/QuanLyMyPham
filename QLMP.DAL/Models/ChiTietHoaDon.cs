﻿using System;
using System.Collections.Generic;

namespace QLMP.DAL.Models
{
    public partial class ChiTietHoaDon
    {
        public int MaHoaDon { get; set; }
        public int MaSp { get; set; }
        public short? SoLuong { get; set; }
        public double? DonGia { get; set; }
        public double? GiamGia { get; set; }

        public virtual HoaDon MaHoaDonNavigation { get; set; } = null!;
        public virtual SanPham MaSpNavigation { get; set; } = null!;
    }
}
