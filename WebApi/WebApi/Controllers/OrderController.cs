using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class OrderController : Controller
    {
        public readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context) => _context = context;

        [HttpGet("GetOrders")]
        public ActionResult<IEnumerable<Order>> GetOrders()
        {
            var orders = _context.Orders.Include(s => s.Client).Include(s => s.PickupPoint).ToList();
            if (!orders.Any()) return BadRequest("В списке нет заказов");
            return Ok(orders);
        }

        [HttpPost("AddOrder")]
        public IActionResult AddOrder(long clientId, DateOnly orderDate, long pickupPointId)
        {
            Order order = new Order();
            order.ClientId = clientId;
            order.OrderDate = orderDate;
            order.PickupPointId = pickupPointId;

            var existingOrder = _context.Orders.FirstOrDefault(s => s.ClientId == clientId
                                                              && s.OrderDate == orderDate
                                                              && s.PickupPointId == pickupPointId);

            if (existingOrder != null) return BadRequest($"Заказ с id: {order.Id} уже существует");

            _context.Orders.Add(order);
            _context.SaveChanges();

            return Ok($"Заказ успешно с id: {order.Id} успешно добавлен");
        }

        [HttpDelete("DeleteOrder")]
        public IActionResult DeleteOrder(long id)
        {
            var order = _context.Orders.Find(id);

            if (order == null) return BadRequest($"Нет заказа с id: {id}");

            try
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Невозможно удалить заказ с id: {id}, так как она связана с другими объектами");
            }

            return Ok($"Заказ c id: {id} успешно удален");
        }

        [HttpPut("UpdateOrder")]
        public IActionResult UpdateOrder(long orderId, long? clientId = null, DateOnly? orderDate = null, long? pickupPointId = null)
        {
            var findOrder = _context.Orders.Find(orderId);

            if (findOrder == null) return BadRequest($"Заказ c id: {orderId} не найден");

            if (clientId != null) findOrder.ClientId = clientId.Value;
            if (pickupPointId != null) findOrder.PickupPointId = pickupPointId.Value;
            if (orderDate != null) findOrder.OrderDate = orderDate.Value;

            _context.Orders.Update(findOrder);
            _context.SaveChanges();

            return Ok($"Заказ с id: {orderId} успешно обновлен");
        }
    }
}
