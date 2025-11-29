using ProjectLTWwarriors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ProjectLTWwarriors.Areas.Admin.Controllers
{
    public class AccountCustomerController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        // GET: Admin/AccountCustomer
        //Danh sách khách hàng (Role = user)
        public ActionResult Index(string search)
        {
            var customers = db.Users.Where(u => u.Role == "user");
            if (!string.IsNullOrEmpty(search))
            {
                customers = customers.Where(u => u.Username.Contains(search) ||
                                                 u.FullName.Contains(search) ||
                                                 u.Email.Contains(search));
            }
            return View(customers.ToList());
        }

        //GET: Admin/AccountCustomer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.Users.Find(id);
            if (user ==null || user.Role != "user")
                return HttpNotFound();

            //Lấy danh sách đơn hàng của khách hàng
            var orders = db.Orders
                        .Where(o => o.UserId == user.Id)
                        .OrderByDescending(o => o.CreatedAt)
                        .ToList();

            ViewBag.OrdersCount = orders.Count();
            ViewBag.Orders = orders;

            return View(user);
        }
    }
}