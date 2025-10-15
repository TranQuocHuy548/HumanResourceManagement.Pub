using HumanResourceManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResourceManagement.Areas.User.Controllers
{
    public class ViewLuongController : Controller
    {
        HRMEntities db = new HRMEntities();  // Đối tượng DbContext

        // GET: User/Luong
        public ActionResult Index(int? thang, int? nam)
        {
            string email = Session["Email"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "DangNhap", new { area = "" });
            }

            // 🔍 Tìm nhân viên đăng nhập bằng email
            var nhanVien = db.nhanviens.FirstOrDefault(nv => nv.Email == email);
            if (nhanVien == null)
            {
                return HttpNotFound("Không tìm thấy nhân viên với email này.");
            }

            int maNV = nhanVien.MaNhanVien;

            int thangValue = thang ?? DateTime.Now.Month;
            int namValue = nam ?? DateTime.Now.Year;
            double tileOT = 1.5;

            // ✅ Lấy bảng lương tháng của nhân viên đang đăng nhập
            List<LuongViewModel> bangLuong = (from c in db.chamcongs
                                              where c.MaNhanVien == maNV &&
                                                    c.Ngay.HasValue &&
                                                    c.Ngay.Value.Month == thangValue &&
                                                    c.Ngay.Value.Year == namValue
                                              group c by c.MaNhanVien into g
                                              select new LuongViewModel
                                              {
                                                  MaNhanVien = (g.Key ?? 0),
                                                  TenNhanVien = g.FirstOrDefault().nhanvien.TenNhanVien,
                                                  TenChucVu = g.FirstOrDefault().nhanvien.chucvu.TenChucVu,
                                                  LuongTheoGio = (g.FirstOrDefault().nhanvien.chucvu.LuongTheoGio ?? 0.0),
                                                  TongGioLam = g.Sum(x => (double)(DbFunctions.DiffMinutes(x.ThoiGianVao, x.ThoiGianRa) ?? 0) / 60.0),
                                                  TongGioOT = 0.0
                                              }).ToList();

            foreach (var item in bangLuong)
            {
                double gioChuan = item.TongGioLam - item.TongGioOT;
                item.LuongChinh = gioChuan * item.LuongTheoGio;
                item.LuongOT = item.TongGioOT * item.LuongTheoGio * tileOT;
                item.TongLuong = item.LuongChinh + item.LuongOT;
            }

            // ✅ Lấy bảng lương hôm nay của nhân viên đăng nhập
            DateTime today = DateTime.Today;
            List<LuongViewModel> luongHomNay = (from c in db.chamcongs
                                                where c.MaNhanVien == maNV &&
                                                      c.Ngay.HasValue &&
                                                      DbFunctions.TruncateTime(c.Ngay.Value) == today
                                                group c by c.MaNhanVien into g
                                                select new LuongViewModel
                                                {
                                                    MaNhanVien = (g.Key ?? 0),
                                                    TenNhanVien = g.FirstOrDefault().nhanvien.TenNhanVien,
                                                    TenChucVu = g.FirstOrDefault().nhanvien.chucvu.TenChucVu,
                                                    LuongTheoGio = (g.FirstOrDefault().nhanvien.chucvu.LuongTheoGio ?? 0.0),
                                                    TongGioLam = g.Sum(x => (double)(DbFunctions.DiffMinutes(x.ThoiGianVao, x.ThoiGianRa) ?? 0) / 60.0),
                                                    TongGioOT = 0.0
                                                }).ToList();

            foreach (var item in luongHomNay)
            {
                double gioChuan = item.TongGioLam - item.TongGioOT;
                item.LuongChinh = gioChuan * item.LuongTheoGio;
                item.LuongOT = item.TongGioOT * item.LuongTheoGio * tileOT;
                item.TongLuong = item.LuongChinh + item.LuongOT;
            }

            ViewBag.Thang = thangValue;
            ViewBag.Nam = namValue;
            ViewBag.LuongHomNay = luongHomNay;
            ViewBag.Ngay = today.ToString("dd/MM/yyyy");

            return View(bangLuong);
        }
    }
}
