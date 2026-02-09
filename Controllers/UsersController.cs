using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                //using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                //await _dbContext.Database.CanConnectAsync(cts.Token);

                if (await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Mobile == dto.Mobile))
                    return Ok("User already exists.");

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
            catch
            {
                return BadRequest(new { message = "Registeration proccess faild!" });
            }
        }
    }
}
