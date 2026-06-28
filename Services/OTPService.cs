using IPE.SmsIrClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SmileProject.Databes.Entities;
using SmileProject.Databes.MainDbContext;
using System.Security.Claims;

namespace SmileProject.Services
{
    public class OTPService
    {
        private readonly Random _random = new();
        private readonly IResultService _resultService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MainDbContext _dbContext;

        public OTPService(IResultService resultService,
                   IHttpContextAccessor httpContextAccessor,
                   MainDbContext dbContext)
        {
            _resultService = resultService;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        // SEND OTP
        public async Task<ResultService> Send(string mobileNumber)
        {
            var code = _random.Next(100000, 1000000);
            SmsIr smsIr = new SmsIr("NXqgkyS7aW23D98kgjqukfbbGw9rSjGQVSK6mVOLXF8eP28d");
            try
            {
                var bulkSendResult = await smsIr.BulkSendAsync(
                    30008828888384,
                    $@"به لبخند خوش آمدید
                کد تأیید شما: {code}
                این کد را در اختیار دیگران قرار ندهید.",
                    new string[] { mobileNumber });
                if (bulkSendResult.Status == 1)
                {
                    var newOtp = new Otp
                    {
                        Code = code.ToString(),
                        CreateDate = DateTime.Now,
                        ExpiresAt = DateTime.Now.AddMinutes(2),
                        IsUsed = false,
                        PhoneNumber = mobileNumber
                    };
                    await _dbContext.Otps.AddAsync(newOtp);
                    await _dbContext.SaveChangesAsync();
                    return _resultService.Success("SMS sent successfully");

                }
                else
                {
                    return _resultService.Failed("SMS has an error");

                }
            }
            catch (Exception ex)
            {
                return _resultService.Failed("SMS has an error");

            }

        }

        // VERIFY + LOGIN
        public async Task<ResultService> SignInUserAsync(string otpCode, string mobileNumber , string name)
        {
            var otp = await _dbContext.Otps
                .FirstOrDefaultAsync(x =>
                    x.PhoneNumber == mobileNumber &&
                    x.Code == otpCode &&
                    !x.IsUsed);
            var userId = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Mobile == mobileNumber &&
                    x.IsActive);

            if (otp == null)
                return _resultService.Failed("کد تأیید صحیح نیست.");

            if (otp.ExpiresAt <= DateTime.Now)
                return _resultService.Failed("کد تأیید منقضی شده است.");

            otp.IsUsed = true;
            await _dbContext.SaveChangesAsync();

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.Id.ToString()),
        new Claim(ClaimTypes.MobilePhone, mobileNumber),
        new Claim(ClaimTypes.Name, name),
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

            return _resultService.Success("ورود با موفقیت انجام شد.");
        }
        public enum Role
        {
            User,
            Admin
        }
    }
}