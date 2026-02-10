using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmileProject.Databes.MainDbContext;
using SmileProject.Models;

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
                if (await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Mobile == dto.Mobile))
                    return Ok(new { message = "!کاربر مورد نظر از قبل وجود دارد" });

                var user = new User
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Mobile = dto.Mobile
                };

                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
                System.IO.File.AppendAllText(
                "Result.txt",
                $"{DateTime.Now} | {user.FirstName} {user.LastName} | {user.Id} | Successfull Login{Environment.NewLine}"
                );
                return Ok(new { message = "ثبت نام با موفقیت انجام شد " });
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(
                "Result.txt",
                $"{DateTime.Now} | {ex.Message}| Login Faild{Environment.NewLine}"
                );
                return BadRequest(new { message = "!ثبت نام با خطا مواجه شد " });
            }
        }
    }
}
