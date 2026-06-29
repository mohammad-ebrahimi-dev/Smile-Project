using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmileProject.Services;
using SmileProject.Services.Dashboard;
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

        public DashboardController(GetCategoryService getCategoryService, SaveCategoryService saveCategoryService)
        {
            _getCategoryService = getCategoryService;
            _saveCategoryService = saveCategoryService;
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
            var name = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var phone = User.FindFirst(System.Security.Claims.ClaimTypes.MobilePhone)?.Value;
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

//GetCategory
