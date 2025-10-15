using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanResourceManagement.Models
{
	public class ChamCongView
	{
        public int MaChamCong { get; set; }
        public int? MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public DateTime? Ngay { get; set; } // Giữ nguyên kiểu DateTime?
        public TimeSpan? ThoiGianVao { get; set; } // Sửa thành TimeSpan?
        public TimeSpan? ThoiGianRa { get; set; } // Sửa thành TimeSpan?
        public string Scan { get; set; }
    }
}