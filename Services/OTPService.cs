using IPE.SmsIrClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using StackExchange.Redis;
using System.Security.Claims;

namespace SmileProject.Services
{
    public class OTP
    {
        private readonly Random _random = new();
        private readonly IResultService _resultService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDatabase _db;

        public OTP(IResultService resultService,
                   IHttpContextAccessor httpContextAccessor)
        {
            _resultService = resultService;
            _httpContextAccessor = httpContextAccessor;

            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _db = redis.GetDatabase();
        }

        // SEND OTP
        public async Task<ResultService> Send(string mobileNumber)
        {
            var code = _random.Next(1000, 9999);

            SmsIr smsIr = new SmsIr("NXqgkyS7aW23D98kgjqukfbbGw9rSjGQVSK6mVOLXF8eP28d");

            var bulkSendResult = await smsIr.BulkSendAsync(
                30008828888384,
                $"کد تایید شما : {code}",
                new string[] { mobileNumber });



            await _db.StringSetAsync(
                $"otp:{mobileNumber}",
                code.ToString(),
                TimeSpan.FromMinutes(2)
            );

            return _resultService.Success("SMS sent successfully");
        }

        // VERIFY + LOGIN
        public async Task<ResultService> SignInUserAsync(string otpCode, string mobileNumber)
        {
            var savedCode = await _db.StringGetAsync($"otp:{mobileNumber}");

            if (savedCode.IsNullOrEmpty)
                return _resultService.Failed("کد منقضی شده است");

            if (savedCode != otpCode)
                return _resultService.Failed("کد اشتباه است");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, mobileNumber),
                new Claim(ClaimTypes.MobilePhone, mobileNumber),
                new Claim(ClaimTypes.Role, Role.User.ToString())
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                });

            await _db.KeyDeleteAsync($"otp:{mobileNumber}");

            return _resultService.Success("Login successful");
        }

        public enum Role
        {
            User,
            Admin
        }
    }
}