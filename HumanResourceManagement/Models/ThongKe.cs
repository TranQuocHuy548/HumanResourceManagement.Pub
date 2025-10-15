using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanResourceManagement.Models
{
	public class ThongKe
	{
        public string GioiTinh { get; set; }
        public int SoLuong { get; set; }
        public List<string> DanhSachNhanVien { get; set; }
        public string DiaChi { get; set; }
        public string Ten { get; set; } // Chứa tên phòng ban, giới tính hoặc địa chỉ
    }
}