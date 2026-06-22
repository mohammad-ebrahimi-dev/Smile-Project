using SmileProject.Databes.Entities;
using SmileProject.Databes.MainDbContext;

namespace SmileProject.Services
{
    public class BoardService
    {
        public MainDbContext _dbContext;
        public IResultService _resultService;

        public BoardService(MainDbContext dbContext, IResultService resultService)
        {
            _dbContext = dbContext;
            _resultService = resultService;
        }
        public ResultService GetBoardMessage()
        {
            var result = _dbContext.Boards.Any();
            if (result)
            {
                var content = _dbContext.Boards.FirstOrDefault(x => x.IsActive == true);
                return _resultService.Success(content.Text);
            }
            return _resultService.NotFound(null);
        }
    }
}
