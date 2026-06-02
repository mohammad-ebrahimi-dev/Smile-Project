using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SmileProject.Databes.MainDbContext;

namespace SmileProject.Services
{
    public class ShowUsers
    {
        public MainDbContext _dbContext;
        public IResultService _result;

        public ShowUsers(MainDbContext dbContext, IResultService result)
        {
            _dbContext = dbContext;
            _result = result;
        }

        public async Task<ResultService> ReturnCount()
        {
            try
            {
                var count = await _dbContext.Users.CountAsync();
                return _result.Success((count + 1000).ToString());
            }
            catch(Exception ex)
            {
                return _result.Failed("0");
            }
            
        }
    }
}
