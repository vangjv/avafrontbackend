using AvaFront.API.Models;
using AvaFront.API.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AvaFront.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenAIController : ControllerBase
    {
        private readonly RedisService _redisService;
        
        private OpenAIService _openAIService;

        public OpenAIController(OpenAIService openAIService)
        {

            _openAIService = openAIService;
        }

        // POST api/<OpenAIController>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ConversationRequest conversationRequest)
        {

            if (!Request.Headers.TryGetValue("ConversationId", out var conversationId) || string.IsNullOrEmpty(conversationId))
            {
                return BadRequest("Missing or empty ConversationId header");
            }

            var response = await _openAIService.GenerateTextAsync(conversationId, conversationRequest.Message);

            return Ok(new ConversationResponse(response, conversationId));
        }

    }
}
