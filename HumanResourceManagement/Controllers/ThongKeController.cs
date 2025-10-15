using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HumanResourceManagement.Models;
using Newtonsoft.Json;
using System.Drawing;

namespace HumanResourceManagement.Controllers
{

    public class ThongKeController : Controller
    {
        HRMEntities db = new HRMEntities();
        // GET: ThongKe
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GioiTinh()
        {
            var thongKe = db.nhanviens
                .GroupBy(nv => nv.GioiTinh)
                .Select(g => new ThongKe
                {
                    GioiTinh = g.Key,
                    SoLuong = g.Count(),
                    DanhSachNhanVien = g.Select(nv => nv.TenNhanVien).ToList()
                })
                .ToList();

            return View(thongKe);
        }
        public ActionResult PhongBan()
        {
            var thongKe = db.nhanviens
                .GroupBy(nv => nv.phongban.TenPhongBan) // Nhóm theo tên phòng ban
                .Select(g => new
                {
                    TenPhongBan = g.Key,
                    SoLuong = g.Count(),
                    DanhSachNhanVien = g.Select(nv => nv.TenNhanVien).ToList()
                })
                .ToList();

            // Chuyển đổi thành model danh sách
            var model = thongKe.Select(x => new ThongKe
            {
                Ten = x.TenPhongBan,
                SoLuong = x.SoLuong,
                DanhSachNhanVien = x.DanhSachNhanVien
            }).ToList();

            return View(model);
        }

