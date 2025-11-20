using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SmileProject.Databes.Entities;
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
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> SendSentences()
    {
        var userId = int.Parse(User.FindFirst("UserId")?.Value);
        var sentIds = await _dbContext.UserSentences
                .Where(us => us.UserId == userId)
                .Select(us => us.SentenceId)
                .ToListAsync();

        var availableSentences = await _dbContext.Sentences
            .Where(s => !sentIds.Contains(s.Id) && s.IsActive)
            .ToListAsync();

        if (!availableSentences.Any())
            return Content("No new sentences available!");

        var ranNumber = new Random();
        var sentence = availableSentences[ranNumber.Next(availableSentences.Count)];

        _dbContext.UserSentences.Add(new UserSentence
        {
            UserId = userId,
            SentenceId = sentence.Id
        });
        await _dbContext.SaveChangesAsync();

        return Content(sentence.SentenceText);
    }
    
}
