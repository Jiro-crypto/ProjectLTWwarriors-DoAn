using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectLTWwarriors.Models;

namespace ProjectLTWwarriors.Areas.Admin.Controllers
{
    public class CategoriesController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Categories
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        // GET: Admin/Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categories categories = db.Categories.Find(id);
            if (categories == null)
            {
                return HttpNotFound();
            }
            return View(categories);
        }

        // GET: Admin/Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Categories categories)
        {
            // Kiểm tra trùng tên
            if (db.Categories.Any(c => c.Name == categories.Name))
            {
                ModelState.AddModelError("Name", "Tên danh mục đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                db.Categories.Add(categories);
                db.SaveChanges();
                TempData["Success"] = "Thêm danh mục thành công!";
                return RedirectToAction("Index");
            }

            return View(categories);
        }
        // GET: Admin/Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categories categories = db.Categories.Find(id);
            if (categories == null)
            {
                return HttpNotFound();
            }
            return View(categories);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Categories categories)
        {
            // Kiểm tra trùng tên nhưng bỏ qua chính nó
            if (db.Categories.Any(c => c.Name == categories.Name && c.Id != categories.Id))
            {
                ModelState.AddModelError("Name", "Tên danh mục đã tồn tại.");
            }

            // Check: danh mục này có sản phẩm nào không?
            bool hasProducts = db.Products.Any(p => p.CategoryId == categories.Id && p.Status == "active"); // đổi CategoryId nếu tên khác

            if (hasProducts)
            {
                // Lỗi không gán field cụ thể -> hiện ở ValidationSummary
                ModelState.AddModelError("", "Không thể sửa danh mục này vì đang có sản phẩm thuộc về nó.");
            }

            if (ModelState.IsValid)
            {
                db.Entry(categories).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Cập nhật danh mục thành công!";
                return RedirectToAction("Index");
            }

            // Nếu có lỗi -> quay lại view, hiện ValidationSummary
            return View(categories);
        }


        // GET: Admin/Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categories categories = db.Categories.Find(id);
            if (categories == null)
            {
                return HttpNotFound();
            }
            return View(categories);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var category = db.Categories.Find(id);

            if (category == null)
            {
                return HttpNotFound();
            }

            // Check: có sản phẩm nào đang dùng danh mục này không?
            bool hasProducts = db.Products.Any(p => p.CategoryId == id); // chỉnh CategoryId đúng tên property

            if (hasProducts)
            {
                // Không cho xoá, dùng TempData để báo lỗi ở trang Index
                TempData["Error"] = "Không thể xóa danh mục này vì đang có sản phẩm thuộc về nó. " +
                                    "Vui lòng xóa hoặc chuyển danh mục cho sản phẩm trước.";
                return RedirectToAction("Index");
            }

            // Không có sản phẩm -> xoá bình thường
            db.Categories.Remove(category);
            db.SaveChanges();
            TempData["Success"] = "Xóa danh mục thành công!";
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
