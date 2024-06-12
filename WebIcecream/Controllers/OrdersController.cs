using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebIcecream.DTOs;
using WebIcecream.Models;

namespace WebIcecream.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ProjectDak3Context _context;

        public OrdersController(ProjectDak3Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            return await _context.Orders
                .Select(order => new OrderDTO
                {
                    OrderId = order.OrderId,
                    UserId = order.UserId,
                    Username = order.Username,
                    Email = order.Email,
                    PhoneNumber = order.PhoneNumber,
                    ShippingAddress = order.ShippingAddress,
                    BookId = order.BookId,
                    Cost = order.Cost,
                    Price = order.Price
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            var orderDTO = new OrderDTO
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                Username = order.Username,
                Email = order.Email,
                PhoneNumber = order.PhoneNumber,
                ShippingAddress = order.ShippingAddress,
                BookId = order.BookId,
                Cost = order.Cost,
                Price = order.Price
            };

            return orderDTO;
        }

        [HttpPost]
        public async Task<ActionResult<OrderDTO>> PostOrder(OrderDTO orderDTO)
        {
            var order = new Order
            {
                UserId = orderDTO.UserId,
                Username = orderDTO.Username,
                Email = orderDTO.Email,
                PhoneNumber = orderDTO.PhoneNumber,
                ShippingAddress = orderDTO.ShippingAddress,
                BookId = orderDTO.BookId,
                Cost = orderDTO.Cost,
                Price = orderDTO.Price
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            orderDTO.OrderId = order.OrderId;

            return CreatedAtAction(nameof(GetOrder), new { id = orderDTO.OrderId }, orderDTO);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, OrderDTO orderDTO)
        {
            if (id != orderDTO.OrderId)
            {
                return BadRequest();
            }

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            order.UserId = orderDTO.UserId;
            order.Username = orderDTO.Username;
            order.Email = orderDTO.Email;
            order.PhoneNumber = orderDTO.PhoneNumber;
            order.ShippingAddress = orderDTO.ShippingAddress;
            order.BookId = orderDTO.BookId;
            order.Cost = orderDTO.Cost;
            order.Price = orderDTO.Price;

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersByUserId(int userId)
        {
            var orders = await _context.Orders
                .Where(order => order.UserId == userId)
                .Select(order => new OrderDTO
                {
                    OrderId = order.OrderId,
                    UserId = order.UserId,
                    Username = order.Username,
                    Email = order.Email,
                    PhoneNumber = order.PhoneNumber,
                    ShippingAddress = order.ShippingAddress,
                    BookId = order.BookId,
                    Cost = order.Cost,
                    Price = order.Price
                })
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound();
            }

            return orders;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> SearchOrdersByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name parameter is required.");
            }

            var orders = await _context.Orders
                .Where(b => b.Username.Contains(name))
                .Select(b => new OrderDTO
                {
                    OrderId = b.OrderId,
                    UserId = b.UserId,
                    Username = b.Username,
                    Email = b.Email,
                    PhoneNumber = b.PhoneNumber,
                    ShippingAddress = b.ShippingAddress,
                    BookId = b.BookId,
                    Cost = b.Cost,
                    Price = b.Price
                })
                .ToListAsync();

            if (orders == null || orders.Count == 0)
            {
                return NotFound("No books found with the provided name.");
            }

            return Ok(orders);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
