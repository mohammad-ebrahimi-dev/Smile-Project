using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmileProject.Databes.Entities;
using SmileProject.Databes.MainDbContext;
using SmileProject.Models;
using SmileProject.Services;
using System.ComponentModel.DataAnnotations;

namespace SmileProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MainDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly Authentication _authentication;
        private readonly ShowUsers _showUsers;

        public UsersController(MainDbContext dbContext, IConfiguration configuration, Authentication authentication, ShowUsers showUsers)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _authentication = authentication;
            _showUsers = showUsers;
        }


        [HttpGet("Count")]
        public async Task<IActionResult> Count()
        {
            try
            {
                var countService = await _showUsers.ReturnCount();

                if (countService == null)
                {
                    return Ok(new { message = 0 });
                }

                return Ok(new { message = countService.Content });
            }
            catch (Exception ex)
            {
                // لاگ کردن خطا
                return StatusCode(500, new { message = 0, error = ex.Message });
            }
        }

        [HttpPost("Sign")]
        public async Task<IActionResult> Sign([FromBody] RegisterDto dto)
        {
            // 1. اعتبارسنجی خودکار مدل به جای دستی
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "اطلاعات وارد شده معتبر نیست",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            // 2. حذف دقت زمانی (Timing attack protection)
            var normalizedMobile = dto.Mobile?.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(dto.Fullname?.Trim()) ||
                string.IsNullOrWhiteSpace(normalizedMobile))
            {
                return BadRequest(new { message = "نام و شماره همراه الزامی است" });
            }

            // 3. اعتبارسنجی شماره موبایل با Regex
            if (!IsValidMobileNumber(normalizedMobile))
            {
                return BadRequest(new { message = "شماره همراه نامعتبر است" });
            }

            try
            {
                var registration = await _authentication.Register(dto);

                if (registration?.Content == null)
                {
                        MaskMobileNumber(normalizedMobile);
                    return StatusCode(500, new { message = "خطا در ثبت نام" });
                }

                // 6. بازگرداندن اطلاعات حداقلی و بدون حساسیت
                return Ok(new
                {
                    message = registration.Content,
                    // هرگز userId توکن یا اطلاعات حساس در این مرحله برنگردانید
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.ToString() });
            }
        }

        // متدهای کمکی
        private bool IsValidMobileNumber(string mobile)
        {
            // Regex استاندارد برای شماره ایران
            return !string.IsNullOrWhiteSpace(mobile) &&
                   System.Text.RegularExpressions.Regex.IsMatch(mobile, @"^09[0-9]{9}$");
        }

        private string MaskMobileNumber(string mobile)
        {
            if (string.IsNullOrEmpty(mobile) || mobile.Length < 7)
                return "***";
            return mobile.Substring(0, 3) + "****" + mobile.Substring(mobile.Length - 4);
        }
    }
}

// DESKTOP-BDCLF1N\A