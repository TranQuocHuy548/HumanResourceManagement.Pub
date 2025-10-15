using HumanResourceManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResourceManagement.Controllers
{
    public class ChamCongController : Controller
    {
        HRMEntities db = new HRMEntities();
        // GET: ChamCong
        public ActionResult Index()
        {
            var chamCongList = db.chamcongs
                  .Join(db.nhanviens, c => c.MaNhanVien, n => n.MaNhanVien, (c, n) => new ChamCongView
                  {
                      MaChamCong = c.MaChamCong,
                      MaNhanVien = c.MaNhanVien ?? 0, // Tránh lỗi nullable int
                      TenNhanVien = n.TenNhanVien,
                      Ngay = c.Ngay, // Giữ nguyên kiểu DateTime?
                      ThoiGianVao = c.ThoiGianVao,
                      ThoiGianRa = c.ThoiGianRa,
                  }).ToList();

            return View(chamCongList);
        }
        public ActionResult Details(int id) // id ở đây phải là MaChamCong
        {
            // Tìm bản ghi chấm công theo MaChamCong
            var chamCongDetail = db.chamcongs
                .Where(c => c.MaChamCong == id) // Tìm theo MaChamCong, không phải MaNhanVien
                .Join(db.nhanviens,
                      c => c.MaNhanVien, // Join với bảng nhân viên để lấy tên
                      n => n.MaNhanVien,
                      (c, n) => new ChamCongView // Sử dụng ViewModel
                      {
                          MaChamCong = c.MaChamCong,
                          MaNhanVien = c.MaNhanVien ?? 0,
                          TenNhanVien = n.TenNhanVien,
                          Ngay = c.Ngay,
                          ThoiGianVao = c.ThoiGianVao,
                          ThoiGianRa = c.ThoiGianRa,
                          Scan = c.Scan
                      })
                .FirstOrDefault();

            if (chamCongDetail == null)
            {
                return HttpNotFound(); // Trả về 404 nếu không tìm thấy
            }

            return View(chamCongDetail);
        }
    
    public ActionResult Edit(int id)
        {
            var chamCongDetail = db.chamcongs
                .Where(c => c.MaChamCong == id)
                .Join(db.nhanviens,
                      c => c.MaNhanVien,
                      n => n.MaNhanVien,
                      (c, n) => new ChamCongView
                      {
                          MaChamCong = c.MaChamCong,
                          MaNhanVien = c.MaNhanVien ?? 0,
                          TenNhanVien = n.TenNhanVien,
                          Ngay = c.Ngay,
                          ThoiGianVao = c.ThoiGianVao,
                          ThoiGianRa = c.ThoiGianRa,
                      })
                .FirstOrDefault();

            if (chamCongDetail == null)
            {
                return HttpNotFound();
            }

            return View(chamCongDetail);
        }

        [HttpPost]
        public ActionResult Edit(ChamCongView model)
        {
            if (ModelState.IsValid)
            {
                var chamCong = db.chamcongs.FirstOrDefault(c => c.MaChamCong == model.MaChamCong);
                if (chamCong != null)
                {
                    // Cập nhật ThoiGianRa từ chuỗi
                    TimeSpan? thoiGianRa = null;
                    if (!string.IsNullOrEmpty(model.ThoiGianRa))
                    {
                        try
                        {
                            thoiGianRa = TimeSpan.Parse(model.ThoiGianRa);  // Chuyển đổi chuỗi thành TimeSpan
                        }
                        catch (FormatException)
                        {
                            ModelState.AddModelError("ThoiGianRa", "Giờ ra không đúng định dạng.");
                        }
                    }

                    chamCong.ThoiGianRa = thoiGianRa;
                    db.SaveChanges();
                    TempData["Success"] = "Cập nhật giờ ra thành công!";
                    return RedirectToAction("Details", new { id = model.MaChamCong });
                }
            }

            return View(model);
        }

    }
}
