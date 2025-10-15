using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanResourceManagement.Models
{
	public class LuongViewModel
	{
        public int MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string TenChucVu { get; set; }
        public double LuongTheoGio { get; set; }
        public double TongGioLam { get; set; }
        public double TongGioOT { get; set; }
        public double LuongChinh { get; set; }
        public double LuongOT { get; set; }
        public double TongLuong { get; set; }
    }
}