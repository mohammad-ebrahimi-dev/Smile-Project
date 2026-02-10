using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmileProject.Databes.MainDbContext;

namespace DailySentencesProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MainController : ControllerBase
{
    public MainDbContext _dbContext;
    public MainController(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    [HttpGet("Send")]
    public async Task<IActionResult> SendSentences()
    {
       
        return Content("True");
    }

}
