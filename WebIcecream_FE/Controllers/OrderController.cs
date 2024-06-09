using Microsoft.AspNetCore.Mvc;
using WebIcecream.Models;

namespace WebIcecream_FE.Controllers
{
    public class OrderController : Controller
    {
        private readonly ProjectDak3Context _context;

        public OrderController(ProjectDak3Context context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                // Lưu thông tin đơn hàng vào cơ sở dữ liệu
                _context.Orders.Add(order);
                _context.SaveChanges();

                // Điều hướng đến phương thức Payment với thông tin đơn hàng
                return RedirectToAction("Payment", "Home", new { amount = order.Cost, infor = order.User, orderinfor = order.OrderId });
            }

            // Nếu dữ liệu không hợp lệ, hiển thị lại form
            return View(order);
        }
    }
}
