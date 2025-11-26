using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectLTWwarriors.Models.ViewModel
{
    public class HomeProductVM
    {
        //public string SearchTerm { get; set; }

        //public int PageNumber { get; set; }
        //public int PageSize { get; set; } = 5;

        //public List<Products> FeaturedProducts { get; set; }

        //public IPagedList<Products> NewProducts { get; set; }

        public string SearchTerm { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;

        // Featured Products
        //public IEnumerable<Products> FeaturedProducts { get; set; } // dùng cho Partial View

        // New Products (danh sách đầy đủ để hiển thị theo danh mục)
        public IEnumerable<Products> NewProducts { get; set; }

        // New Products phân trang (dùng cho trang list sản phẩm)
        public IPagedList<Products> NewProductsPaged { get; set; }
    }
}