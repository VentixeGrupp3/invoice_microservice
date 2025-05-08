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

        [ApiKeyAuthorize]
        [HttpGet("user-test")]
        public IActionResult UserTest()
        {
            var role = HttpContext.Items["Role"]?.ToString();
            var userId = HttpContext.Items["UserId"]?.ToString();

            if (role != "User")
                return Forbid("Access denied. Only users may access this endpoint.");

            return Ok($"✅ User access confirmed. User ID: {userId}");
        }

        [ApiKeyAuthorize]
        [HttpGet("admin-test")]
        public IActionResult AdminTest()
        {
            var role = HttpContext.Items["Role"]?.ToString();

            if (role != "Admin")
                return Forbid("Access denied. Only admins may access this endpoint.");

            return Ok("✅ Admin access confirmed.");
        }
    }
}
