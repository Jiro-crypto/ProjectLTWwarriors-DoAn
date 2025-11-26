using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectLTWwarriors.Models
{
    public class CategoriesMetadata
    {
        [Required(ErrorMessage = "Tên danh mục bắt buộc nhập")]
        [StringLength(100, ErrorMessage = "Tên danh mục tối đa 100 ký tự")]
        public string Name { get; set; }
    }

    // Gán metadata cho entity Categories (trùng tên với class do EF sinh ra)
    [MetadataType(typeof(CategoriesMetadata))]
    public partial class Categories
    {
        // không cần viết gì thêm ở đây
    }
}