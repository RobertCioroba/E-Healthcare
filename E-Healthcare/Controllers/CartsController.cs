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
using E_Healthcare.Models.Enums;

namespace E_Healthcare.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class CartsController : ControllerBase
    {
        private readonly DataContext _context;

        public CartsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("getAllCarts")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCarts()
        {
          if (_context.Carts == null)
          {
              return NotFound();
          }
            return await _context.Carts.ToListAsync();
        }

        [HttpGet("getCartById/{id}")]
        public async Task<ActionResult<Cart>> GetCart(int id)
        {
          if (_context.Carts == null)
          {
              return NotFound();
          }
            var cart = await _context.Carts.FindAsync(id);

            if (cart == null)
            {
                return NotFound();
            }

            return cart;
        }

        [HttpPut("checkout/{cartId}")]
        public async Task<ActionResult> Checkout(int cartId)
        {
            //getting all the data from the database
            if (_context.Carts == null)
            {
                return NotFound();
            }

            Cart cart = await _context.Carts.FindAsync(cartId);

            if (cart == null)
                return BadRequest("Card not found.");

            User user = await _context.Users.FirstOrDefaultAsync(x => x.ID == cart.OwnerID);
            List<CartItem> cartItems = await _context.CartItems.Include(x => x.Product).Where(x => x.CartID == cartId).ToListAsync();

            if (cartItems.Count == 0)
            {
                BadRequest("Please add a medicine before.");
            }

            double total = 0;

            //calculate the total of the cart items
            foreach(var item in cartItems)
            {
                total += item.Product.Price * item.Quantity;
            }

            //pay for the products with the money from personal account
            Account account = await _context.Accounts.FirstOrDefaultAsync(x => x.Email.Equals(user.Email));
            if (account == null)
            {
                return BadRequest("Account not found.");
            }

            if(total > account.Amount)
            {
                return BadRequest("Insufficient funds.");
            }
            account.Amount -= total;

            //remove the items from the cart
            foreach (var item in cartItems)
            {
                _context.CartItems.Remove(item);
            }

            //place the new order
            Order order = new();

            order.PlacedOn = DateTime.Now;
            order.User = user;
            order.UserID = user.ID;
            order.TotalAmount = total;
            order.Status = OrderStatus.New;

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            return Ok(order);
        }

        private bool CartExists(int id)
        {
            return (_context.Carts?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
