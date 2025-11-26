using System.IO;
using System.Linq;
using PagedList;
using PagedList.Mvc;
using ProjectLTWwarriors.Models;
using ProjectLTWwarriors.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ProjectLTWwarriors.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Products
        public ActionResult Index(string searchTerm, decimal? minPrice, decimal? maxPrice, string sortOrder, int? page)
        {
            var model = new ProductSearchVm();
            var products = db.Products.AsQueryable();

            // Chỉ lấy sản phẩm đang active (không lấy đã ẩn)
            products = products.Where(p => p.Status == "active");

            // Tìm kiếm theo từ khóa (tên sp, mô tả, tên danh mục)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm) ||
                    p.Categories.Name.Contains(searchTerm)   // nếu navigation là Categories
                );
            }

            // Tìm kiếm sản phẩm theo giá tối thiểu
            if (minPrice.HasValue)
            {
                model.MinPrice = minPrice.Value;
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            // Tìm kiếm sản phẩm theo giá tối đa
            if (maxPrice.HasValue)
            {
                model.MaxPrice = maxPrice.Value;
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "name_asc":
                    products = products.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }
            model.SortOrder = sortOrder;

            // Phân trang
            int pageNumber = page ?? 1;  // trang hiện tại
            int pageSize = 5;            // số sản phẩm / trang (tùy bạn)

            model.Products = products.ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Price,Description,CategoryId")] Products products,string imageUrl)   // <-- trùng với name="imageUrl" trong view
        {
            if (ModelState.IsValid)
            {
                // 1. Set thêm field mặc định
                products.CreatedAt = DateTime.Now;
                products.Status = "active";

                // 2. Lưu sản phẩm trước
                db.Products.Add(products);
                db.SaveChanges();      // sau dòng này products.Id đã có giá trị

                // 3. Nếu có nhập link hình thì lưu ProductImages
                if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    var img = new ProductImages
                    {
                        ProductId = products.Id,
                        ImageUrl = imageUrl,   // ví dụ: https://... hoặc ~/images_LandingPage/xxx.png
                        IsPrimary = true
                    };

                    db.ProductImages.Add(img);
                    db.SaveChanges();
                }

                TempData["Success"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("Index");
            }

            // Nếu ModelState lỗi: set lại dropdown rồi trả về view
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", products.CategoryId);
            return View(products);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }

            //ViewBag dùng làm túi lưu trữ tạm thời mà controller gửi sang View mà không cần tạo ViewModel
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", products.CategoryId);
            return View(products);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Price,Description,CategoryId,Status,CreatedAt")] Products products)
        {
            if (db.Products.Any(p => p.Name == products.Name
                                  && p.CategoryId == products.CategoryId
                                  && p.Id != products.Id
                                  && p.Status == "active"))
            {
                ModelState.AddModelError("Name", "Sản phẩm cùng danh mục đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                db.Entry(products).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index");
            }

            //ViewBag dùng làm túi lưu trữ tạm thời mà controller gửi sang View mà không cần tạo ViewModel
            //lấy toàn bộ danh mục từ database -> chuyển dữ liệu trong danh sách xuống dropdown 
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", products.CategoryId);
            return View(products);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Không xóa cứng, chỉ ẩn sản phẩm
            product.Status = "inactive"; // ẩn sản phẩm

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
