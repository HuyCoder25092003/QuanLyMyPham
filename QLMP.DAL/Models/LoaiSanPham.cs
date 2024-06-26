﻿using System;
using System.Collections.Generic;

namespace QLMP.DAL.Models
{
    public partial class LoaiSanPham
    {
        public LoaiSanPham()
        {
            SanPhams = new HashSet<SanPham>();
        }

        public int MaLoaiSp { get; set; }
        public string? TenLoaiSp { get; set; }

        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}
