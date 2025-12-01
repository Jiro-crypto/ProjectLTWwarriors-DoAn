using PagedList;
using ProjectLTWwarriors.Data;
using ProjectLTWwarriors.Models;
using ProjectLTWwarriors.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace ProjectLTWwarriors.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyStoreEntities _db = new MyStoreEntities();
        // Hàm này sẽ tự động chạy trước mọi Action trong controller
        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            // Gọi hàm đếm và truyền số lượng sản phẩm vào ViewBag để hiển thị trên giao diện
            ViewBag.SoLuongTrongGio = DemSoLuongTrongGioHang();
            // Truyền user hiện tại cho mọi view
            ViewBag.CurrentUser = Session["CurrentUser"] as Users;
            base.OnActionExecuting(context);
        }

        public ActionResult Index()
        {
            var products = ProductData.GetAllProducts();  // lấy danh sách sản phẩm
            return View(products);
        }

        public ActionResult LandingPage(string searchTerm, int? page)
        {
            var model = new HomeProductVM();

            var products = _db.Products
                              .Include(p => p.Categories)
                              .Include(p => p.ProductImages)
                              .Include(p => p.OrderItems)
                              .Where(p => p.Status == "active")
                              .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm) ||
                    (p.Categories != null && p.Categories.Name.Contains(searchTerm))
                );
            }

            int pageNumber = page ?? 1;

            // Full list để hiển thị theo danh mục
            model.NewProducts = products
                                .OrderByDescending(p => p.CreatedAt)
                                .ToList();

            // Phân trang
            model.NewProductsPaged = products
                                      .OrderByDescending(p => p.CreatedAt)
                                      .ToPagedList(pageNumber, model.PageSize);

            model.PageNumber = pageNumber;

            return View(model);
        }

        public ActionResult SignUp()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult SignUp(
    string Ho,
    string Ten,
    int Ngay,
    int Thang,
    int Nam,
    string Email,
    string MatKhau,
    string XacNhanMatKhau,
    string Phone, 
    string TinhThanh, 
    string PhuongXa, 
    string DiaChi)
        {
            // 1. Kiểm tra mật khẩu xác nhận
            if (MatKhau != XacNhanMatKhau)
            {
                ViewBag.Error = "Mật khẩu xác nhận không đúng.";
                return View();
            }

            // 2. Check email trùng
            var normEmail = (Email ?? "").Trim().ToLowerInvariant();
            var existed = _db.Users.FirstOrDefault(u => u.Email.ToLower() == normEmail);
            if (existed != null)
            {
                ViewBag.Error = "Email này đã được dùng.";
                return View();
            }

            // 3. Validate số điện thoại
            var phone = (Phone ?? "").Trim();
            if (!IsValidPhone(phone))
            {
                ViewBag.Error = "Số điện thoại phải bắt đầu bằng 0, gồm 10 chữ số và không được toàn một số giống nhau.";
                return View();
            }

            // 3. Convert ngày sinh
            DateTime ngaySinh;
            try
            {
                ngaySinh = new DateTime(Nam, Thang, Ngay);
            }
            catch
            {
                ViewBag.Error = "Ngày sinh không hợp lệ.";
                return View();
            }

            // 4. Tạo user mới (GÁN ĐẦY ĐỦ 5 TRƯỜNG REQUIRED)
            var user = new Users
            {
                Username = normEmail,                   // BẮT BUỘC
                FullName = (Ho + " " + Ten).Trim(),     // BẮT BUỘC
                Email = normEmail,                      // BẮT BUỘC
                Password = MatKhau,                     // BẮT BUỘC
                NgaySinh = ngaySinh,
                Phone = phone,
                TinhThanh = TinhThanh,
                PhuongXa = PhuongXa,
                DiaChi = DiaChi,
                CreatedAt = DateTime.Now,               // BẮT BUỘC
                Role = "user"
            };

            _db.Users.Add(user);
            _db.SaveChanges();   // Lúc này user.Id mới có

            // 5. Đăng nhập luôn sau đăng ký
            Session["CurrentUser"] = user;
            Session["CurrentUserName"] = user.FullName;
            Session["CurrentUserId"] = user.Id;

            return RedirectToAction("WelcomeBack");
        }




        public ActionResult SignIn()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult iPhone()
        {
            // Lấy tất cả sản phẩm từ DB (đã map sẵn trong ProductData)
            var allProducts = ProductData.GetAllProducts();

            // Chỉ lấy những sản phẩm thuộc Category "iPhone"
            var iphone = _db.Products
                            .Include(p => p.ProductImages)
                            .Where(p => p.Status == "active" && // chỉ lấy những sản phẩm còn active
                                        p.Categories.Name == "iPhone") //lấy sản phẩm Iphone trong catagory
                            .ToList();

            return View(iphone);   // truyền list sản phẩm qua View
        }

        public ActionResult Mac()
        {

            // Lấy tất cả sản phẩm từ DB (đã map sẵn trong ProductData)
            var allProducts = ProductData.GetAllProducts();

            // Chỉ lấy những sản phẩm thuộc Category "Mac"
            var mac = _db.Products
                            .Include(p => p.ProductImages)
                            .Where(p => p.Status == "active" && // chỉ lấy những sản phẩm còn active
                                        p.Categories.Name == "Mac") //lấy sản phẩm Mac trong catagory
                            .ToList();

            return View(mac);   // truyền list sản phẩm qua View
        }

        public ActionResult iPad()
        {
            // Lấy tất cả sản phẩm từ DB (đã map sẵn trong ProductData)
            var allProducts = ProductData.GetAllProducts();

            // Chỉ lấy những sản phẩm thuộc Category "iPad"
            var ipad = _db.Products
                            .Include(p => p.ProductImages)
                            .Where(p => p.Status == "active" && // chỉ lấy những sản phẩm còn active
                                        p.Categories.Name == "iPad") //lấy sản phẩm iPad trong catagory
                            .ToList();

            return View(ipad);   // truyền list sản phẩm qua View
        }

        public ActionResult Watch()
        {
            // Lấy tất cả sản phẩm từ DB (đã map sẵn trong ProductData)
            var allProducts = ProductData.GetAllProducts();

            // Chỉ lấy những sản phẩm thuộc Category "Watch"
            var watch = _db.Products
                            .Include(p => p.ProductImages)
                            .Where(p => p.Status == "active" && // chỉ lấy những sản phẩm còn active
                                        p.Categories.Name == "Watch") //lấy sản phẩm Watch trong catagory
                            .ToList();

            return View(watch);   // truyền list sản phẩm qua View
        }

        public ActionResult TaiNghe_Loa()
        {
            // Lấy tất cả sản phẩm từ DB (đã map sẵn trong ProductData)
            var allProducts = ProductData.GetAllProducts();

            // Chỉ lấy những sản phẩm thuộc Category "TaiNghe_Loa"
            var tainghe_loa = _db.Products
                            .Include(p => p.ProductImages)
                            .Where(p => p.Status == "active" && // chỉ lấy những sản phẩm còn active
                                        p.Categories.Name == "TaiNghe_Loa") //lấy sản phẩm TaINghe_Loa trong catagory
                            .ToList();

            return View(tainghe_loa);   // truyền list sản phẩm qua View
        }

        public ActionResult PhuKien()
        {
            // Lấy tất cả sản phẩm từ DB (đã map sẵn trong ProductData)
            var allProducts = ProductData.GetAllProducts();

            // Chỉ lấy những sản phẩm thuộc Category "PhuKien"
            var phukien = _db.Products
                            .Include(p => p.ProductImages)
                            .Where(p => p.Status == "active" && // chỉ lấy những sản phẩm còn active
                                        p.Categories.Name == "Phụ Kiện") //lấy sản phẩm PhuKien trong catagory
                            .ToList();

            return View(phukien);   // truyền list sản phẩm qua View
        }

        public ActionResult Welcome()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult WelcomeBack()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ThanhToanThanhCong(string id)
        {
            // Nếu không truyền id thì thử lấy từ TempData
            if (string.IsNullOrEmpty(id))
            {
                id = TempData["LastOrderId"] as string;
                if (string.IsNullOrEmpty(id))
                {
                    // Không có mã đơn thì quay về trang chủ
                    return RedirectToAction("Index");
                }
            }

            // Lấy đơn hàng từ DB theo MaDon
            var order = _db.Orders
                           .Include(o => o.OrderItems.Select(oi => oi.Products))
                           .FirstOrDefault(o => o.MaDon == id);

            if (order == null)
            {
                return HttpNotFound("Không tìm thấy đơn hàng");
            }

            // Truyền cả object order sang view ThanhToanThanhCong
            return View(order);
        }

        public ActionResult ThanhToan()
        {
            // yêu cầu đăng nhập
            if (GetCurrentUserId() == 0)
            {
                return RedirectToAction("SignIn", "Home");
            }

            var current = Session["CurrentUser"] as Users;
            if (current != null)
            {
                ViewBag.HoTen = current.FullName;
                ViewBag.SoDienThoai = current.Phone;
                ViewBag.TinhThanh = current.TinhThanh;
                ViewBag.PhuongXa = current.PhuongXa;
                ViewBag.DiaChi = current.DiaChi;
            }
            // 1. Lấy giỏ hàng hiện tại từ DB
            var cart = GetOrCreateCart();

            var items = _db.CartItems
                           .Where(ci => ci.CartId == cart.Id)
                           .GroupBy(ci => ci.ProductId)
                           .Select(g => new
                           {
                               ProductId = g.Key,
                               Qty = g.Sum(x => x.Qty),
                               UnitPrice = g.FirstOrDefault().UnitPrice
                           })
                           .ToList();

            // Nếu không còn gì trong giỏ → quay lại trang giỏ
            if (!items.Any())
            {
                return RedirectToAction("XemGioHang");
            }

            // 2. Map sang List<MatHangTrongGio> và LẤY THÊM HÌNH từ ProductImages
            var gioHang = new List<MatHangTrongGio>();

            foreach (var row in items)
            {
                var product = _db.Products
                                 .Include(p => p.ProductImages)
                                 .FirstOrDefault(p => p.Id == row.ProductId);

                if (product == null) continue;

                // danh sách ảnh của sản phẩm (ưu tiên IsPrimary)
                var imageUrls = product.ProductImages
                                       .OrderByDescending(pi => pi.IsPrimary)
                                       .Select(pi => pi.ImageUrl)
                                       .ToList();

                gioHang.Add(new MatHangTrongGio
                {
                    SanPham = new Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = row.UnitPrice,
                        ImageUrls = imageUrls
                    },
                    SoLuong = row.Qty
                });
            }

            // 3. Tính tổng tiền để hiển thị trên view ThanhToan
            decimal tongTienHang = gioHang.Sum(item => item.SanPham.Price * item.SoLuong);
            ViewBag.TongTienHang = tongTienHang;

            // 4. Trả về view ThanhToan với model là danh sách mặt hàng
            return View(gioHang);
        }




       
        public ActionResult ProductDetail(int? id)
        {


            if (id == null)
                return RedirectToAction("Index"); // hoặc trả về view thông báo lỗi

            var product = ProductData.GetAllProducts().FirstOrDefault(p => p.Id == id);
            if (product == null)
                return HttpNotFound();


            return View(product);
        }




        // Action để hiển thị trang giỏ hàng
        public ActionResult GioHangTrong()
        {
            // Lấy giỏ hàng từ Session
            var gioHang = Session["GioHang"] as List<MatHangTrongGio>;

            // Nếu giỏ hàng chưa có hoặc không có sản phẩm nào
            if (gioHang == null || gioHang.Count == 0)
            {
                // Trả về view giỏ hàng trống
                return View("GioHangTrong"); // Bạn cần tạo 1 view tên là GioHangTrong.cshtml
            }

            // Nếu có sản phẩm, trả về view với danh sách mặt hàng
            return View(gioHang);
        }




        // HÀM SỐ 1: DÙNG ĐỂ "XEM" GIỎ HÀNG
        // Đây là hàm duy nhất bạn cần để hiển thị trang giỏ hàng.
        // URL đúng để vào trang này là: /Home/XemGioHang
        public ActionResult XemGioHang()
        {
            // Nếu chưa đăng nhập thì chuyển sang trang đăng nhập
            if (GetCurrentUserId() == 0)
            {
                return RedirectToAction("SignIn", "Home");
            }


            var cart = GetOrCreateCart();

            // GOM NHÓM theo ProductId, cộng Qty lại
            var items = _db.CartItems                                  // Lấy thông tin sản phẩm có trong giỏ hàng trong bảng CartItems để đọc xong mới chuyển dữ liệu đó sang trang GioHangCoSanPham
                           .Where(ci => ci.CartId == cart.Id)
                           .GroupBy(ci => ci.ProductId)
                           .Select(g => new
                           {
                               ProductId = g.Key,
                               Qty = g.Sum(x => x.Qty),
                               UnitPrice = g.FirstOrDefault().UnitPrice
                           })
                           .ToList();

            if (!items.Any())
            {
                return View("GioHangTrong");
            }

            var model = new List<MatHangTrongGio>();

            foreach (var row in items)
            {
                // Lấy luôn ProductImages để có ảnh
                var product = _db.Products
                                 .Include(p => p.ProductImages)
                                 .FirstOrDefault(p => p.Id == row.ProductId);
                if (product == null) continue;

                // Lấy 1 ảnh đại diện cho sản phẩm trong giỏ
                var imageUrl = product.ProductImages
                                      .OrderByDescending(img => img.IsPrimary)
                                      .Select(img => img.ImageUrl)
                                      .FirstOrDefault() ?? "~/images_LandingPage/no-image.png";

                model.Add(new MatHangTrongGio
                {
                    SanPham = new Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = row.UnitPrice,
                        ImageUrl = imageUrl       //Gán hình vào trong giỏ hàng 
                    },
                    SoLuong = row.Qty
                });
            }

            return View("GioHangCoSanPham", model);
        }


        // HÀM SỐ 2: DÙNG ĐỂ "THÊM" SẢN PHẨM VÀO GIỎ
        // Tên của nó phải mô tả đúng hành động nó làm.
        // [HttpPost] là quy tắc đặc biệt: "Chỉ JavaScript mới được gọi hàm này, gõ trên trình duyệt sẽ không chạy!"
        [HttpPost]
        public ActionResult ThemVaoGio(int maSP, int soLuong)
        {
            // dòng lệnh yêu cầu việc phải đăng nhập mới làm được
            if (GetCurrentUserId() == 0)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thêm vào giỏ hàng." });
            }
            var cart = GetOrCreateCart();

            var product = _db.Products.FirstOrDefault(p => p.Id == maSP);
            if (product == null)
                return Json(new { success = false, message = "Không tìm thấy sản phẩm" });

            var item = _db.CartItems
                          .FirstOrDefault(ci => ci.CartId == cart.Id && ci.ProductId == maSP); // Tạo 1 id mới trong CartItem

            if (item == null)
            {
                // tạo mới
                item = new CartItems
                {
                    CartId = cart.Id,
                    ProductId = maSP,
                    Qty = soLuong,
                    UnitPrice = product.Price
                };
                _db.CartItems.Add(item);
            }
            else
            {
                // cập nhật số lượng
                item.Qty += soLuong;
                _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }

            cart.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges(); //Lưu vào trong database CartItems

            return Json(new { success = true, message = "Đã thêm sản phẩm vào giỏ hàng!" });
        }


        // Hàm này dùng để đếm tổng số lượng sản phẩm trong giỏ hàng
        private int DemSoLuongTrongGioHang()
        {
            int userId = GetCurrentUserId();

            var cart = _db.Carts.FirstOrDefault(c => c.UserId == userId);
            if (cart == null) return 0;
            { 
                // Nếu chưa có CartItems cho cart này thì trả về 0
                var tong = _db.CartItems
                              .Where(ci => ci.CartId == cart.Id)
                              .Select(ci => (int?)ci.Qty)
                              .Sum() ?? 0;
                return tong;
            }
        }


        [HttpPost]
        public ActionResult TangSoLuong(int maSP)
        {
            var cart = GetOrCreateCart();

            var item = _db.CartItems
                          .FirstOrDefault(ci => ci.CartId == cart.Id && ci.ProductId == maSP);
            if (item == null) return Json(new { success = false });

            item.Qty++;
            cart.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult GiamSoLuong(int maSP)
        {
            var cart = GetOrCreateCart();

            var item = _db.CartItems
                          .FirstOrDefault(ci => ci.CartId == cart.Id && ci.ProductId == maSP);

            // Có thể request bị gửi trễ, item đã bị xoá bởi lần bấm trước đó
            if (item == null)
            {
                return Json(new { success = false, message = "Sản phẩm không còn trong giỏ." });
            }

            // Nếu chỉ còn 1 cái thì xoá luôn dòng đó
            if (item.Qty <= 1)
            {
                _db.CartItems.Remove(item);
            }
            else
            {
                item.Qty--;
            }

            cart.UpdatedAt = DateTime.UtcNow;

            try
            {
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                // Không để app văng, trả JSON để JS xử lý
                return Json(new { success = false, message = "Lỗi lưu giỏ hàng: " + ex.Message });
            }

            var items = _db.CartItems.Where(ci => ci.CartId == cart.Id).ToList();
            int count = items.Sum(x => x.Qty);
            decimal tong = items.Sum(x => x.Qty * x.UnitPrice);
            bool empty = !items.Any();

            return Json(new { success = true, empty, count, tong });
        }


        [HttpPost]
        public ActionResult XoaHetGio()
        {
            // Bắt buộc đăng nhập
            if (GetCurrentUserId() == 0)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để xóa giỏ hàng." });
            }

            // Lấy giỏ hàng hiện tại
            var cart = GetOrCreateCart();

            // Lấy tất cả item thuộc giỏ này
            var items = _db.CartItems
                           .Where(ci => ci.CartId == cart.Id)
                           .ToList();

            if (items.Any())
            {
                _db.CartItems.RemoveRange(items);
                _db.SaveChanges();
            }

            return Json(new { success = true });
        }



        [HttpPost]
        public ActionResult TienHanhThanhToan(
        string HoTen,
        string SoDienThoai,
        string TinhThanh,
        string PhuongXa,
        string DiaChi,
        string GhiChu,
        string PhuongThucThanhToan,
        string VoucherCode)
        {
            //dòng này yêu cầu đăng nhập mới được sử dụng

            if (GetCurrentUserId() == 0)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thêm vào giỏ hàng." });
            }

            // Lấy giỏ hiện tại từ DB
            var cart = GetOrCreateCart();

            var items = _db.CartItems
                           .Where(ci => ci.CartId == cart.Id)
                           .ToList();

            // Nếu giỏ trống -> quay lại trang giỏ hàng
            if (!items.Any())
            {
                return RedirectToAction("XemGioHang");
            }

            // Tính tiền dựa trên CartItems
            decimal tamTinh = items.Sum(x => x.Qty * x.UnitPrice);
            decimal ship = 0;
            decimal giam = 0;
            decimal tong = Math.Max(0, tamTinh + ship - giam);

            // Tạo mã đơn
            string maDon = "DH" + DateTime.Now.ToString("yyyyMMddHHmmssfff");

            // Tạo bản ghi trong bảng orders 
            var order = new Orders
            {
                MaDon = maDon,
                CreatedAt = DateTime.Now,

                NguoiNhanTen = HoTen,
                NguoiNhanSDT = SoDienThoai,
                NguoiNhanDiaChi = $"{DiaChi}, {PhuongXa}, {TinhThanh}",

                PhuongThucThanhToan = PhuongThucThanhToan,
                TrangThai = (PhuongThucThanhToan ?? "")
                                .ToUpper()
                                .Contains("COD") ? "processing" : "paid",

                Subtotal = tamTinh,
                GiamGia = giam,
                PhiVanChuyen = ship,
                TongCong = tong,

                UserId = GetCurrentUserId()   // tạm lấy user giả, sau này sửa theo đăng nhập thật
            };

            _db.Orders.Add(order);
            _db.SaveChanges();   // để có order.Id

            // Lưu chi tiết đơn hàng vào trong OrderItems
            foreach (var ci in items)
            {
                var orderItem = new OrderItems
                {
                    OrderId = order.Id,
                    ProductId = ci.ProductId,
                    Qty = ci.Qty,
                    UnitPrice = ci.UnitPrice
                    // Nếu bảng OrderItems có cột Subtotal/ThanhTien thì gán thêm:
                    // Subtotal = ci.Qty * ci.UnitPrice
                };

                _db.OrderItems.Add(orderItem);
            }

            // Xóa giỏ hàng sau khi đặt
            _db.CartItems.RemoveRange(items);                                // Khi nhấn tiến hành dặt hàng thì sẽ tự động xóa dữ liệu trong CartItems
            _db.SaveChanges();

            // Lưu mã đơn chuyển sang trang thanh toán thành công
            TempData["LastOrderId"] = maDon;

            return RedirectToAction("ThanhToanThanhCong", new { id = maDon });
        }



        // GET: /Home/OrderDetail?id=...
        public ActionResult ChiTietDonHang(string id)
        {
            // Lấy Orders kèm OrderItems + Products
            var order = _db.Orders
                           .Include(o => o.OrderItems.Select(oi => oi.Products))
                           .FirstOrDefault(o => o.MaDon == id);

            if (order == null)
                return HttpNotFound("Không tìm thấy đơn hàng");

            // Map từ entity Orders sang view-model DonHang
            var model = new DonHang
            {
                MaDon = order.MaDon,
                CreatedAt = order.CreatedAt,
                HoTen = order.NguoiNhanTen,
                SoDienThoai = order.NguoiNhanSDT,
                DiaChiDayDu = order.NguoiNhanDiaChi,
                PhuongThucThanhToan = order.PhuongThucThanhToan,
                TrangThai = order.TrangThai,
                TamTinh = order.Subtotal,
                PhiVanChuyen = order.PhiVanChuyen,
                GiamGia = order.GiamGia,
                TongCong = order.TongCong ?? 0,
                // Nếu bạn có cột mã khuyến mãi trong Orders thì gán ở đây,
                // còn không thì để null hoặc bỏ thuộc tính trong DonHang
                MaKhuyenMai = null,

                Items = order.OrderItems.Select(oi => new MatHangTrongGio
                {
                    SanPham = new Product
                    {
                        Id = oi.ProductId,
                        Name = oi.Products.Name,
                        Price = oi.UnitPrice
                    },
                    SoLuong = oi.Qty
                }).ToList()
            };

            return View(model);   // View: Views/Home/ChiTietDonHang.cshtml
        }

        // Tạo trang để check lịch sử mua hàng
        // GET: /Home/LichSuMuaHang
        public ActionResult LichSuMuaHang()
        {
            int userId = GetCurrentUserId();

            // Lấy tất cả đơn của user hiện tại từ DB
            var orders = _db.Orders
                            .Where(o => o.UserId == userId)
                            .OrderByDescending(o => o.CreatedAt)
                            .ToList();

            // Map sang List<DonHang> cho view
            var ds = orders.Select(order => new DonHang
            {
                MaDon = order.MaDon,
                CreatedAt = order.CreatedAt,
                TrangThai = order.TrangThai,
                TongCong = order.TongCong ?? 0
            }).ToList();

            return View(ds);   
        }


        [HttpPost]
        public ActionResult SignIn(string email, string password)
        {
            var normEmail = (email ?? "").Trim().ToLowerInvariant();

            // Lấy user từ DB
            var user = _db.Users
                          .FirstOrDefault(u =>
                              u.Email.ToLower() == normEmail &&
                              u.Password == password
                          );

            if (user == null)
            {
                ViewBag.Error = "Sai email hoặc mật khẩu!";
                return View();
            }

            // Lưu user đã đăng nhập vào Session
            Session["CurrentUser"] = user;
            Session["CurrentUserName"] = user.FullName; //để hiện trên header
            Session["CurrentUserId"] = user.Id; // để tách riêng giỏ hàng ra

            return RedirectToAction("WelcomeBack");
        }

        public ActionResult Logout()
        {
            // Xóa thông tin đăng nhập
            Session["CurrentUser"] = null;
            Session["CurrentUserId"] = null;
            Session["CurrentUserName"] = null;

            return RedirectToAction("LandingPage");
        }


        // GET: /Home/Profile
        [HttpGet]
        public new ActionResult Profile()
        {
            // 1. Bắt buộc đăng nhập
            var current = Session["CurrentUser"] as Users;
            if (current == null)
                return RedirectToAction("SignIn");

            // 2. Lấy user thật trong DB
            var user = _db.Users.Find(current.Id);
            if (user == null)
                return RedirectToAction("SignIn");

            // 3. Trả về view Profile với model là Users
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public new ActionResult Profile(Users form)
        {
            // 1. Bắt buộc đăng nhập
            var current = Session["CurrentUser"] as Users;
            if (current == null)
                return RedirectToAction("SignIn");

            // 2. Lấy user thật trong DB
            var user = _db.Users.Find(current.Id);
            if (user == null)
                return RedirectToAction("SignIn");

            // 3.1 Validate của tên
            var fullName = (form.FullName ?? "").Trim(); //Trim() dùng để xóa khoảng trắng ở đầu và cuối
            if (string.IsNullOrEmpty(fullName))
            {
                ModelState.AddModelError("FullName", "Họ tên không được để trống.");
            }

            if (!ModelState.IsValid)
            {
                return View(user); // trả về thông tin cũ + error
            }

            // 3.2 Validate số điện thoại
            var phone = (form.Phone ?? "").Trim();
            if (!string.IsNullOrEmpty(phone) && !IsValidPhone(phone))
            {
                ModelState.AddModelError("Phone",
                    "Số điện thoại phải bắt đầu bằng 0, gồm 10 chữ số và không được toàn một số giống nhau.");
            }


            // 4. Ghi các field được phép sửa
            user.FullName = fullName;
            user.NgaySinh = form.NgaySinh;
            user.Phone = phone;
            user.TinhThanh = form.TinhThanh;
            user.PhuongXa = form.PhuongXa;
            user.DiaChi = form.DiaChi;

            _db.SaveChanges();

            // 5. Cập nhật lại Session
            Session["CurrentUser"] = user;
            Session["CurrentUserName"] = user.FullName;

            ViewBag.Success = "Cập nhật thông tin thành công!";
            return View(user);
        }




        // Trả về số lượng sản phẩm trong giỏ hàng (cho JavaScript cập nhật header)
        //Bấm nút “Thêm vào giỏ”, trang không reload, nên ViewBag (được render từ server) không cập nhật lại.
        // => Cách để cập nhật trong lúc đang ở trang hiện tại là dùng AJAX (fetch) => gọi hàm JSON như GetCartCount
        [HttpGet]
        public JsonResult GetCartCount()
        {
            // Lấy user hiện tại (0 = chưa đăng nhập)
            int userId = GetCurrentUserId();

            if (userId == 0)
            {
                // Chưa đăng nhập thì coi như giỏ trống
                return Json(new { count = 0 }, JsonRequestBehavior.AllowGet);
            }

            var cart = _db.Carts.FirstOrDefault(c => c.UserId == userId);
            if (cart == null)
            {
                return Json(new { count = 0 }, JsonRequestBehavior.AllowGet);
            }

            var items = _db.CartItems
                           .Where(ci => ci.CartId == cart.Id)
                           .Select(ci => (int?)ci.Qty)
                           .ToList();

            int count = items.Sum() ?? 0;

            // QUAN TRỌNG: JsonRequestBehavior.AllowGet
            return Json(new { count }, JsonRequestBehavior.AllowGet);
        }

        //Dùng để kiểm tra mật khẩu
        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;

            phone = phone.Trim();

            // Bắt đầu bằng 0, tổng 10 số, không được toàn 1 số giống nhau (vd: 0000000000)
            var regex = new Regex(@"^0(?!([0-9])\1+$)[0-9]{9}$"); //regax: regular expression dùng để kiểm tra chuỗi đúng định dạng
            return regex.IsMatch(phone);
        }


        private int GetCurrentUserId()
        {
            var user = Session["CurrentUser"] as Users;
            if (user != null)
            {
                return user.Id;  // hoặc UserId, tuỳ tên cột trong bảng Users
            }

            // Nếu chưa đăng nhập, tạm trả 0
            return 0;
        }

        private Carts GetOrCreateCart()
        {
            int userId = GetCurrentUserId();

            // Chưa đăng nhập thì không tạo giỏ
            if (userId == 0)
            {
                return null;
            }

            var cart = _db.Carts.FirstOrDefault(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Carts
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _db.Carts.Add(cart);
                _db.SaveChanges();
            }

            return cart;


        }
    }
}   