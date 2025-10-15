using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HumanResourceManagement.Models;

namespace HumanResourceManagement.Controllers
{
    public class NhanVienController : Controller
    {
        HRMEntities db = new HRMEntities();

        // GET: NhanVien
        public ActionResult Index(string keyword)
        {
            try
            {
                // Lấy danh sách nhân viên từ cơ sở dữ liệu
                var nhanViens = db.nhanviens.AsQueryable();

                // Lọc theo từ khóa nếu có
                if (!string.IsNullOrEmpty(keyword))
                {
                    nhanViens = nhanViens.Where(nv => nv.TenNhanVien.Contains(keyword) || nv.Email.Contains(keyword));
                }

                var chucVus = db.chucvus.ToList();
                var phongBans = db.phongbans.ToList();

                if (!nhanViens.Any())
                {
                    ViewBag.Message = "Không có nhân viên nào phù hợp với từ khóa tìm kiếm.";
                }
                else
                {
                    ViewBag.Message = $"Số lượng nhân viên: {nhanViens.Count()}";
                }

                var nhanVienWithChucVuPhongBan = nhanViens.ToList().Select(nv => new
                {
                    nv,
                    TenChucVu = chucVus.FirstOrDefault(cv => cv.MaChucVu == nv.MaChucVu)?.TenChucVu ?? "Chưa có chức vụ",
                    TenPhongBan = phongBans.FirstOrDefault(pb => pb.MaPhongBan == nv.MaPhongBan)?.TenPhongBan ?? "Chưa có phòng ban"
                }).ToList();

                var model = nhanVienWithChucVuPhongBan.Select(item => new nhanvien
                {
                    MaNhanVien = item.nv.MaNhanVien,
                    TenNhanVien = item.nv.TenNhanVien,
                    Email = item.nv.Email,
                    GioiTinh = item.nv.GioiTinh,
                    NgaySinh = item.nv.NgaySinh,
                    MaChucVu = item.nv.MaChucVu,
                    MaPhongBan = item.nv.MaPhongBan,
                    TenChucVu = item.TenChucVu,
                    TenPhongBan = item.TenPhongBan
                }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi khi tải dữ liệu: " + ex.Message;
                return View("Error");
            }
        }

        // GET: NhanVien/Edit/5
        public ActionResult Edit(int id)
        {
            // Tìm nhân viên theo ID
            var nhanVien = db.nhanviens.Find(id);

            if (nhanVien == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách phòng ban và chức vụ để hiển thị trong dropdown
            ViewBag.MaPhongBan = new SelectList(db.phongbans, "MaPhongBan", "TenPhongBan", nhanVien.MaPhongBan);
            ViewBag.MaChucVu = new SelectList(db.chucvus, "MaChucVu", "TenChucVu", nhanVien.MaChucVu);

            return View(nhanVien); ;
        }


        // POST: NhanVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Include = "MaNhanVien,TenNhanVien,Email,GioiTinh,NgaySinh,MaPhongBan,MaChucVu")] nhanvien nhanVien)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingNhanVien = db.nhanviens.Find(id);
                    if (existingNhanVien == null)
                    {
                        return HttpNotFound();
                    }

                    // Cập nhật thông tin nhân viên
                    existingNhanVien.TenNhanVien = nhanVien.TenNhanVien;
                    existingNhanVien.Email = nhanVien.Email;
                    existingNhanVien.GioiTinh = nhanVien.GioiTinh;
                    existingNhanVien.NgaySinh = nhanVien.NgaySinh;
                    existingNhanVien.MaPhongBan = nhanVien.MaPhongBan; // Cập nhật mã phòng ban
                    existingNhanVien.MaChucVu = nhanVien.MaChucVu; // Cập nhật mã chức vụ

                    // Lưu thay đổi
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Lỗi khi cập nhật nhân viên: " + ex.Message;
                    return View("Error");
                }
            }

            // Nếu có lỗi validation, trả về view và tiếp tục hiển thị dữ liệu
            ViewBag.MaPhongBan = new SelectList(db.phongbans, "MaPhongBan", "TenPhongBan", nhanVien.MaPhongBan);
            ViewBag.MaChucVu = new SelectList(db.chucvus, "MaChucVu", "TenChucVu", nhanVien.MaChucVu);

            return View(nhanVien);
        }

        // GET: NhanVien/Details/5
        public ActionResult Details(int id)
        {
            var nhanVien = db.nhanviens.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }

