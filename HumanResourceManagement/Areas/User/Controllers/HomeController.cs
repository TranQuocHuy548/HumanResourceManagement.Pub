using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HumanResourceManagement.Models; 

namespace HumanResourceManagement.Areas.User.Controllers
{
    public class HomeController : Controller
    {
        HRMEntities db = new HRMEntities();

        public ActionResult Index()
        {
            // Lấy email của người dùng đã đăng nhập từ session
            string email = Session["Email"]?.ToString();
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Index", "DangNhap", new { area = "" });

            // Tìm nhân viên trong bảng nhanvien dựa vào email
            var nv = db.nhanviens.FirstOrDefault(x => x.Email == email);
            if (nv == null)
                return HttpNotFound();

            // Trả về view với đối tượng nhân viên
            return View(nv);
        }

        [HttpPost]
        public ActionResult CapNhat(nhanvien model)
        {
            var nv = db.nhanviens.FirstOrDefault(x => x.MaNhanVien == model.MaNhanVien);
            if (nv == null)
                return HttpNotFound();

            // Cập nhật thông tin sửa đổi (SĐT và Địa chỉ)
            nv.SDT = model.SDT;
            nv.DiaChi = model.DiaChi;
            db.SaveChanges();

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Index");
        }
    }
}
