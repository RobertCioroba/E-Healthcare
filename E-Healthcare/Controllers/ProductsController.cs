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
using E_Healthcare.Models.ViewModels;
using E_Healthcare.Models.Enums;

namespace E_Healthcare.Controllers
{
    [Route("api/medicine")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly DataContext _context;


        public ProductsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("getAllMedicine")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            return await _context.Products.ToListAsync();
        }

        [HttpGet("searchByUse/{use}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchByUse(string use)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            return await _context.Products.Where(x => x.Uses.Contains(use)).ToListAsync();
        }

        [HttpGet("getMedicineById/{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPut("updateMedicine/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ID)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        [HttpPost("addMedicine")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
          if (_context.Products == null)
          {
              return Problem("Product is null.");
          }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.ID }, product);
        }

        [HttpDelete("deleteMedicineById/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("generateReport/{sales}/{stock}/{range?}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ReportViewModel>> GenerateReport(bool sales, bool stock, string range = "total")
        {
            List<Product> medicines = new();
            List<Order> orders = new();

            //set the first day of the filter interval if it's needed
            DateTime initialDay = DateTime.Today;
            switch(range)
            {
                case "weekly":
                    initialDay = DateTime.Today.AddDays(-7);
                    break;
                case "monthly":
                    initialDay = DateTime.Today.AddMonths(-1);
                    break;
                case "yearly":
                    initialDay = DateTime.Today.AddYears(-1);
                    break;
                default:
                    break;
            }

            //get all orderes data if the user have been selected that option
            if(sales == true)
            {
                orders = await _context.Orders.ToListAsync();

                if (!range.Equals(""))
                {
                    orders = orders.Where(x => x.PlacedOn.Date >= initialDay.Date && x.PlacedOn.Date <= DateTime.Today.Date).ToList();
                }
            }

            //get all stocks data if the user have been selected that option
            if(stock == true)
            {
                medicines = await _context.Products.ToListAsync();
            }

            ReportViewModel report = new();
            report.Medicines = medicines;
            report.Orders = orders;

            return Ok(report);
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
