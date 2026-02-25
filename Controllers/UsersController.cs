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
        private readonly Authentication _authentication;

        public UsersController(MainDbContext dbContext, IConfiguration configuration, Authentication authentication)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _authentication = authentication;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var registeration = _authentication.Register(dto);
            return Ok(new { message = registeration.Result.Content });

        }
    }
}
