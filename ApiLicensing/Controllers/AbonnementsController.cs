using ApiLicensing.DTO;
using ApiLicensing.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiLicensing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AbonnementsController : ControllerBase
    {
        private readonly AppService _service;

        public AbonnementsController(AppService service)
            => _service = service;

        [HttpGet]                                   // GET /api/abonnements
        public IResult GetAll()
            => Results.Ok(_service.GetTousLesAbonnements());

        [HttpGet("client/{clientId}")]              // GET /api/abonnements/client/1
        public IResult GetByClient(int clientId)
            => Results.Ok(_service.GetAbonnementsByClientId(clientId));

        [HttpPost]                                  // POST /api/abonnements
        public IResult Ajouter([FromBody] ClientAbonnementDTO abonnement)
        {
            _service.AjouterAbonnement(abonnement);
            return Results.Ok(abonnement);
        }

        [HttpPut]                                   // PUT /api/abonnements
        public IResult Modifier([FromBody] ClientAbonnementDTO abonnement)
        {
            _service.ModifierAbonnement(abonnement);
            return Results.Ok();
        }

        [HttpDelete("{id}")]                        // DELETE /api/abonnements/1
        public IResult Supprimer(int id)
        {
            _service.SupprimerAbonnement(id);
            return Results.Ok();
        }

        [HttpGet("total")]                          // GET /api/abonnements/total
        public IResult GetTotal()
            => Results.Ok(_service.GetTotalAbonnements());

        [HttpGet("client/{clientId}/total")]        // GET /api/abonnements/client/1/total
        public IResult GetTotalParClient(int clientId)
            => Results.Ok(_service.GetTotalAbonnementsParClient(clientId));

        [HttpGet("actifs")]                         // GET /api/abonnements/actifs
        public IResult GetActifs()
            => Results.Ok(_service.GetNombreAbonnementsActifs());

        [HttpGet("recherche")]                      // GET /api/abonnements/recherche?q=E3
        public IResult Rechercher([FromQuery] string q)
            => Results.Ok(_service.RechercherAbonnements(q));
    }
}