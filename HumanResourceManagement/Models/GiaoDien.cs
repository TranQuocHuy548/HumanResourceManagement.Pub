using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HumanResourceManagement.Models
{
	public class GiaoDien
	{
        [Key]
        public int Id { get; set; }
        public string MauNen { get; set; } // Lưu màu nền
        public string MauChu { get; set; } // Lưu màu chữ
        public string Theme { get; set; }   // Sáng / Tối
    }
}