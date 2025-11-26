using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectLTWwarriors.Models
{
   
        // GET: DonHang
        public class DonHang
        {
            public string MaDon { get; set; }
            public DateTime CreatedAt { get; set; }

            // Thông tin người nhận
            public string HoTen { get; set; }
            public string SoDienThoai { get; set; }
            public string DiaChiDayDu { get; set; }

            // Thanh toán & trạng thái
            public string PhuongThucThanhToan { get; set; }  // COD | Momo | Bank
            public string TrangThai { get; set; }            // processing|paid|shipping|completed|cancelled
            public string MaKhuyenMai { get; set; }

            // Tiền
            public decimal TamTinh { get; set; }
            public decimal PhiVanChuyen { get; set; }
            public decimal GiamGia { get; set; }
            public decimal TongCong { get; set; }

            // DÙNG LUÔN ITEMS CỦA GIỎ
            public List<MatHangTrongGio> Items { get; set; } = new List<MatHangTrongGio>();
        }





    }

