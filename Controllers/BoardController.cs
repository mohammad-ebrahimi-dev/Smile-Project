using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmileProject.Services;

namespace SmileProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        private BoardService _boardService;
        public BoardController(BoardService boardService)
        {
            _boardService = boardService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var result = _boardService.GetBoardMessage();
            return Ok(result.Content);
        }
    }
}