        // GET: NhanVien/Delete/5
        public ActionResult Delete(int id)
        {
            var nhanVien = db.nhanviens.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }

            // Đặt thông tin nhân viên vào ViewBag
            ViewBag.EmployeeName = nhanVien.TenNhanVien;
            ViewBag.EmployeeId = nhanVien.MaNhanVien;

            return View(nhanVien);
        }

        // POST: NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Tìm nhân viên cần xóa
            var nhanVien = db.nhanviens.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }

            // Xóa nhân viên khỏi cơ sở dữ liệu
            db.chamcongs.RemoveRange(nhanVien.chamcongs);
            db.nhanviens.Remove(nhanVien);
            db.SaveChanges();


            // Quay về trang danh sách nhân viên sau khi xóa
            return RedirectToAction("Index");
        }
        public ActionResult Create()
        {
            ViewBag.MaPhongBan = new SelectList(db.phongbans, "MaPhongBan", "TenPhongBan");
            ViewBag.MaChucVu = new SelectList(db.chucvus, "MaChucVu", "TenChucVu");
            return View();
        }

        // Xử lý form thêm nhân viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(nhanvien nhanVien)
        {
            List<string> errors = new List<string>();

            // Kiểm tra tên nhân viên
            if (string.IsNullOrWhiteSpace(nhanVien.TenNhanVien))
                errors.Add("Tên nhân viên không được để trống.");

            // Kiểm tra ngày sinh
            if (nhanVien.NgaySinh == null)
                errors.Add("Ngày sinh không được để trống.");
            else if (nhanVien.NgaySinh > DateTime.Now)
                errors.Add("Ngày sinh không hợp lệ (không thể lớn hơn ngày hiện tại).");

            // Kiểm tra giới tính
            if (string.IsNullOrWhiteSpace(nhanVien.GioiTinh))
                errors.Add("Giới tính không được để trống.");

            // Kiểm tra email
            if (string.IsNullOrWhiteSpace(nhanVien.Email))
                errors.Add("Email không được để trống.");
            else if (!nhanVien.Email.Contains("@"))
                errors.Add("Email không hợp lệ.");
            else if (db.nhanviens.Any(nv => nv.Email == nhanVien.Email))
                errors.Add("Email đã tồn tại! Vui lòng nhập email khác.");

            // Kiểm tra số điện thoại
            else if (!nhanVien.SDT.All(char.IsDigit))
                errors.Add("Số điện thoại chỉ được chứa số.");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(nhanVien.SDT, @"^\d{10}$"))
                errors.Add("Số điện thoại phải có đúng 10 chữ số.");
            else if (db.nhanviens.Any(n => n.SDT == nhanVien.SDT))
                errors.Add("Số điện thoại này đã được sử dụng. Vui lòng nhập số khác.");
            // Kiểm tra chọn phòng ban
            if (nhanVien.MaPhongBan == null)
                errors.Add("Vui lòng chọn phòng ban.");

            // Kiểm tra chọn chức vụ
            if (nhanVien.MaChucVu == null)
                errors.Add("Vui lòng chọn chức vụ.");

            if (errors.Count > 0)
            {
                ViewBag.Errors = errors;
                ViewBag.MaPhongBan = new SelectList(db.phongbans, "MaPhongBan", "TenPhongBan", nhanVien.MaPhongBan);
                ViewBag.MaChucVu = new SelectList(db.chucvus, "MaChucVu", "TenChucVu", nhanVien.MaChucVu);
                return View(nhanVien);
            }

            // Thêm vào database
            db.nhanviens.Add(nhanVien);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Thêm nhân viên thành công!";
            return RedirectToAction("Index");
        }
        public JsonResult GetChucVuByPhongBan(int maPhongBan)
        {
            var chucVus = db.chucvus.Where(cv => cv.MaPhongBan == maPhongBan)
                           .Select(cv => new { cv.MaChucVu, cv.TenChucVu })
                           .ToList();

            // Lấy danh sách chức vụ đã có nhân viên trong phòng ban
            var chucVuDaCoNhanVien = db.nhanviens.Where(nv => nv.MaPhongBan == maPhongBan)
                                                .Select(nv => nv.MaChucVu)
                                                .Distinct()
                                                .ToList();

            return Json(new { chucVus, chucVuDaCoNhanVien }, JsonRequestBehavior.AllowGet);
        }

    }
}
