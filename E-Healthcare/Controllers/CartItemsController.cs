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
    [Route("api/cartItem")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class CartItemsController : ControllerBase
    {
        private readonly DataContext _context;

        public CartItemsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("getAllCartItems")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItems()
        {
            if (_context.CartItems == null)
            {
                return NotFound();
            }
            return await _context.CartItems.ToListAsync();
        }

        [HttpGet("getCartItemById/{id}")]
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

        [HttpPut("updateQuantity{cartItemId}/{quantity}")]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            CartItem cartItem = await _context.CartItems.FirstOrDefaultAsync(x => x.ID == cartItemId);
            if (cartItem == null)
                return BadRequest("Cart item is null.");

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();

            return Ok("Successfully updated");
        }

        [HttpDelete("removeCartItem/{id}")]
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

            //update the quantity from the stock
            Product medicine = await _context.Products.FirstOrDefaultAsync(x => x.ID == cartItem.ProductID);

            if (medicine == null)
                return BadRequest("Not able to find the medicine in the stock.");

            medicine.Quantity += cartItem.Quantity;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("addCartItem/{medicineId}/{userId}/{quantity}")]
        public async Task<ActionResult> AddItemToCart(int medicineId, int userId,int quantity)
        {
            //getting the data from the database
            Product medicine = await _context.Products.FindAsync(medicineId);

            if (medicine == null)
                return BadRequest("Medicine not found.");

            if (medicine.Quantity < quantity)
                return BadRequest("The stock is too low.");

            Cart cart = await _context.Carts.FirstOrDefaultAsync(x => x.OwnerID == userId);

            //create and add the new entry
            CartItem cartItem = new();
            cartItem.Cart = cart;
            cartItem.CartID = cart.ID;
            cartItem.Product = medicine;
            cartItem.ProductID = medicine.ID;
            cartItem.Quantity = quantity;

            _context.CartItems.Add(cartItem);

            //update the remained quantity
            medicine.Quantity -= cartItem.Quantity;

            await _context.SaveChangesAsync();

            return Ok(medicine);
        }

        private bool CartItemExists(int id)
        {
            return (_context.CartItems?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
