using Microsoft.EntityFrameworkCore;
using SmileProject.Controllers;
using SmileProject.Databes.Entities;
using SmileProject.Databes.MainDbContext;
using System.Security.Claims;

namespace SmileProject.Services.Dashboard
{
    public class SaveCategoryService
    {
        private readonly MainDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IResultService _resultService;

        public SaveCategoryService(
            MainDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IResultService resultService)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _resultService = resultService;
        }

        public async Task<IResultService> SaveAsync(SaveCategoryDto model)
        {
            try
            {
                var mobileClaim = _httpContextAccessor.HttpContext?.User
    .FindFirst(ClaimTypes.MobilePhone);

                if (mobileClaim == null)
                    return _resultService.Failed("کاربر یافت نشد.");

                string mobile = mobileClaim.Value;

                var oldCategories = await _dbContext.UserCategorySentences
                    .Where(x => x.User.Mobile == mobile).ToListAsync();

                if (oldCategories.Any())
                    _dbContext.UserCategorySentences.RemoveRange(oldCategories);

                var user = _dbContext.Users.Where(x => x.Mobile == mobile).FirstOrDefault();
                foreach (var item in model.Categories.Where(x => x.IsSelected))
                {
                    var categoryId = _dbContext.CategorySentences.Where(x => x.CategoryName.Contains(item.Name)).FirstOrDefault();

                    _dbContext.UserCategorySentences.Add(new UserCategorySentence
                    {
                        UserId = user.Id,
                        CategorySentenceId = categoryId.Id
                    });
                }

                await _dbContext.SaveChangesAsync();

                return _resultService.Success("تنظیمات با موفقیت ذخیره شد.");
            }
            catch(Exception ex)
            {
                return _resultService.Failed("ذخیره تنظیمات موفقیت آمیز نبود..");
            }

        }
    }
}
