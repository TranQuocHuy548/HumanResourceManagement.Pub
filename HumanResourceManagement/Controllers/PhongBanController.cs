using HumanResourceManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResourceManagement.Controllers
{
    public class PhongBanController : Controller
    {
        HRMEntities db = new HRMEntities();
        // GET: PhongBan
        public ActionResult Index()
        {
           

            // Lấy danh sách phòng ban từ cơ sở dữ liệu
            var phongBans = db.phongbans.ToList();

            // Nếu danh sách phòng ban là null, tạo một danh sách rỗng thay vì trả về null
            if (phongBans == null)
            {
                phongBans = new List<phongban>(); // Đảm bảo không trả về null
            }

            // Trả về view với model hợp lệ
            return View(phongBans); // Truyền danh sách phòng ban vào view
        }
        public ActionResult Edit(int id)
        {
            try
            {
                var phongBan = db.phongbans.Find(id);  // Tìm phòng ban theo ID
                if (phongBan == null)
                {
                    return HttpNotFound();  // Nếu không tìm thấy phòng ban, trả về lỗi
                }
                return View(phongBan);  // Trả về phòng ban cho view
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi khi tìm phòng ban: " + ex.Message;
                return View("Error");  // Nếu có lỗi, trả về view Error
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Include = "TenPhongBan")] phongban phongBan)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(phongBan.TenPhongBan))
            {
                errors.Add("Tên phòng ban không được để trống.");
            }

            // Kiểm tra phòng ban đã tồn tại chưa
            bool isExist = db.phongbans.Any(pb => pb.TenPhongBan.Trim().ToLower() == phongBan.TenPhongBan.Trim().ToLower());

            Console.WriteLine($"DEBUG: isExist = {isExist}");  // Thêm dòng này để kiểm tra giá trị

            if (isExist)
            {
                errors.Add("Phòng ban này đã tồn tại! Vui lòng nhập tên khác.");
            }

            if (errors.Count > 0)
            {
                ViewBag.Errors = errors;
                return View(phongBan);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.phongbans.Add(phongBan);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Thêm phòng ban thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Lỗi khi thêm phòng ban: " + ex.Message;
                    return View("Error");
                }
            }

            return View(phongBan);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaPhongBan, TenPhongBan")] phongban phongBan)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(phongBan.TenPhongBan))
            {
                errors.Add("Tên phòng ban không được để trống.");
            }

            // Kiểm tra xem phòng ban đã tồn tại chưa
            bool exists = db.phongbans.Any(p => p.TenPhongBan == phongBan.TenPhongBan);
            if (exists)
            {
                errors.Add("Tên phòng ban này đã tồn tại!");
            }

            if (errors.Count > 0)
            {
                ViewBag.Errors = errors;
                return View(phongBan);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Thêm phòng ban mới vào cơ sở dữ liệu
                    db.phongbans.Add(phongBan);
                    db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                    TempData["SuccessMessage"] = "Thêm phòng ban thành công!";
                    return RedirectToAction("Index", "PhongBan"); // Quay lại trang danh sách phòng ban
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Lỗi khi thêm phòng ban: " + ex.Message;
                    return RedirectToAction("Index");
                }
            }
            return View(phongBan); // Nếu model không hợp lệ, quay lại view
        }
        // Action GET để xác nhận xóa phòng ban
        public ActionResult Delete(int id)
        {
            try
            {
                // Tìm phòng ban theo ID
                var phongBan = db.phongbans.Find(id);

                if (phongBan == null)
                {
                    return HttpNotFound();
                }

                // Trả về view với thông tin phòng ban cần xóa
                return View(phongBan);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tìm phòng ban: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        // Action POST để xóa phòng ban
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var phongBan = db.phongbans.Find(id);
                if (phongBan == null)
                {
                    return HttpNotFound();
                }

                if (db.nhanviens.Any(nv => nv.MaPhongBan == id))
                {
                    TempData["SwalError"] = "Không thể xóa! Phòng ban này đang có nhân viên.";
                    return RedirectToAction("Index");
                }

                db.phongbans.Remove(phongBan);
                db.SaveChanges();

                TempData["SwalSuccess"] = "Xóa phòng ban thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["SwalError"] = "Lỗi khi xóa phòng ban: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

    }
}
