using E_Healthcare.Models;
using E_Healthcare.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace E_Healthcare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new();
        private static User adminAccount = new User
        {
            Email = "admin@admin.com",
            IsAdmin = true,
            AdminPassword = "admin"
        };

        private readonly IConfiguration _configuration;
        private readonly DataContext _context;

        public AuthController(IConfiguration configuration, DataContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterDto request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Phone = request.Phone;
            user.IsAdmin = false;
            user.AdminPassword = String.Empty;
            user.Address = request.Address;
            user.DateOfBirth = request.DateOfBirth;
            user.Email = request.Email;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto request)
        {
            if(!_context.Users.Any(x => x.Email == request.Email) && adminAccount.Email != request.Email)
            {
                return BadRequest("User not found.");
            }

            User currentUser = new();
            //admin case
            if(request.Email.Equals(adminAccount.Email))
            {
                if(!request.Password.Equals(adminAccount.AdminPassword))
                {
                    return BadRequest("Wrong password.");
                }

                currentUser.Email = request.Email;
                currentUser.IsAdmin = true;
            }
            //user case
            else
            {
                currentUser = _context.Users.FirstOrDefault(x => x.Email == request.Email);

                if (!VerifyPasswordHash(request.Password, currentUser.PasswordHash, currentUser.PasswordSalt))
                {
                    return BadRequest("Wrong password.");
                }
            }    

            string token = CreateToken(currentUser);
            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new();

            claims.Add(new Claim(ClaimTypes.Email, user.Email));

            if(user.IsAdmin == true)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(1),signingCredentials: credentials);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
