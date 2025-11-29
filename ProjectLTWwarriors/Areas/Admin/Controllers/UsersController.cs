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
    public class UsersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Users
        public ActionResult Index(string search)
        {
            var admins = db.Users.Where(u => u.Role == "admin");

            if (!string.IsNullOrEmpty(search))
            {
                admins = admins.Where(u => u.Username.Contains(search)
                                        || u.Email.Contains(search));
            }

            return View(admins.ToList());
        }

        // GET: Admin/Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // GET: Admin/Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,Password")] Users users)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng username
                if (db.Users.Any(u => u.Username == users.Username))
                {
                    ViewBag.Error = "Username đã tồn tại.";
                    return View(users);
                }

                users.Role = "admin";          // tự set
                users.CreatedAt = DateTime.Now;

                db.Users.Add(users);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(users);
        }



        // GET: Admin/Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int Id, string NewPassword, string ConfirmPassword)
        {
            var user = db.Users.Find(Id);
            if (user == null || user.Role != "admin")
                return HttpNotFound();

            // check xác nhận mật khẩu
            if (NewPassword != ConfirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View(user); // trả lại view cùng user để hiện lỗi
            }

            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                user.Password = NewPassword;  
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }



        // GET: Admin/Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Users users = db.Users.Find(id);
            db.Users.Remove(users);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
