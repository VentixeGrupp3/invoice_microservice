using Business.Forms;
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
    }
}
