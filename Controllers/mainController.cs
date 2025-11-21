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
    [HttpGet("Send")]
    [HttpGet("Send")]
    public async Task<IActionResult> SendSentences()
    {
        var users = _dbContext.Users.Where(x => x.Id > 0).ToList();
        var sentenceCount = _dbContext.Sentences.Count();
        var random = new Random();

        var usersWithoutNewSentence = new List<int>(); // لیست userId هایی که جمله جدید ندارند

        foreach (var user in users)
        {
            // تعداد جملاتی که قبلاً برای این کاربر ارسال شده
            var sentCount = _dbContext.UserSentences.Count(us => us.UserId == user.Id);

            if (sentCount >= sentenceCount)
            {
                // یعنی تمام جملات قبلاً ارسال شده، skip کن
                usersWithoutNewSentence.Add(user.Id);
                Console.WriteLine($"User {user.Id} has received all sentences. Skipping.");
                continue;
            }

            int sentenceId;
            do
            {
                sentenceId = random.Next(1, sentenceCount + 1);
            }
            while (_dbContext.UserSentences.Any(us => us.UserId == user.Id && us.SentenceId == sentenceId));

            var chosenSentence = _dbContext.Sentences.First(x => x.Id == sentenceId);

            /// ارسال SMS یا هر کار دیگه
            ///

            var saveInformationForUser = new UserSentence
            {
                SendAt = DateTime.Now,
                Sentence = chosenSentence,
                SentenceId = sentenceId,
                User = user,
                UserId = user.Id
            };

            await _dbContext.UserSentences.AddAsync(saveInformationForUser);
        }

        await _dbContext.SaveChangesAsync();

        // در صورت نیاز، می‌تونی لیست usersWithoutNewSentence رو لاگ کنی یا برگردونی
        return Content("True");
    }

}
