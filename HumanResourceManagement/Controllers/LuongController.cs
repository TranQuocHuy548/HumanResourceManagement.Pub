using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using HumanResourceManagement.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace HumanResourceManagement.Controllers
{

	public class LuongController : Controller
	{
		private HRMEntities db = new HRMEntities();

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult TinhLuong(int? thang, int? nam)
		{
            int thangValue = thang ?? DateTime.Now.Month;
            int namValue = nam ?? DateTime.Now.Year;
            double tileOT = 1.5;

            // Lương tháng
            List<LuongViewModel> bangLuong = (from c in db.chamcongs
                                              where c.Ngay.HasValue && c.Ngay.Value.Month == thangValue && c.Ngay.Value.Year == namValue
                                              group c by c.MaNhanVien into g
                                              select new LuongViewModel
                                              {
                                                  MaNhanVien = (g.Key ?? 0),
                                                  TenNhanVien = g.FirstOrDefault().nhanvien.TenNhanVien,
                                                  TenChucVu = g.FirstOrDefault().nhanvien.chucvu.TenChucVu,
                                                  LuongTheoGio = (g.FirstOrDefault().nhanvien.chucvu.LuongTheoGio ?? 0.0),
                                                  TongGioLam = g.Sum((chamcong x) => (double)(DbFunctions.DiffMinutes(x.ThoiGianVao, x.ThoiGianRa) ?? 0) / 60.0),
                                                  TongGioOT = 0.0
                                              }).ToList();

            foreach (var item in bangLuong)
            {
                double gioChuan = item.TongGioLam - item.TongGioOT;
                item.LuongChinh = gioChuan * item.LuongTheoGio;
                item.LuongOT = item.TongGioOT * item.LuongTheoGio * tileOT;
                item.TongLuong = item.LuongChinh + item.LuongOT;
            }

            // Lương hôm nay
            DateTime today = DateTime.Today;
            List<LuongViewModel> luongHomNay = (from c in db.chamcongs
                                                where c.Ngay.HasValue && DbFunctions.TruncateTime(c.Ngay.Value) == today
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

		public ActionResult XuatExcelLuong(int thang, int nam)
		{
			
			int thangValue = thang;
			int namValue = nam;
			double tileOT = 1.5;
			List<LuongViewModel> bangLuong = (from c in db.chamcongs
											  where c.Ngay.HasValue && c.Ngay.Value.Month == thangValue && c.Ngay.Value.Year == namValue
											  group c by c.MaNhanVien into g
											  select new LuongViewModel
											  {
												  MaNhanVien = (g.Key ?? 0),
												  TenNhanVien = g.FirstOrDefault().nhanvien.TenNhanVien,
												  TenChucVu = g.FirstOrDefault().nhanvien.chucvu.TenChucVu,
												  LuongTheoGio = (g.FirstOrDefault().nhanvien.chucvu.LuongTheoGio ?? 0.0), // Updated column name
												  TongGioLam = g.Sum((chamcong x) => (double)(DbFunctions.DiffMinutes(x.ThoiGianVao, x.ThoiGianRa) ?? 0) / 60.0),
												  TongGioOT = 0.0
											  }).ToList();
			foreach (LuongViewModel item2 in bangLuong)
			{
				double gioChuan = item2.TongGioLam - item2.TongGioOT;
				item2.LuongChinh = gioChuan * item2.LuongTheoGio;
				item2.LuongOT = item2.TongGioOT * item2.LuongTheoGio * tileOT;
				item2.TongLuong = item2.LuongChinh + item2.LuongOT;
			}
			using (ExcelPackage excel = new ExcelPackage())
			{
				ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("BangLuong");
				worksheet.Cells["A1"].Value = "Mã NV";
				worksheet.Cells["B1"].Value = "Tên Nhân Viên";
				worksheet.Cells["C1"].Value = "Chức Vụ";
				worksheet.Cells["D1"].Value = "Lương Cơ Bản";
				worksheet.Cells["E1"].Value = "Giờ Làm";
				worksheet.Cells["F1"].Value = "OT";
				worksheet.Cells["G1"].Value = "Lương Chính";
				worksheet.Cells["H1"].Value = "Lương OT";
				worksheet.Cells["I1"].Value = "Tổng Lương";
				using (ExcelRange range = worksheet.Cells["A1:I1"])
				{
					range.Style.Font.Bold = true;
					range.Style.Fill.PatternType = ExcelFillStyle.Solid;
					range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
				}
				int row = 2;
				foreach (LuongViewModel item in bangLuong)
				{
					worksheet.Cells[row, 1].Value = item.MaNhanVien;
					worksheet.Cells[row, 2].Value = item.TenNhanVien;
					worksheet.Cells[row, 3].Value = item.TenChucVu;
					worksheet.Cells[row, 4].Value = item.LuongTheoGio;
					worksheet.Cells[row, 5].Value = item.TongGioLam;
					worksheet.Cells[row, 6].Value = item.TongGioOT;
					worksheet.Cells[row, 7].Value = item.LuongChinh;
					worksheet.Cells[row, 8].Value = item.LuongOT;
					worksheet.Cells[row, 9].Value = item.TongLuong;
					row++;
				}
				worksheet.Cells.AutoFitColumns();
				MemoryStream stream = new MemoryStream();
				excel.SaveAs(stream);
				stream.Position = 0L;
				return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BangLuong_{thang}_{nam}.xlsx");
			}


		}
        public ActionResult LuongHomNay()
        {
            DateTime today = DateTime.Today;
            double tileOT = 1.5;

            List<LuongViewModel> luongNgay = (from c in db.chamcongs
                                              where c.Ngay.HasValue && DbFunctions.TruncateTime(c.Ngay.Value) == today
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

            foreach (LuongViewModel item in luongNgay)
            {
                double gioChuan = item.TongGioLam - item.TongGioOT;
                item.LuongChinh = gioChuan * item.LuongTheoGio;
                item.LuongOT = item.TongGioOT * item.LuongTheoGio * tileOT;
                item.TongLuong = item.LuongChinh + item.LuongOT;
            }

            ViewBag.Ngay = today.ToString("dd/MM/yyyy");
            return View("LuongHomNay", luongNgay);
        }

    }
}
