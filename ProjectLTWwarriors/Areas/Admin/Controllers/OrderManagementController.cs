using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ProjectLTWwarriors.Models;

namespace ProjectLTWwarriors.Areas.Admin.Controllers
{
    public class OrderManagementController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/OrderManagement
        // Hiện tất cả đơn đặt hàng của khách
        public ActionResult Index(string search)
        {
            var orders = db.Orders
                           .Include(o => o.Users) // lấy luôn thông tin user
                           .OrderByDescending(o => o.CreatedAt);

            if (!string.IsNullOrEmpty(search))
            {
                 orders = orders.Where(o =>
                                o.MaDon.Contains(search) ||
                                o.Users.FullName.Contains(search) ||
                                o.Users.Username.Contains(search)
                 )
                                .OrderByDescending(o => o.CreatedAt);
            }

            return View(orders.ToList());
        }

        // GET: Admin/OrderManagement/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var order = db.Orders
                          .Include(o => o.Users)
                          .Include(o => o.OrderItems.Select(oi => oi.Products))
                          .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return HttpNotFound();

            return View(order);   // Model = Orders
        }
    }
}
