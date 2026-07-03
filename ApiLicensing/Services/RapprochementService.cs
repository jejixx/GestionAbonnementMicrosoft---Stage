using ApiLicensing.DTO;

namespace ApiLicensing.Services
{
    public partial class AppService
    {
        private readonly List<RapprochementDTO> _rapprochements = new()
        {
            new RapprochementDTO { Id = 1, Date = "01/06/2024", Client = "Alpha Solutions",      Periode = "Juin 2024", FactureFournisseur = "F-2024-0001", MontantFournisseur = "1 250,00 €", FactureClient = "FAC-2024-0001", MontantClient = "1 675,00 €", Ecart = "425,00 €", EcartPourcent = "33,99 %", Statut = "Écart positif", Commentaire = "Marge conforme aux prévisions" },
            new RapprochementDTO { Id = 2, Date = "03/06/2024", Client = "Beta Conseil",         Periode = "Juin 2024", FactureFournisseur = "F-2024-0002", MontantFournisseur = "625,00 €",   FactureClient = "FAC-2024-0002", MontantClient = "750,00 €",   Ecart = "125,00 €", EcartPourcent = "20,00 %", Statut = "Écart positif", Commentaire = "Écart acceptable" },
            new RapprochementDTO { Id = 3, Date = "05/06/2024", Client = "Gamma Industrie",      Periode = "Juin 2024", FactureFournisseur = "F-2024-0003", MontantFournisseur = "2 030,00 €", FactureClient = "FAC-2024-0003", MontantClient = "2 280,00 €", Ecart = "250,00 €", EcartPourcent = "12,32 %", Statut = "Écart positif", Commentaire = "Marge un peu plus faible" },
            new RapprochementDTO { Id = 4, Date = "07/06/2024", Client = "Delta Services",       Periode = "Juin 2024", FactureFournisseur = "F-2024-0004", MontantFournisseur = "880,00 €",   FactureClient = "FAC-2024-0004", MontantClient = "1 050,00 €", Ecart = "170,00 €", EcartPourcent = "19,32 %", Statut = "Écart positif", Commentaire = "Marge conforme" },
            new RapprochementDTO { Id = 5, Date = "10/06/2024", Client = "Epsilon Informatique", Periode = "Juin 2024", FactureFournisseur = "F-2024-0005", MontantFournisseur = "450,00 €",   FactureClient = "FAC-2024-0005", MontantClient = "610,00 €",   Ecart = "160,00 €", EcartPourcent = "35,56 %", Statut = "Écart positif", Commentaire = "Très bonne marge" },
        };

        public List<RapprochementDTO> GetRapprochements()
            => _rapprochements.ToList();
    }
}