using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PickupPointController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PickupPointController(ApplicationDbContext context) => _context = context;

        [HttpGet("GetPickupPoints")]
        public ActionResult<IEnumerable<PickupPoint>> GetPickupPoints()
        {
            var pickupPoints = _context.PickupPoints.ToList();
            if (!pickupPoints.Any()) return BadRequest("В списке нет пунктов выдачи");
            return Ok(pickupPoints);
        }

        [HttpPost("AddPickupPoint")]
        public IActionResult AddPickupPoint(string city, string address)
        {
            PickupPoint pickup = new PickupPoint();
            pickup.City = city;
            pickup.Address = address;

            var existingPickup = _context.PickupPoints.FirstOrDefault(b => b.City == city && b.Address == address);

            if (existingPickup != null) return BadRequest($"Пункт выдачи в горове: {city} по адресу {address} уже существует");

            _context.PickupPoints.Add(pickup);
            _context.SaveChanges();

            return Ok($"Пункт выдачи успешно создан, его id: {pickup.Id}");
        }

        [HttpDelete("DeletePickupPoint")]
        public IActionResult DeletePickupPoint(long id)
        {
            var pickup = _context.PickupPoints.Find(id);

            if (pickup == null)
            {
                return BadRequest($"Нет пункта выдачи с id: {id}");
            }

            try
            {
                _context.PickupPoints.Remove(pickup);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Невозможно удалить пункт выдачи с id: {id}, так как она связана с другими объектами");
            }

            return Ok($"Пункт выдачи c id: {id} успешно удален");
        }

        [HttpPut("UpdateDeletePickupPoint")]
        public IActionResult UpdateDeletePickupPoint(long pickupId, string? city = null, string? address = null)
        {
            var findPickup = _context.PickupPoints.Find(pickupId);

            if (findPickup == null) return NotFound($"Пункт выдачи c id: {pickupId} не найден");
            if (city != null) findPickup.City = city;
            if (address != null) findPickup.Address = address;
            

            _context.PickupPoints.Update(findPickup);
            _context.SaveChanges();

            return Ok($"Пункт выдачи с id: {pickupId} успешно обновлен");
        }
    }
}
