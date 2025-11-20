using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmileProject.Databes.MainDbContext;
using SmileProject.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmileProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MainDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public UsersController(MainDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        
        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            if (_dbContext.Users.Any(u => u.Mobile == dto.Mobile))
                return BadRequest("User already exists.");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Mobile = dto.Mobile
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return Ok(new { message = "User registered successfully!" });
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Mobile == dto.Mobile);
            if (user == null)
                return Unauthorized("Invalid mobile number.");

            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("Mobile", user.Mobile)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    }
}
