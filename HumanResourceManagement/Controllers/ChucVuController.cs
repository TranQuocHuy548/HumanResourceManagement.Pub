using HumanResourceManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResourceManagement.Controllers
{
    public class ChucVuController : Controller
    {
        HRMEntities db = new HRMEntities();
        // GET: ChucVu
        public ActionResult Index()
        {
            var chucvus = db.chucvus.ToList(); // Không dùng Include
            return View(chucvus);

        }
        // GET: ChucVu/Create
        public ActionResult Create()
        {
            ViewBag.MaPhongBan = new SelectList(db.phongbans, "MaPhongBan", "TenPhongBan");
            return View();
        }

        // POST: ChucVu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaChucVu, TenChucVu,MaPhongBan")] chucvu chucVu)

        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(chucVu.TenChucVu))
                errors.Add("Tên chức vụ không được để trống.");

            if (chucVu.MaPhongBan == null)
                errors.Add("Vui lòng chọn phòng ban.");
            // Kiểm tra chức vụ đã tồn tại trong phòng ban
            bool isExis = db.chucvus.Any(c => c.TenChucVu == chucVu.TenChucVu && c.MaPhongBan == chucVu.MaPhongBan);
            if (isExis)
                errors.Add("Chức vụ này đã tồn tại trong phòng ban này. Vui lòng nhập tên khác.");

            if (errors.Count > 0)
            {
                ViewBag.Errors = errors;
                ViewBag.MaPhongBan = new SelectList(db.phongbans, "MaPhongBan", "TenPhongBan", chucVu.MaPhongBan);
                return View(chucVu);
            }

            // Kiểm tra trùng chức vụ trong cùng phòng ban
            bool isExist = db.chucvus.Any(c => c.TenChucVu == chucVu.TenChucVu && c.MaPhongBan == chucVu.MaPhongBan);
            if (isExist)
            {
                TempData["ErrorMessage"] = "Chức vụ này đã tồn tại trong phòng ban! Vui lòng nhập tên khác.";
                return RedirectToAction("Create");
            }

            db.chucvus.Add(chucVu);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Thêm chức vụ thành công!";
            return RedirectToAction("Index");
        }


        // GET: ChucVu/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var chucVu = db.chucvus.Find(id); // Tìm chức vụ theo ID
                if (chucVu == null)
                {
                    return HttpNotFound();
                }

                return View(chucVu); // Trả về view để xác nhận xóa
            }
            catch (Exception ex)
            {
                TempData["SwalError"] = "Lỗi khi tìm chức vụ: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: ChucVu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var chucVu = db.chucvus.Find(id);
                if (chucVu == null)
                {
                    return HttpNotFound();
                }

                // Kiểm tra xem chức vụ có nhân viên nào không
                var hasEmployees = db.nhanviens.Any(nv => nv.MaChucVu == id);
                if (hasEmployees)
                {
                    TempData["SwalError"] = "Không thể xóa! Chức vụ này đang có nhân viên.";
                    return RedirectToAction("Index");
                }

                db.chucvus.Remove(chucVu);
                db.SaveChanges();
                TempData["SwalSuccess"] = "Xóa chức vụ thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["SwalError"] = "Lỗi khi xóa chức vụ: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        // GET: ChucVu/Edit/5
        public ActionResult Edit(int id)
        {
            var chucVu = db.chucvus.Find(id);
            if (chucVu == null)
            {
                return HttpNotFound();
            }
            return View(chucVu);
        }


        // POST: ChucVu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Include = "MaChucVu, TenChucVu")] chucvu chucVu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingChucVu = db.chucvus.Find(id);
                    if (existingChucVu == null)
                    {
                        return HttpNotFound();
                    }

                    // Cập nhật các trường từ chucVu vào existingChucVu
                    existingChucVu.TenChucVu = chucVu.TenChucVu;

                    // Lưu thay đổi
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Lỗi khi cập nhật chức vụ: " + ex.Message;
                    return View("Error");
                }
            }
            return View(chucVu);
        }


    }

}

