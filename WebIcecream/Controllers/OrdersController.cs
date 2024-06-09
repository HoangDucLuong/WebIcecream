using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebIcecream.DTOs;

namespace WebIcecream.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private static List<OrderDTO> _orders = new List<OrderDTO>(); 

        // GET: api/orders
        [HttpGet]
        public ActionResult<IEnumerable<OrderDTO>> GetAllOrders()
        {
            return Ok(_orders);
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public ActionResult<OrderDTO> GetOrderById(int id)
        {
            var order = _orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // POST: api/orders
        [HttpPost]
        public ActionResult<OrderDTO> CreateOrder([FromBody] OrderDTO orderDto)
        {
            // In a real scenario, you would typically validate the orderDto before proceeding.
            // Here, we assume the input is valid for simplicity.

            orderDto.OrderId = _orders.Any() ? _orders.Max(o => o.OrderId) + 1 : 1;
            _orders.Add(orderDto);

            return CreatedAtAction(nameof(GetOrderById), new { id = orderDto.OrderId }, orderDto);
        }

        // PUT: api/orders/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, [FromBody] OrderDTO orderDto)
        {
            var existingOrder = _orders.FirstOrDefault(o => o.OrderId == id);
            if (existingOrder == null)
            {
                return NotFound();
            }

            // Update the fields of existingOrder with data from orderDto
            existingOrder.UserId = orderDto.UserId;
            existingOrder.Username = orderDto.Username;
            existingOrder.Email = orderDto.Email;
            existingOrder.PhoneNumber = orderDto.PhoneNumber;
            existingOrder.ShippingAddress = orderDto.ShippingAddress;
            existingOrder.BookId = orderDto.BookId;
            existingOrder.Cost = orderDto.Cost;
            
            existingOrder.Price = orderDto.Price;

            return NoContent();
        }

        // DELETE: api/orders/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var existingOrder = _orders.FirstOrDefault(o => o.OrderId == id);
            if (existingOrder == null)
            {
                return NotFound();
            }

            _orders.Remove(existingOrder);
            return NoContent();
        }
    }
}
