using ChatGPT_CSharp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatGPT_CSharp.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class GPTController : ControllerBase
    {
        private readonly OpenAIService _openAIService;

        public GPTController(OpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [HttpGet]
        [Route("UseChatGPT")]
        public async Task<IActionResult> GetCompletion(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                return BadRequest("Prompt cannot be empty.");
            }

            var result = await _openAIService.GetCompletionAsync(prompt);
            return Ok(new { Completion = result });
        }
    }
}
