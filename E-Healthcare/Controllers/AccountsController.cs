using E_Healthcare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Healthcare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AccountsController : ControllerBase
    { 
        private readonly DataContext _context;
        public AccountsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Account>>> Get()
        {
            return Ok(await _context.Accounts.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Account>>> Get(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
                return BadRequest("Account not found");

            return Ok(account);
        }

        [HttpPost]
        public async Task<ActionResult<List<Account>>> AddAccount(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return Ok(await _context.Accounts.ToListAsync());
        }

        [HttpPut]
        public async Task<ActionResult<List<Account>>> UpdateAccount(Account request)
        {
            var account = await _context.Accounts.FindAsync(request.ID);
            if (account == null)
                return BadRequest("Account not found");

            account.AccNumber = request.AccNumber;
            account.Amount = request.Amount;
            account.Email = request.Email;

            await _context.SaveChangesAsync();

            return Ok(await _context.Accounts.ToListAsync()); ;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Account>>> Delete(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
                return BadRequest("Account not found");

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return Ok(await _context.Accounts.ToListAsync());
        }
    }
}
