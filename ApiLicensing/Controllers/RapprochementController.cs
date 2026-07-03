using ApiLicensing.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiLicensing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RapprochementController : ControllerBase
    {
        private readonly AppService _service;
        public RapprochementController(AppService service) => _service = service;

        [HttpGet]  // GET /api/rapprochement
        public IActionResult GetTous() => Ok(_service.GetRapprochements());
    }
}