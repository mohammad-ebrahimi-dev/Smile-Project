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
            try
            {
                var users = _dbContext.Users.AsNoTracking().ToList();
                var random = new Random();
                foreach (var user in users)
                {
                    var number = random.Next(1, _dbContext.Sentences.Count());
                    var sentence = await _dbContext.Sentences.AsNoTracking().FirstOrDefaultAsync(x => x.Id == number);

                    System.IO.File.AppendAllText(
                        "Result.txt",
                        $"{DateTime.Now} | {sentence.SentenceText} | {user.FirstName} {user.LastName} - {user.Id}{Environment.NewLine}"
                    );
                    var saveInformationForUser = new UserSentence
                    {
                        SendAt = DateTime.Now,
                        Sentence = sentence,
                        SentenceId = number,
                        User = user,
                        UserId = user.Id
                    };

                    await _dbContext.UserSentences.AddAsync(saveInformationForUser);
                }
                return _result.Success("Sentencess Sent successfully");
            }
            catch (Exception ex)
            {
                return _result.Failed("Sentencess could not send");
            }
        }

    }

}
