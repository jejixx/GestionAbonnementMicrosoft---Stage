using ApiLicensing.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiLicensing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoriqueController : ControllerBase
    {
        private readonly AppService _service;
        public HistoriqueController(AppService service) => _service = service;

        [HttpGet("client/{clientId}")]  // GET /api/historique/client/1
        public IActionResult GetByClient(int clientId)
            => Ok(_service.GetHistoriqueByClientId(clientId));
    }
}