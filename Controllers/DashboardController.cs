using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmileProject.Services;

namespace SmileProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly OTP _opt;
        public DashboardController(OTP opt)
        {
            _opt = opt;
        }
        [HttpGet("send")]
        public async Task<IActionResult> SendOTP(string mobile)
        {
            var result = await _opt.Send(mobile);
            return Ok(result.Content); 
        }

        public async Task<IActionResult> CheckOTP(string mobile , string code)
        {
            var result = await _opt.SignInUserAsync(code , mobile);
            return Ok(result.Content);
        }
    }
}
