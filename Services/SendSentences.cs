using IPE.SmsIrClient;
using Microsoft.EntityFrameworkCore;
using SmileProject.Databes.Entities;
using SmileProject.Databes.MainDbContext;

namespace SmileProject.Services
{
    public class SentencesService
    {
        public MainDbContext _dbContext;
        public IResultService _result;
        public SentencesService(MainDbContext dbContext, IResultService result)
        {
            _dbContext = dbContext;
            _result = result;
        }
        public async Task<ResultService> SendSentences()
        {
            try
            {
                var users = _dbContext.Users.AsNoTracking().Where(x => x.IsActive == true).ToList();
                var random = new Random();
                foreach (var user in users)
                {
                    var number = random.Next(1, _dbContext.Sentences.Count());
                    var sentence = await _dbContext.Sentences.AsNoTracking().FirstOrDefaultAsync(x => x.Id == number);

                    //System.IO.File.AppendAllText(
                    //    "Result.txt",
                    //    $"{DateTime.Now} | {sentence.SentenceText} | {user.FirstName} {user.LastName} - {user.Id}{Environment.NewLine}"
                    //);
                    //NXqgkyS7aW23D98kgjqukfbbGw9rSjGQVSK6mVOLXF8eP28d
                    try
                    {
                        SmsIr smsIr = new SmsIr("NXqgkyS7aW23D98kgjqukfbbGw9rSjGQVSK6mVOLXF8eP28d");
                        var bulkSendResult = await smsIr.BulkSendAsync(30008828888384,
                        $"{sentence.SentenceText}",
                        new string[] { $"{user.Mobile}" });

                    }
                    catch (Exception ex)
                    {
                        var logError = new Log { Text = $"Error sending SMS: {ex.Message}" };
                        await _dbContext.Logs.AddAsync(logError);
                    }
                    await _dbContext.SaveChangesAsync();
                }
                await _dbContext.SaveChangesAsync();
                return _result.Success("Sentencess Sent successfully");
            }
            catch (Exception ex)
            {
                return _result.Failed("Sentencess could not send");
            }
        }

    }

}
