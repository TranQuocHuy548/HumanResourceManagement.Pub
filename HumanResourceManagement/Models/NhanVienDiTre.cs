using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanResourceManagement.Models
{
	public class NhanVienDiTre
	{
        public int? MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public TimeSpan? ThoiGianVao { get; set; }
        public string GioVaoCa { get; set; }
    }
}