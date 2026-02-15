using Microsoft.AspNetCore.Mvc;
using SmileProject.Services;

namespace SmileProject.Controllers
{
    [ApiController]
    [Route("api/jobs")]
    public class JobController : ControllerBase
    {
        private readonly SentencesService _sentencesService;

        public JobController(SentencesService sentencesService)
        {
            _sentencesService = sentencesService;
        }

        [HttpGet("send-daily-sentences")]
        public async Task<IActionResult> SendDaily(string token)
        {
            if (token == "EbrTest526")
            {
                await _sentencesService.SendSentences();
                return Ok("Job executed");
            }
             return Unauthorized();
        }
    }
}