        public ActionResult XuatExcelPhongBan()
        {
            // Phiên bản EPPlus 5+ (bỏ comment dòng này nếu dùng bản 5+)
            // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var thongKe = db.nhanviens
                .GroupBy(nv => nv.phongban.TenPhongBan)
                .Select(g => new
                {
                    TenPhongBan = g.Key,
                    SoLuong = g.Count(),
                    DanhSachNhanVien = g.Select(nv => nv.TenNhanVien).ToList()
                })
                .ToList();

            using (ExcelPackage excel = new ExcelPackage())
            {
                // Cài đặt tiêu đề cho workbook
                excel.Workbook.Properties.Title = "Báo cáo thống kê nhân viên theo phòng ban";
                excel.Workbook.Properties.Author = "Hệ thống Quản lý Nhân sự";
                excel.Workbook.Properties.Company = "Công ty ABC";

                var worksheet = excel.Workbook.Worksheets.Add("ThongKePhongBan");

                /* ===== PHẦN ĐẦU TRANG - QUỐC HIỆU, TIÊU NGỮ ===== */
                // Dòng 1: Quốc hiệu
                worksheet.Cells["A1"].Value = "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM";
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.Font.Size = 14;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:H1"].Merge = true;

                // Dòng 2: Tiêu ngữ
                worksheet.Cells["A2"].Value = "Độc lập - Tự do - Hạnh phúc";
                worksheet.Cells["A2"].Style.Font.Italic = true;
                worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:H2"].Merge = true;

                // Dòng trống
                worksheet.Cells["A3"].Value = "";
                worksheet.Cells["A3:H3"].Merge = true;

                /* ===== TIÊU ĐỀ BÁO CÁO ===== */
                worksheet.Cells["A4"].Value = "BÁO CÁO THỐNG KÊ NHÂN VIÊN THEO PHÒNG BAN";
                worksheet.Cells["A4"].Style.Font.Size = 16;
                worksheet.Cells["A4"].Style.Font.Bold = true;
                worksheet.Cells["A4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A4:H4"].Merge = true;

                // Dòng trống
                worksheet.Cells["A5"].Value = "";
                worksheet.Cells["A5:H5"].Merge = true;

                /* ===== THÔNG TIN NGÀY XUẤT BÁO CÁO ===== */
                worksheet.Cells["D6"].Value = "Ngày xuất báo cáo:";
                worksheet.Cells["D6"].Style.Font.Bold = true;
                worksheet.Cells["E6"].Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                worksheet.Cells["E6"].Style.Font.Italic = true;

                // Dòng trống
                worksheet.Cells["A7"].Value = "";
                worksheet.Cells["A7:H7"].Merge = true;

                /* ===== TIÊU ĐỀ CỘT ===== */
                // Header row
                worksheet.Cells["C8"].Value = "Phòng Ban";
                worksheet.Cells["D8"].Value = "Số Lượng";
                worksheet.Cells["E8"].Value = "Danh Sách Nhân Viên";

                // Định dạng header
                using (var range = worksheet.Cells["C8:E8"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // Đường viền
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Top.Color.SetColor(Color.Black);
                    range.Style.Border.Bottom.Color.SetColor(Color.Black);
                    range.Style.Border.Left.Color.SetColor(Color.Black);
                    range.Style.Border.Right.Color.SetColor(Color.Black);
                }

                /* ===== DỮ LIỆU BÁO CÁO ===== */
                int row = 9; // Bắt đầu từ dòng 9
                foreach (var item in thongKe)
                {
                    worksheet.Cells[row, 3].Value = item.TenPhongBan;
                    worksheet.Cells[row, 4].Value = item.SoLuong;
                    worksheet.Cells[row, 5].Value = string.Join(", ", item.DanhSachNhanVien);

                    // Định dạng các ô dữ liệu
                    using (var range = worksheet.Cells[row, 3, row, 5])
                    {
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        // Đường viền
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Color.SetColor(Color.Black);
                        range.Style.Border.Bottom.Color.SetColor(Color.Black);
                        range.Style.Border.Left.Color.SetColor(Color.Black);
                        range.Style.Border.Right.Color.SetColor(Color.Black);
                    }

                    // Tự động xuống dòng cho danh sách nhân viên
                    worksheet.Cells[row, 5].Style.WrapText = true;

                    row++;
                }

                /* ===== CHÂN TRANG - CHỮ KÝ ===== */
                worksheet.Cells[row + 2, 5].Value = "Người lập báo cáo";
                worksheet.Cells[row + 2, 5].Style.Font.Bold = true;
                worksheet.Cells[row + 2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[row + 2, 5, row + 2, 7].Merge = true;

                worksheet.Cells[row + 4, 5].Value = "(Ký và ghi rõ họ tên)";
                worksheet.Cells[row + 4, 5].Style.Font.Italic = true;
                worksheet.Cells[row + 4, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[row + 4, 5, row + 4, 7].Merge = true;

                /* ===== TỰ ĐỘNG CÂN CHỈNH CỘT ===== */
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                /* ===== XUẤT FILE EXCEL ===== */
                var stream = new MemoryStream();
                excel.SaveAs(stream);
                stream.Position = 0;

                return File(
                    fileStream: stream,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: "ThongKePhongBan.xlsx"
                );
            }
        }
    

        public ActionResult TraCuu()
        {
            return View();
        }

        // Action POST để xử lý tìm kiếm
        [HttpPost]
        public ActionResult TraCuu(string tenNhanVien)
        {
            if (string.IsNullOrEmpty(tenNhanVien))
            {
                ViewBag.Message = "Vui lòng nhập tên nhân viên.";
                return View();
            }

            var nhanVien = db.nhanviens
                .Include("PhongBan") // Hoặc .Include(nv => nv.PhongBan)
                .Include("ChucVu")   // Hoặc .Include(nv => nv.ChucVu)
                .FirstOrDefault(nv => nv.TenNhanVien.Contains(tenNhanVien));

            if (nhanVien == null)
            {
                ViewBag.Message = "Không tìm thấy nhân viên.";
                return View();
            }

            return View(nhanVien);
        }

        public ActionResult ThongKeNhanVien(DateTime? ngay, int? thang, int? nam, string gioVaoCa)
        {
            // Mặc định là ngày hôm nay nếu không có giá trị
            if (!ngay.HasValue) ngay = DateTime.Today;
            if (!thang.HasValue) thang = ngay.Value.Month;
            if (!nam.HasValue) nam = ngay.Value.Year;

            // Nếu người dùng không nhập giờ vào ca, sử dụng giờ mặc định (ví dụ: 08:00)
            TimeSpan gioTre;
            if (string.IsNullOrEmpty(gioVaoCa))
            {
                gioTre = new TimeSpan(8, 0, 0); // Giờ mặc định là 08:00
            }
            else
            {
                // Chuyển chuỗi giờ vào ca thành TimeSpan
                gioTre = TimeSpan.Parse(gioVaoCa);
            }

            // Thống kê số lượng nhân viên làm việc trong ngày
            int soLuongTrongNgay = db.chamcongs
                .Where(c => c.Ngay.HasValue &&
                            c.Ngay.Value.Year == ngay.Value.Year &&
                            c.Ngay.Value.Month == ngay.Value.Month &&
                            c.Ngay.Value.Day == ngay.Value.Day)
                .Select(c => c.MaNhanVien)
                .Distinct()
                .Count();

            // Thống kê số lượng nhân viên làm việc trong tháng
            int soLuongTrongThang = db.chamcongs
                .Where(c => c.Ngay.HasValue &&
                            c.Ngay.Value.Month == thang.Value &&
                            c.Ngay.Value.Year == nam.Value)
                .Select(c => c.MaNhanVien)
                .Distinct()
                .Count();

            // Thống kê danh sách nhân viên đi trễ trong ngày
            var danhSachDiTreNgay = db.chamcongs
                .Where(c => c.Ngay.HasValue &&
                            c.Ngay.Value.Year == ngay.Value.Year &&
                            c.Ngay.Value.Month == ngay.Value.Month &&
                            c.Ngay.Value.Day == ngay.Value.Day &&
                            c.ThoiGianVao > gioTre)
                .Select(c => new NhanVienDiTre
                {
                    MaNhanVien = c.MaNhanVien,
                    TenNhanVien = c.nhanvien.TenNhanVien,
                    ThoiGianVao = c.ThoiGianVao
                })
                .ToList();

            // Thống kê danh sách nhân viên đi trễ trong tháng
            var danhSachDiTreThang = db.chamcongs
                .Where(c => c.Ngay.HasValue &&
                            c.Ngay.Value.Month == thang.Value &&
                            c.Ngay.Value.Year == nam.Value &&
                            c.ThoiGianVao > gioTre)
                .Select(c => new NhanVienDiTre
                {
                    MaNhanVien = c.MaNhanVien,
                    TenNhanVien = c.nhanvien.TenNhanVien,
                    ThoiGianVao = c.ThoiGianVao
                })
                .ToList();

            // Gửi dữ liệu sang View
            ViewBag.Ngay = ngay.Value.ToString("dd/MM/yyyy");
            ViewBag.Thang = thang;
            ViewBag.Nam = nam;
            ViewBag.SoLuongTrongNgay = soLuongTrongNgay;
            ViewBag.SoLuongTrongThang = soLuongTrongThang;
            ViewBag.DanhSachDiTreNgay = danhSachDiTreNgay;
            ViewBag.DanhSachDiTreThang = danhSachDiTreThang;
            ViewBag.GioVaoCa = gioVaoCa; // Truyền giá trị giờ vào ca trở lại View để giữ giá trị đã nhập

            return View();
        }
    }
}