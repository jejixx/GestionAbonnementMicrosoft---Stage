namespace ApiLicensing.DTO
{
    public class RapprochementDTO
    {
        public int Id { get; set; }

        public string? Date { get; set; }
        public string? Client { get; set; }
        public string? Periode { get; set; }

        public string? FactureFournisseur { get; set; }
        public string? MontantFournisseur { get; set; }

        public string? FactureClient { get; set; }
        public string? MontantClient { get; set; }

        public string? Ecart { get; set; }
        public string? EcartPourcent { get; set; }

        public string? Statut { get; set; }
        public string? Commentaire { get; set; }
    }
}