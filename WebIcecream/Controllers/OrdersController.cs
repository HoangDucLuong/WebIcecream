using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebIcecream.DTOs;

namespace WebIcecream.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private static List<OrderDTO> _orders = new List<OrderDTO>(); // In-memory store for orders

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
            orderDto.OrderId = _orders.Any() ? _orders.Max(o => o.OrderId) + 1 : 1;
            orderDto.OrderStatus = "Pending"; // Default status, could be "Received", "Processing", "Shipped", etc.
            _orders.Add(orderDto);
            return CreatedAtAction(nameof(GetOrderById), new { id = orderDto.OrderId }, orderDto);
        }

        // PUT: api/orders/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, [FromBody] OrderDTO orderDto)
        {
            var index = _orders.FindIndex(o => o.OrderId == id);
            if (index < 0)
            {
                return NotFound();
            }

            orderDto.OrderId = id; // Ensure the ID is correct
            _orders[index] = orderDto;
            return NoContent();
        }

        // DELETE: api/orders/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var index = _orders.FindIndex(o => o.OrderId == id);
            if (index < 0)
            {
                return NotFound();
            }

            _orders.RemoveAt(index);
            return NoContent();
        }
    }
}
