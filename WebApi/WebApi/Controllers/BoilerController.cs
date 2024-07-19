using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BoilerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BoilerController(ApplicationDbContext context) => _context = context;

        [HttpGet("GetBoilers")]
        public ActionResult<IEnumerable<Boiler>> GetBoilers()
        {
            var boiler = _context.Boilers.Include(s => s.Brand).Include(s => s.Category).ToList();
            if (!boiler.Any()) return BadRequest("В списке нет обуви");
            return Ok(boiler);
        }

        [HttpPost("AddBoiler")]
        public IActionResult AddBoiler(string name, int price, int size, string color, int quantity, long brandId, long categoryId)
        {
            var errors = new List<string>();

            var brandExists = _context.Brands.Any(b => b.Id == brandId);
            if (!brandExists) errors.Add($"Нет бренда с id: {brandId}");

            var categoryExists = _context.Categories.Any(c => c.Id == categoryId);
            if (!categoryExists) errors.Add($"Нет категории с id: {categoryId}");

            if (errors.Any()) return BadRequest(string.Join(" и ", errors));

            Boiler boiler = new Boiler();
            boiler.Name = name;
            boiler.Price = price;
            boiler.Size = size;
            boiler.Color = color;
            boiler.Quantity = quantity;
            boiler.BrandId = brandId;
            boiler.CategoryId = categoryId;

            var existingBoiler = _context.Boilers.FirstOrDefault(s => s.Name == boiler.Name 
                                                            && s.Price == boiler.Price
                                                            && s.Size == boiler.Size
                                                            && s.Color == boiler.Color
                                                            && s.Quantity == boiler.Quantity);

            if (existingBoiler != null) return BadRequest($"Газовый котел с названием {name} уже существует");
            
            _context.Boilers.Add(boiler);
            _context.SaveChanges();

            return Ok($"Газовый котел успешно добавлен, ее id: {boiler.Id}");
        }

        [HttpDelete("DeleteBoiler")]
        public IActionResult DeleteBoiler(long id)
        {
            var boiler = _context.Boilers.Find(id);

            if (boiler == null) return BadRequest($"Нет газового оборудования с id: {id}");

            try
            {
                _context.Boilers.Remove(boiler);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Невозможно удалить газовое оборудование с id: {id}, так как оно связано с другими объектами");
            }

            return Ok($"Газовый котел с id: {id} успешно удален");
        }

        [HttpPut("UpdateBoiler")]
        public IActionResult UpdateBoiler(long boilerId, string? name = null, int? price = null, int? size = null, string color =   null, int? quantity = null, 
            long? brandId = null, long? categoryId = null)
        {
            bool[] flags = new bool[2] { false, false };
            
            var findBoiler = _context.Boilers.Find(boilerId);

            if (findBoiler == null) return BadRequest($"Газовый котел c id: {boilerId} не найдена");
            
            if (name != null) findBoiler.Name = name;
            if (price != null) findBoiler.Price = price.Value;
            if (size != null) findBoiler.Size = size.Value;
            if (color != null) findBoiler.Color = color;
            if (quantity != null) findBoiler.Quantity = quantity.Value;
            if (brandId != null)
            {
                var brandExists = _context.Brands.Any(b => b.Id == brandId);
                if (!brandExists) return BadRequest($"Нет бренда с id: {brandId}");
                else findBoiler.BrandId = brandId.Value;
                
            }
            if (categoryId != null)
            {
                var categoryExists = _context.Categories.Any(c => c.Id == categoryId);
                if (!categoryExists) return BadRequest($"Нет категории с id: {categoryId}");
                else findBoiler.CategoryId = categoryId.Value;
            }

            _context.Boilers.Update(findBoiler);
            _context.SaveChanges();

            return Ok($"Газовый котел с id: {boilerId} успешно добавлен");
        }
    }
}
