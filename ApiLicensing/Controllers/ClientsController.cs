using ApiLicensing.DTO;
using ApiLicensing.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiLicensing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly AppService _service;

        public ClientsController(AppService service)
        {
            _service = service;
        }

        [HttpGet]                                   // GET /api/clients
        public IResult GetAll()
            => Results.Ok(_service.GetClients());

        [HttpGet("summary")]                    // GET /api/clients/summary
        public IResult GetSummary([FromQuery] string? q = null)
            => Results.Ok(_service.GetClientsSummary(q));

        [HttpGet("{id}")]                           // GET /api/clients/1
        public IResult GetById(int id)
        {
            var client = _service.GetClientById(id);
            if (client == null) return Results.Ok();
            return Results.Ok(client);
        }

        [HttpGet("recherche")]                      // GET /api/clients/recherche?q=alpha
        public IResult Rechercher([FromQuery] string q)
            => Results.Ok(_service.GetClients(q));

        [HttpGet("total")]                          // GET /api/clients/total
        public IResult GetTotal()
            => Results.Ok(_service.GetTotalClients());

        [HttpPost]                                  // POST /api/clients
        public IResult Ajouter([FromBody] ClientDTO client)
        {
            var clientCree = _service.AjouterClient(client);
            return Results.Ok(clientCree);  // ✅ renvoie le client complet
        }

        [HttpPut]                                   // PUT /api/clients
        public IResult Modifier([FromBody] ClientDTO client)
        {
            _service.ModifierClient(client);
            return Results.Ok();
        }

        [HttpDelete("{id}")]
        public IResult Supprimer(int id, [FromQuery] bool force = false)
        {
            var client = _service.GetClientById(id);
            if (client == null)
                return Results.BadRequest($"Client {id} introuvable.");

            bool ok = _service.SupprimerClient(id, force);

            return ok ? Results.Ok() : Results.BadRequest("Ce client a des abonnements. Confirmez avec force=true.");
        }


    }
}