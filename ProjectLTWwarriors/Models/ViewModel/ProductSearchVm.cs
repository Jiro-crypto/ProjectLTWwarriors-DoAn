using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using PagedList.Mvc;

namespace ProjectLTWwarriors.Models.ViewModel
{
    public class ProductSearchVm
    {
         public string SearchTerm { get; set; }       // từ khóa tìm kiếm (tên, mô tả, tên danh mục)
        public decimal? MinPrice { get; set; }       // giá tối thiểu
        public decimal? MaxPrice { get; set; }       // giá tối đa
        public string SortOrder { get; set; }        // sắp xếp: price_asc, price_desc, name_asc, name_desc

        public IPagedList<Products> Products { get; set; }  // danh sách sản phẩm sau khi lọc + phân trang
    }
}