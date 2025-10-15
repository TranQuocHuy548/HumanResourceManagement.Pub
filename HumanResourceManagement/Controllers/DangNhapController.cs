using HumanResourceManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HumanResourceManagement.Controllers
{
    public class DangNhapController : Controller
    {
        HRMEntities db = new HRMEntities();
        // GET: DangNhap
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string username, string password)
        {
            Console.WriteLine("Email nhập: " + username);
            Console.WriteLine("Mật khẩu nhập: " + password);

            var nv = db.nhanviens.FirstOrDefault(x => x.Email.Trim() == username.Trim());

            if (nv != null)
            {
                Console.WriteLine("Email trong DB: " + nv.Email);
                Console.WriteLine("Mật khẩu trong DB: " + nv.Password);

                if (nv.Password.Trim() == password.Trim())
                {
                    // Đăng nhập thành công
                    Session["Email"] = nv.Email;
                    Session["MaNhanVien"] = nv.MaNhanVien;
                    Session["HoTen"] = nv.TenNhanVien;
                    Session["PhanQuyen"] = nv.PhanQuyen;

                    if (nv.PhanQuyen.Trim().ToLower() == "admin")
                    {
                        return RedirectToAction("Index", "DashB");
                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(username, false);
                        return RedirectToAction("Index", "Home", new { area = "User" });
                    }
                }
                else
                {
                    ViewBag.MatKhauDB = nv.Password;
                    ViewBag.EmailDB = nv.Email;
                    ViewBag.ThongBao = "Sai mật khẩu!";
                }
            }
            else
            {
                ViewBag.ThongBao = "Không tìm thấy tài khoản!";
            }

            return View();
        }

    }
}