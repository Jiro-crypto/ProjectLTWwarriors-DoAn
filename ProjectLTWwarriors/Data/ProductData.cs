using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectLTWwarriors.Models;
using System.Data.Entity;

namespace ProjectLTWwarriors.Data
{
    public class ProductData
    {
        public static List<Product> GetAllProducts()
        {
            using (var db = new MyStoreEntities())
            {
                // 1. Lấy entities từ DB về bộ nhớ
                var entities = db.Products
                                 .Include(p => p.Categories)
                                 .Include(p => p.ProductImages)
                                 .ToList();   // <-- rất quan trọng

                // 2. Map sang DTO Product (Models/Product.cs)
                var result = entities.Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,

                    CategoryName = p.Categories != null ? p.Categories.Name : null,

                    // Ảnh đại diện(primary)
                    ImageUrl = p.ProductImages
                                .OrderByDescending(i => i.IsPrimary)
                                .Select(i => i.ImageUrl)
                                .FirstOrDefault() ?? "~/images_LandingPage/no-image.png",

                    // ====== QUAN TRỌNG ======
                    // Đổ toàn bộ ảnh vào ImageUrls
                    ImageUrls = p.ProductImages
                                 .OrderByDescending(i => i.IsPrimary)
                                 .Select(i => i.ImageUrl)
                                 .ToList(),

                    // Các field ProductDetail cần nhưng chưa có data — để list rỗng vẫn OK
                    Storage = new List<string>(),
                    Colors = new List<string>()
                })
                .ToList();

                return result;
            }
        }
    }
}