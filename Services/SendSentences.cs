using IPE.SmsIrClient;
using Microsoft.EntityFrameworkCore;
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
            var users = await _dbContext.Users.Where(x => x.Id > 0).ToListAsync().ConfigureAwait(false);
            var sentenceCount = await _dbContext.Sentences.AsNoTracking().CountAsync();
            var random = new Random();

            var usersWithoutNewSentence = new List<int>();
            try
            {
                foreach (var user in users)
                {
                    var sentCount = await _dbContext.UserSentences.AsNoTracking().CountAsync(us => us.UserId == user.Id);

                    if (sentCount >= sentenceCount)
                    {
                        usersWithoutNewSentence.Add(user.Id);
                        Console.WriteLine($"User {user.Id} has received all sentences. Skipping.");
                        continue;
                    }

                    int sentenceId;
                    do
                    {
                        sentenceId = random.Next(1, sentenceCount + 1);
                    }
                    while (await _dbContext.UserSentences.AsNoTracking()
                        .AnyAsync(us => us.UserId == user.Id && us.SentenceId == sentenceId));

                    var chosenSentence = await _dbContext.Sentences.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == sentenceId);

                    if (chosenSentence == null || string.IsNullOrEmpty(user.Mobile))
                    {
                        Logs.LogToFile("Sending sentences failed: Null part detected.");
                        return _result.Failed("Sending sentences failed (Null Part)");
                    }

                    SmsIr smsIr = new SmsIr("API_KEY_HERE");

                    var bulkSendResult = await smsIr.BulkSendAsync(300000000000,
                        chosenSentence.SentenceText,
                        new string[] { user.Mobile });

                    var saveInformationForUser = new UserSentence
                    {
                        SendAt = DateTime.Now,
                        SentenceId = sentenceId,
                        UserId = user.Id
                    };

                    await _dbContext.UserSentences.AddAsync(saveInformationForUser);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logs.LogToFile($"Sending sentences operation failed: {ex.Message}");
                return _result.Failed("Sending sentences operation failed");
            }

            Logs.LogToFile("Sending sentences succeeded.");
            return _result.Success("Sending sentences successful");
        }

    }

}
