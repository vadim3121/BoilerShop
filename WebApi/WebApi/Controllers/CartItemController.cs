using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CartItemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartItemController(ApplicationDbContext context) => _context = context;

        [HttpGet("GetCartItems")]
        public ActionResult<IEnumerable<CartItem>> GetCartItems()
        {
            var cartItems = _context.CartItems.Include(s => s.Cart).Include(s => s.Boiler).ToList();
            if (!cartItems.Any()) return BadRequest("В списке нет объектов корзины");
            return Ok(cartItems);
        }

        [HttpPost("AddCartItem")]
        public IActionResult AddCartItem(long cartId, long boilerId, int quantity)
        {
            CartItem cartItem = new CartItem();
            cartItem.CartId = cartId;
            cartItem.BoilerId = boilerId;
            cartItem.Quantity = quantity;

            var boiler = _context.Boilers.Find(boilerId);

            if (boiler.Quantity < quantity) return BadRequest("Не достаточно газовых котлов");

            boiler.Quantity -= quantity;
            _context.CartItems.Add(cartItem);
            _context.SaveChanges();

            return Ok($"Обувь успешно добавлена в корзину, ее id: {boilerId} в количестве: {quantity}");
        }

        [HttpDelete("DeleteCartItem")]
        public IActionResult DeleteCartItem(long id)
        {
            var cartItem = _context.CartItems.Include(ci => ci.Boiler).FirstOrDefault(ci => ci.Id == id);

            if (cartItem == null) return BadRequest($"Нет записи в корзине с id: {id}");

            var boiler = cartItem.Boiler;
            boiler.Quantity += cartItem.Quantity;

            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();

            return Ok($"Элемент корзины успешно удален. Возвращено {cartItem.Quantity} пары обуви.");
        }

        [HttpPut("UpdateCartItem")]
        public IActionResult UpdateCartItem(long cartItemId, long? cartId = null, long? boilerId = null, int? quantity = null)
        {
            var findCartItem = _context.CartItems.Include(ci => ci.Boiler).FirstOrDefault(ci => ci.Id == cartItemId);

            if (findCartItem == null) return BadRequest("Запись в корзине не найдена");

            if (quantity != null)
            {
                var boiler = findCartItem.Boiler;
                int oldQuantity = findCartItem.Quantity;
                int newQuantity = quantity.Value;
                int quantityDifference = newQuantity - oldQuantity;

                if (quantityDifference > 0 && boiler.Quantity < quantityDifference)
                {
                    return BadRequest("Обуви недостаточно на складе для увеличения количества");
                }

                boiler.Quantity -= quantityDifference;
                findCartItem.Quantity = newQuantity;
            }

            if (boilerId != null && findCartItem.BoilerId != boilerId.Value)
            {
                var oldBoiler = findCartItem.Boiler;
                var newBoiler = _context.Boilers.Find(boilerId.Value);

                if (newBoiler == null) return BadRequest("Новая обувь не найдена");

                oldBoiler.Quantity += findCartItem.Quantity;

                if (newBoiler.Quantity < findCartItem.Quantity)
                {
                    return BadRequest("Новой обуви недостаточно на складе");
                }

                newBoiler.Quantity -= findCartItem.Quantity;
                findCartItem.BoilerId = boilerId.Value;
                findCartItem.Boiler = newBoiler;
            }

            if (cartId != null) findCartItem.CartId = cartId.Value;

            _context.CartItems.Update(findCartItem);
            _context.SaveChanges();

            return Ok($"Объект корзины с id: {cartItemId} успешно обновлен");
        }
    }
}
