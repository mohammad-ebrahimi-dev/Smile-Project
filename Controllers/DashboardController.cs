using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmileProject.Services;
using System.Threading.Tasks;

namespace SmileProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly GetCategoryService _getCategoryService;

        public DashboardController(GetCategoryService getCategoryService)
        {
            _getCategoryService = getCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory()
        {
            var result = await _getCategoryService.Get();
            return Ok(result);
        }
    }
}
