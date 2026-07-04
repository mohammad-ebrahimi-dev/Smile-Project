using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmileProject.Databes.Entities;
using SmileProject.Databes.MainDbContext;
using SmileProject.Services;
using SmileProject.Services.Dashboard;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmileProject.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly GetCategoryService _getCategoryService;
        private readonly SaveCategoryService _saveCategoryService;
        private readonly MainDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardController(
            GetCategoryService getCategoryService,
            SaveCategoryService saveCategoryService,
            MainDbContext dbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _getCategoryService = getCategoryService;
            _saveCategoryService = saveCategoryService;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetInfo()
        {
            var result = await _getCategoryService.Get();
            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetUserInfo()
        {
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
            var phone = User.FindFirst(ClaimTypes.MobilePhone)?.Value;
            var result = await _getCategoryService.Get();

            return Ok(new
            {
                name,
                phone,
                result
            });
        }

        [HttpPost("categories")]
        public async Task<IActionResult> SaveCategory([FromBody] SaveCategoryDto model)
        {
            var save = await _saveCategoryService.SaveAsync(model);
            return Ok(save);
        }

        // دریافت وضعیت فعال‌سازی کاربر
        [HttpGet("active-status")]
        public async Task<IActionResult> GetActiveStatus()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "کاربر شناسایی نشد." });

            var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound(new { message = "کاربر یافت نشد." });

            return Ok(new { isActive = user.IsActive });
        }

        // تغییر وضعیت فعال/غیرفعال (Toggle)
        [HttpPost("toggle-active")]
        public async Task<IActionResult> ToggleActive()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "کاربر شناسایی نشد." });

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound(new { message = "کاربر یافت نشد." });

            // تغییر وضعیت
            user.IsActive = !user.IsActive;
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                isActive = user.IsActive,
                message = user.IsActive ? "حساب شما فعال شد و پیامک‌ها ارسال می‌شوند." : "حساب شما غیرفعال شد و پیامک‌ها ارسال نمی‌شوند."
            });
        }
    }

    public class SaveCategoryDto
    {
        public List<CategoryItemDto> Categories { get; set; } = [];
    }

    public class CategoryItemDto
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}