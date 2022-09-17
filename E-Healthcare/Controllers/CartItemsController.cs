using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Healthcare.Data;
using E_Healthcare.Models;
using Microsoft.AspNetCore.Authorization;

namespace E_Healthcare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class CartItemsController : ControllerBase
    {
        private readonly DataContext _context;

        public CartItemsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItems()
        {
            if (_context.CartItems == null)
            {
                return NotFound();
            }
            return await _context.CartItems.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CartItem>> GetCartItem(int id)
        {
            if (_context.CartItems == null)
            {
                return NotFound();
            }
            var cartItem = await _context.CartItems.FindAsync(id);

            if (cartItem == null)
            {
                return NotFound();
            }

            return cartItem;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCartItem(int id, CartItem cartItem)
        {
            if (id != cartItem.ID)
            {
                return BadRequest();
            }

            _context.Entry(cartItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartItemExists(id))
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

        [HttpPost]
        public async Task<ActionResult<CartItem>> PostCartItem(CartItem cartItem)
        {
            if (_context.CartItems == null)
            {
                return Problem("CartItems is null.");
            }
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCartItem", new { id = cartItem.ID }, cartItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            if (_context.CartItems == null)
            {
                return NotFound();
            }
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{medicineId}/{userId}")]
        public async Task<ActionResult> AddItemToTheCart(int medicineId, int userId)
        {
            Product medicine = await _context.Products.FindAsync(medicineId);

            if (medicine == null)
                return BadRequest("Medicine not found.");

            Cart cart = await _context.Carts.FirstOrDefaultAsync(x => x.OwnerID == userId);

            CartItem cartItem = new();
            cartItem.Cart = cart;
            cartItem.CartID = cart.ID;
            cartItem.Product = medicine;
            cartItem.ProductID = medicine.ID;

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return Ok(medicine);
        }

        private bool CartItemExists(int id)
        {
            return (_context.CartItems?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
