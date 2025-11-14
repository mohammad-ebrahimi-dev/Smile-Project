
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using  SmileProject.Databes.MainDbContext; 

namespace DailySentencesProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MainController : ControllerBase
{
          public MainDbContext _dbContext ;
          public MainController(MainDbContext dbContext)
          {
                    _dbContext = dbContext;
          }
          [HttpGet]
          public IActionResult Test()
          {
                    var data = _dbContext.Sentences.Where(x => x.SentenceText != null).ToList();
                    var result = new StringBuilder();
                    foreach(var itheme in data)
                    {
                              result.Append($"{itheme} \n");
                    }
                    return Content(result.ToString());
          }
}
