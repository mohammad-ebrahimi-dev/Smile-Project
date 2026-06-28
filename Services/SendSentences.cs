using IPE.SmsIrClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SmileProject.Databes.Entities;
using SmileProject.Databes.MainDbContext;

namespace SmileProject.Services
{
    public class SentencesService
    {
        public MainDbContext _dbContext;
        public IResultService _result;
        private static Random _random = new Random();
        public SentencesService(MainDbContext dbContext, IResultService result)
        {
            _dbContext = dbContext;
            _result = result;
        }
        public async Task<ResultService> SendSentences()
        {
            try
            {
                var users = _dbContext.Users
                .AsNoTracking()
                .Where(x => x.IsActive)
                .ToList();

                // گرفتن همه دسته‌ها یک‌بار (برای جلوگیری از N+1)
                var allCategoryIds = _dbContext.CategorySentences
                    .AsNoTracking()
                    .Select(x => x.Id)
                    .ToList();

                foreach (var user in users)
                {
                    var userCategories = _dbContext.UserCategorySentences
                        .AsNoTracking()
                        .Where(x => x.UserId == user.Id)
                        .Select(x => x.CategorySentenceId)
                        .ToList();

                    int categoryId;

                    // اگر کاربر دسته داشت
                    if (userCategories.Any())
                    {
                        categoryId = userCategories[_random.Next(userCategories.Count)];
                    }
                    else
                    {
                        // اگر نداشت از کل دسته‌ها
                        categoryId = allCategoryIds[_random.Next(allCategoryIds.Count)];
                    }

                    var sentences = _dbContext.Sentences
                        .AsNoTracking()
                        .Where(x => x.CategoryId == categoryId)
                        .ToList();

                    if (!sentences.Any())
                        continue;

                    var sentence = sentences[_random.Next(sentences.Count)];
                    SmsIr smsIr = new SmsIr("NXqgkyS7aW23D98kgjqukfbbGw9rSjGQVSK6mVOLXF8eP28d");
                    var bulkSendResult = await smsIr.BulkSendAsync(30008828888384,
                    $"{sentence.SentenceText}",
                    new string[] { $"{user.Mobile}" });

                }
                return _result.Success("Sentencess Sent successfully");

            }
            catch (Exception ex)
            {
                var logError = new Log { Text = $"Error sending SMS: {ex.Message}" };
                await _dbContext.Logs.AddAsync(logError);

            }
            await _dbContext.SaveChangesAsync();
            await _dbContext.SaveChangesAsync();
            return _result.Success("Sentencess Sent successfully");
        }

    }

}
