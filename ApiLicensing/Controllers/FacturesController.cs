using ApiLicensing.DTO;
using ApiLicensing.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiLicensing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturesController : ControllerBase
    {
        private readonly AppService _service;
        public FacturesController(AppService service) => _service = service;

        [HttpGet]
        public IResult GetTous() => Results.Ok(_service.GetToutesLesFactures());

        [HttpGet("{id}")]
        public IResult GetById(int id)
        {
            var f = _service.GetFactureById(id);
            return f is null ? Results.NotFound() : Results.Ok(f);
        }

        [HttpGet("client/{clientId}")]
        public IResult GetByClient(int clientId) => Results.Ok(_service.GetFacturesByClientId(clientId));

        [HttpGet("attente/total")]
        public IResult GetTotalEnAttente() => Results.Ok(_service.GetNombreFacturesEnAttente());

        [HttpPost]
        public IResult Ajouter(FactureClientDTO f) { _service.AjouterFacture(f); return Results.Ok(f); }

        [HttpPut]
        public IResult Modifier(FactureClientDTO f) { _service.ModifierFacture(f); return Results.Ok(); }

        [HttpDelete("{id}")]
        public IResult Supprimer(int id) { _service.SupprimerFacture(id); return Results.Ok(); }

        [HttpGet("resume")]
        public IResult GetResume() => Results.Ok(_service.GetLignesResume());

        // ── Fournisseurs ─────────────────────────────────────────────────

        [HttpGet("fournisseurs")]
        public IResult GetFournisseurs() => Results.Ok(_service.GetToutesLesFacturesFournisseurs());

        [HttpGet("fournisseurs/{id}")]
        public IResult GetFournisseurById(int id)
        {
            var f = _service.GetFactureFournisseurById(id);
            return f is null ? Results.NotFound() : Results.Ok(f);
        }

        [HttpPost("fournisseurs")]
        public IResult AjouterFournisseur(FactureFournisseurDTO f) { _service.AjouterFactureFournisseur(f); return Results.Ok(f); }

        [HttpPut("fournisseurs")]
        public IResult ModifierFournisseur(FactureFournisseurDTO f) { _service.ModifierFactureFournisseur(f); return Results.Ok(); }

        [HttpDelete("fournisseurs/{id}")]
        public IResult SupprimerFournisseur(int id) { _service.SupprimerFactureFournisseur(id); return Results.Ok(); }
    }
}