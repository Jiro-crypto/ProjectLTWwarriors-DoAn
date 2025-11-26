using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectLTWwarriors.Models
{
    public class ProductsMetadata
    {
        [Required(ErrorMessage = "Tên sản phẩm bắt buộc nhập")]
        [StringLength(200, ErrorMessage = "Tên tối đa 200 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Giá bắt buộc nhập")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải >= 0")]
        public decimal? Price { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Danh mục bắt buộc chọn")]
        public int? CategoryId { get; set; }
    }

    [MetadataType(typeof(ProductsMetadata))]
    public partial class Products
    {
        // để trống – EF sinh code ở file khác
    }
}