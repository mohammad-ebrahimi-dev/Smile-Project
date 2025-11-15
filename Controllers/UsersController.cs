using Microsoft.AspNetCore.Mvc;
using SmileProject.Databes.MainDbContext;
using SmileProject.Models;

namespace SmileProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MainDbContext _dbContext;

        public UsersController(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult Post([FromForm] UserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Mobile = userDto.Mobile
            };

            _dbContext.Users.Add(user); 
            _dbContext.SaveChanges();

            return Ok(new { message = "Information saved successfully!" });
        }
    }
}
