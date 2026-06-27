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
        private readonly RateLimitation _rateLimiter;
        private readonly OTPService _otpService;

        public UsersController(MainDbContext dbContext, IConfiguration configuration, Authentication authentication, ShowUsers showUsers, RateLimitation rateLimiter, OTPService otpService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _authentication = authentication;
            _showUsers = showUsers;
            _rateLimiter = rateLimiter;
            _otpService = otpService;
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
            //limitation 
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var key = $"{ip}:{dto.Mobile}";

            var result = _rateLimiter.Check(key);

            if (!result.allowed)
            {
                return StatusCode(429, new
                {
                    message = "⛔ خیلی سریع درخواست دادی",
                    retryAfter = result.retryAfterSeconds
                });
            }
            //end limitation 
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
                //var registration = await _authentication.Register(dto);
                var SendOTP = _otpService.Send(dto.Mobile);

                return Ok(new
                {
                    message = "پیامک هویت سنجی ارسال شد",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "ثبت نام با خطا مواجه شد"
                });
            }
        }
        
        [HttpPost("SignInOtp")]
        public async Task<IActionResult> SignInOtp(SignInOtpRequest dto)
        {
            var result = await _otpService.SignInUserAsync(dto.OtpCode, dto.MobileNumber);
            return Ok(new
            {
                message = result.Content,
            });
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
        public class SignInOtpRequest
        {
            public string MobileNumber { get; set; }
            public string OtpCode { get; set; }
        }
    }
}

// DESKTOP-BDCLF1N\A