namespace GestionAboMicroService.DTO
{
    // Base commune
    public class FactureDTO
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string DateEcheance { get; set; } = string.Empty;
        public string MontantHT { get; set; } = string.Empty;
        public string TVA { get; set; } = string.Empty;
        public string MontantTTC { get; set; } = string.Empty;
        public string ModePaiement { get; set; } = string.Empty;
        public string DatePaiement { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
        public string Commentaire { get; set; } = string.Empty;
    }

    // Facture Client — ajoute ClientId + nom du client pour l'affichage
    public class FactureClientDTO : FactureDTO
    {
        public int ClientId { get; set; }
        public string Client { get; set; } = string.Empty;
    }

    // Facture Fournisseur — ajoute le nom du fournisseur
    public class FactureFournisseurDTO : FactureDTO
    {
        public string Fournisseur { get; set; } = string.Empty;
    }
}