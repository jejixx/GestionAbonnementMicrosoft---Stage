using Microsoft.AspNetCore.Mvc;

namespace ApiLicensing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("ok")]
        public IActionResult OkTest()
        {
            return Ok("Test OK");
        }

        [HttpGet("badrequest")]
        public IActionResult BadRequestTest()
        {
            return BadRequest("Test BadRequest");
        }

        [HttpGet("notfound")]
        public IActionResult NotFoundTest()
        {
            return NotFound("Test NotFound");
        }

        [HttpGet("error")]
        public IActionResult ErrorTest()
        {
            return StatusCode(500, "Test Error");
        }
    }
}