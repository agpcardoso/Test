using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SantanderTest.Services;

namespace SantanderTest.Controllers
{
    [ApiController]
    [Route("api/beststories")]
    public class BestStoriesController : ControllerBase
    {
        private readonly IBestStoriesService _service;


        public BestStoriesController(IBestStoriesService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetBestStories([FromQuery] int n = 10)
        {
            if (n <= 0) return BadRequest("n must be > 0");
            var result = await _service.GetBestStoriesAsync(n);
            return Ok(result);
        }
    }
}
