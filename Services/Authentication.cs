using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmileProject.Databes.Entities;
using SmileProject.Databes.MainDbContext;
using SmileProject.Models;

namespace SmileProject.Services
{
    public class Authentication
    {
        public MainDbContext _dbContext;
        public IResultService _resultService;
        public Authentication(MainDbContext dbContext, IResultService result)
        {
            _dbContext = dbContext;
            _resultService = result;
        }

        public async Task<ResultService> Register([FromBody] RegisterDto dto)
        {
            if (dto.Mobile.Count() < 10 || dto.Mobile.Count() > 11)
                return _resultService.Failed("!فرمت وارد شده درست نمیباشد");
            ;

            if (dto.Mobile[0] != '0')
                dto.Mobile = '0' + dto.Mobile;

            if (dto.Mobile[0] != '0' || dto.Mobile[1] != '9')
                return _resultService.Failed("!فرمت وارد شده درست نمیباشد");

            try
            {
                if (await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Mobile == dto.Mobile))
                    return _resultService.Failed("!کاربر مورد نظر از قبل وجود دارد");

                var user = new User
                {
                    Fullname = dto.Fullname,
                    Mobile = dto.Mobile
                };

                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();
                return _resultService.Success("ثبت نام با موفقیت انجام شد ");

            }
            catch (Exception ex)
            {
                var log = new Log()
                {
                    Text = ex.Message,
                };
                await _dbContext.Logs.AddAsync(log);
                await _dbContext.SaveChangesAsync();
                return _resultService.Failed("!ثبت نام با خطا مواجه شد " );
            }
        }
    }
}
