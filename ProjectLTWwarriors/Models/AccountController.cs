using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectLTWwarriors.Models
{
    public class AccountController : Controller
    {
        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public ActionResult Register(string Ho, string Ten, int Ngay, int Thang, int Nam, string Email, string MatKhau, string XacNhanMatKhau)
        {
            if (MatKhau != XacNhanMatKhau)
            {
                ViewBag.ThongBao = "❌ Mật khẩu xác nhận không khớp!";
                return View();
            }

            var user = new InfoUser
            {
                Ho = Ho,
                Ten = Ten,
                NgaySinh = new DateTime(Nam, Thang, Ngay),
                Email = Email,
                MatKhau = MatKhau
            };

            // Lưu tạm người dùng trong Session (hoặc database nếu có)
            var dsNguoiDung = Session["NguoiDung"] as List<InfoUser> ?? new List<InfoUser>();
            dsNguoiDung.Add(user);
            Session["NguoiDung"] = dsNguoiDung;

            // Chuyển sang trang Welcome
            return RedirectToAction("Welcome", "Home");
        }
    }
}