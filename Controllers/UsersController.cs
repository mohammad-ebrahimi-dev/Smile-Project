using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmileProject.Databes.MainDbContext;
using SmileProject.Models;
using SmileProject.Databes.Entities;
using SmileProject.Services;

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
            if (dto.Mobile.Count() < 10 || dto.Mobile.Count() > 11)
                return BadRequest(new { message = "!فرمت وارد شده درست نمیباشد" });

            if (dto.Mobile[0] != '0')
                dto.Mobile = '0' + dto.Mobile;

            if (dto.Mobile[0] != '0' || dto.Mobile[1] != '9')
                return BadRequest(new { message = "!فرمت وارد شده درست نمیباشد" });

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

                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();
                //var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Result.txt");
                //await System.IO.File.AppendAllTextAsync(
                //logPath,
                //$"{DateTime.Now} | {user.FirstName} {user.LastName} | {user.Id} | Successfull Login{Environment.NewLine}"
                //);
                var log = new Log()
                {
                    Text = $"ثبت نام کاربر {user.FirstName} {user.LastName} با موبایل {user.Mobile}",
                };
                await _dbContext.Logs.AddAsync(log);
                await _dbContext.SaveChangesAsync();
                return Ok(new { message = "ثبت نام با موفقیت انجام شد " });

            }
            catch (Exception ex)
            {
                //System.IO.File.AppendAllText(
                //"Result.txt",
                //$"{DateTime.Now} | {ex.Message}| Login Faild{Environment.NewLine}"
                //);
                var log = new Log()
                {
                    Text = ex.Message,
                };
                await _dbContext.Logs.AddAsync(log);
                await _dbContext.SaveChangesAsync();
                return BadRequest(new { message = "!ثبت نام با خطا مواجه شد " });
            }
        }
    }
}
