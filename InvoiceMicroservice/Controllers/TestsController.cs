using Business.Forms;
using InvoiceMicroservice.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        [HttpGet("get-test")]
        public IActionResult Get()
        {
            return Ok("Swagger is working!");
        }

        [HttpPost("post-test")]
        public IActionResult Post(SwaggerTestForm form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok("Swagger is working!");
        }

        [ApiKeyAuthorize("User")]
        [HttpGet("user-test")]
        public IActionResult UserTest()
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            return Ok($"✅ User access confirmed. User ID: {userId}");
        }

        // Now only accessible by admins
        [ApiKeyAuthorize("Admin")]
        [HttpGet("admin-test")]
        public IActionResult AdminTest()
        {
            return Ok("✅ Admin access confirmed.");
        }
    }
}
