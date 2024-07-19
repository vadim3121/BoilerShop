using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class OrderItemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderItemController(ApplicationDbContext context) => _context = context;

        [HttpGet("GetOrderItems")]
        public ActionResult<IEnumerable<OrderItem>> GetOrderItems()
        {
            var orderItems = _context.OrderItems.Include(s => s.Order).Include(s => s.Boiler).ToList();
            if (!orderItems.Any()) return BadRequest("В списке нет объектов заказа");
            return Ok(orderItems);
        }

        [HttpPost("AddOrderItem")]
        public IActionResult AddOrderItem(long orderId, long boilerId, int quantity, bool isPickedUp)
        {
            var boiler = _context.Boilers.Find(boilerId);
            if (boiler == null) return BadRequest("Газовый котел не найден");

            OrderItem orderItem = new OrderItem
            {
                OrderId = orderId,
                BoilerId = boilerId,
                Quantity = quantity,
                Price = boiler.Price * quantity,
                IsPickedUp = isPickedUp
            };

            var existingOrderItem = _context.OrderItems.FirstOrDefault(s => s.OrderId == orderItem.OrderId
                                                                      && s.BoilerId == orderItem.BoilerId
                                                                      && s.Quantity == orderItem.Quantity
                                                                      && s.Price == orderItem.Price
                                                                      && s.IsPickedUp == orderItem.IsPickedUp);

            if (existingOrderItem != null) return BadRequest($"Заказ уже существует");

            _context.OrderItems.Add(orderItem);
            _context.SaveChanges();

            return Ok($"Заказ успешно добавлен, его id: {orderItem.Id}");
        }

        [HttpDelete("DeleteOrderItem")]
        public IActionResult DeleteOrderItem(long id)
        {
            var orderItem = _context.OrderItems.Find(id);

            if (orderItem == null) return BadRequest($"Нет заказа с id: {id}");

            if (orderItem.IsPickedUp) return BadRequest($"Не возможно удалить забранный товар");

            _context.OrderItems.Remove(orderItem);
            _context.SaveChanges();

            return Ok($"Заказ c id: {id} успешно удален");
        }

        [HttpPut("UpdateOrderItem")]
        public IActionResult UpdateOrderItem(long orderItemId, long? orderId = null, long? boilerId = null, int? quantity = null, bool? isPickedUp = null)
        {
            var findOrderItem = _context.OrderItems.Find(orderItemId);

            if (findOrderItem == null) return BadRequest($"Заказ с id: {orderItemId} не найден");

            if (orderId != null) findOrderItem.OrderId = orderId.Value;
            
            if (boilerId != null) findOrderItem.BoilerId = boilerId.Value;
            
            if (quantity != null) findOrderItem.Quantity = quantity.Value;
            
            if (boilerId != null || quantity != null)
            {
                var boiler = _context.Boilers.Find(findOrderItem.BoilerId);
                if (boiler != null)
                {
                    findOrderItem.Price = boiler.Price * findOrderItem.Quantity;
                }
            }
            if (isPickedUp != null)
            {
                findOrderItem.IsPickedUp = isPickedUp.Value;
            }

            _context.OrderItems.Update(findOrderItem);
            _context.SaveChanges();

            return Ok($"Заказ с id: {orderItemId} успешно обновлен");
        }
    }
}
