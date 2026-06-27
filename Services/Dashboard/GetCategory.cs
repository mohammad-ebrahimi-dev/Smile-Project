using IPE.SmsIrClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SmileProject.Databes.Entities;
using SmileProject.Databes.MainDbContext;
using System.Security.Claims;

namespace SmileProject.Services
{
    public class GetCategoryService
    {
        private readonly IResultService _resultService;
        public MainDbContext _dbContext;

        public GetCategoryService(IResultService resultService,
                   MainDbContext dbContext)
        {
            _resultService = resultService;
            _dbContext = dbContext;
        }
        public  async Task<ResultService> Get()
        {
            var categories = _dbContext.CategorySentences.Where(x => x.IsActive == true).Select (x => x.CategoryName).ToList();
            return _resultService.Success("موفقیت آمیز بود ",obj:categories);
        }
    }
}