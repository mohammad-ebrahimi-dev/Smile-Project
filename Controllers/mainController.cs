using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
    [HttpGet]
    public async Task<IActionResult> SendSentences()
    {
        var data = await _dbContext.Sentences.AsNoTracking().CountAsync(x => x.SentenceText != null);
        var ranNumber = new Random();
        var one = ranNumber.Next(1,data);
        var text =  await _dbContext.Sentences.SingleOrDefaultAsync(x => x.Id == one);
        return Content(text.SentenceText);
    }
    
}
