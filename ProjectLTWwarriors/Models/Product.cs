using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectLTWwarriors.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public string Description { get; set; }

        // ----- Các thuộc tính DB -----
        public string ImageUrl { get; set; }      // hình đại diện từ ProductImages
        public string CategoryName { get; set; }  // tên danh mục từ Categories

        // ----- Các thuộc tính phụ nếu dùng trong ProductDetail -----
        public List<string> Storage { get; set; } = new List<string>();

        public List<string> Colors { get; set; } = new List<string>();

        public List<string> ImageUrls { get; set; } = new List<string>();

        
    }
}