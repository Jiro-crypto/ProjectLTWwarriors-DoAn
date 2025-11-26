using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectLTWwarriors.Models
{
    public class MatHangTrongGio
    {
        // Sản phẩm được mua
        public Product SanPham { get; set; }

        // Số lượng của sản phẩm đó
        public int SoLuong { get; set; }
    }
}