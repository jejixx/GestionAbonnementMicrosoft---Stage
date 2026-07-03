using ApiLicensing.DTO;
using System.Globalization;  // ← corrige CultureInfo et DateTimeStyles

namespace ApiLicensing.Services
{
    public partial class AppService
    {
        private readonly List<FactureClientDTO> _factures = new()
        {
            new FactureClientDTO { Id = 1,  ClientId = 1,  Client = "Alpha Solutions",      Numero = "FAC-2024-001", Date = "31/01/2024", DateEcheance = "29/02/2024", MontantHT = "2066,67 €", TVA = "413,33 €", MontantTTC = "2480,00 €", Statut = "Payée",     DatePaiement = "20/02/26", ModePaiement = "Virement",    Commentaire = "Facture mensuelle janvier" },
            new FactureClientDTO { Id = 2,  ClientId = 1,  Client = "Alpha Solutions",      Numero = "FAC-2024-002", Date = "28/02/2024", DateEcheance = "29/03/2024", MontantHT = "2066,67 €", TVA = "413,33 €", MontantTTC = "2480,00 €", Statut = "Payée",     DatePaiement = "20/02/26", ModePaiement = "Virement",    Commentaire = "Facture mensuelle février" },
            new FactureClientDTO { Id = 3,  ClientId = 1,  Client = "Alpha Solutions",      Numero = "FAC-2024-003", Date = "31/03/2024", DateEcheance = "30/04/2024", MontantHT = "2066,67 €", TVA = "413,33 €", MontantTTC = "2480,00 €", Statut = "EnAttente", DatePaiement = "",         ModePaiement = "Virement",    Commentaire = "Facture mensuelle mars" },
            new FactureClientDTO { Id = 4,  ClientId = 2,  Client = "Beta Conseil",         Numero = "FAC-2024-004", Date = "28/02/2025", DateEcheance = "30/03/2025", MontantHT = "916,67 €",  TVA = "183,33 €", MontantTTC = "1100,00 €", Statut = "Payée",     DatePaiement = "20/02/26", ModePaiement = "CB",          Commentaire = "Abonnement Business Standard" },
            new FactureClientDTO { Id = 5,  ClientId = 2,  Client = "Beta Conseil",         Numero = "FAC-2024-005", Date = "30/04/2024", DateEcheance = "30/05/2024", MontantHT = "916,67 €",  TVA = "183,33 €", MontantTTC = "1100,00 €", Statut = "EnRetard",  DatePaiement = "",         ModePaiement = "CB",          Commentaire = "Règlement en retard" },
            new FactureClientDTO { Id = 6,  ClientId = 3,  Client = "Gamma Industrie",      Numero = "FAC-2024-006", Date = "10/01/2024", DateEcheance = "09/02/2024", MontantHT = "2666,67 €", TVA = "533,33 €", MontantTTC = "3200,00 €", Statut = "Payée",     DatePaiement = "20/02/26", ModePaiement = "Prélèvement", Commentaire = "Facture lot E3" },
            new FactureClientDTO { Id = 7,  ClientId = 3,  Client = "Gamma Industrie",      Numero = "FAC-2024-007", Date = "10/02/2024", DateEcheance = "11/03/2024", MontantHT = "2666,67 €", TVA = "533,33 €", MontantTTC = "3200,00 €", Statut = "Payée",     DatePaiement = "20/02/26", ModePaiement = "Prélèvement", Commentaire = "Facture lot E3 février" },
            new FactureClientDTO { Id = 8,  ClientId = 4,  Client = "Delta Services",       Numero = "FAC-2023-008", Date = "30/11/2023", DateEcheance = "30/12/2023", MontantHT = "433,33 €",  TVA = "86,67 €",  MontantTTC = "520,00 €",  Statut = "EnRetard",  DatePaiement = "",         ModePaiement = "Virement",    Commentaire = "Facture impayée" },
            new FactureClientDTO { Id = 9,  ClientId = 5,  Client = "Epsilon Informatique", Numero = "FAC-2025-009", Date = "15/03/2025", DateEcheance = "14/04/2025", MontantHT = "800,00 €",  TVA = "160,00 €", MontantTTC = "960,00 €",  Statut = "Payée",     DatePaiement = "20/02/26", ModePaiement = "Virement",    Commentaire = "Facture E5" },
            new FactureClientDTO { Id = 10, ClientId = 6,  Client = "Zeta Corp",            Numero = "FAC-2025-010", Date = "01/06/2025", DateEcheance = "01/07/2025", MontantHT = "325,00 €",  TVA = "65,00 €",  MontantTTC = "390,00 €",  Statut = "EnAttente", DatePaiement = "",         ModePaiement = "CB",          Commentaire = "En attente de règlement" },
            new FactureClientDTO { Id = 11, ClientId = 7,  Client = "Eta Solutions",        Numero = "FAC-2024-011", Date = "01/09/2024", DateEcheance = "01/10/2024", MontantHT = "216,67 €",  TVA = "43,33 €",  MontantTTC = "260,00 €",  Statut = "Payée",     DatePaiement = "20/02/26", ModePaiement = "CB",          Commentaire = "Facture simple" },
            new FactureClientDTO { Id = 12, ClientId = 8,  Client = "Theta Group",          Numero = "FAC-2023-012", Date = "01/05/2023", DateEcheance = "31/05/2023", MontantHT = "108,33 €",  TVA = "21,67 €",  MontantTTC = "130,00 €",  Statut = "EnRetard",  DatePaiement = "",         ModePaiement = "Virement",    Commentaire = "Ancienne facture non soldée" },
            new FactureClientDTO { Id = 13, ClientId = 9,  Client = "Iota Digital",         Numero = "FAC-2025-013", Date = "01/01/2025", DateEcheance = "31/01/2025", MontantHT = "650,00 €",  TVA = "130,00 €", MontantTTC = "780,00 €",  Statut = "Payée",     DatePaiement = "20/02/26", ModePaiement = "Virement",    Commentaire = "Facture janvier" },
            new FactureClientDTO { Id = 14, ClientId = 10, Client = "Kappa Tech",           Numero = "FAC-2025-014", Date = "01/04/2025", DateEcheance = "01/05/2025", MontantHT = "866,67 €",  TVA = "173,33 €", MontantTTC = "1040,00 €", Statut = "EnAttente", DatePaiement = "",         ModePaiement = "CB",          Commentaire = "Facture trimestre 2" },
            new FactureClientDTO { Id = 15, ClientId = 11, Client = "Lambda Pro",           Numero = "FAC-2024-015", Date = "01/02/2024", DateEcheance = "02/03/2024", MontantHT = "1083,33 €", TVA = "216,67 €", MontantTTC = "1300,00 €", Statut = "EnRetard",  DatePaiement = "",         ModePaiement = "Virement",    Commentaire = "Paiement relancé" },
            new FactureClientDTO { Id = 16, ClientId = 12, Client = "Mu Conseil",           Numero = "FAC-2025-016", Date = "01/07/2025", DateEcheance = "31/07/2025", MontantHT = "541,67 €",  TVA = "108,33 €", MontantTTC = "650,00 €",  Statut = "Payée",     DatePaiement = "20/02/26", ModePaiement = "Prélèvement", Commentaire = "Facture standard" },
        };

        private readonly List<FactureFournisseurDTO> _facturesFournisseurs = new()
        {
            new FactureFournisseurDTO { Id = 1, Fournisseur = "Microsoft France",  Numero = "FOUR-2024-001", Date = "02/01/2024", DateEcheance = "01/02/2024", MontantHT = "1500,00 €", TVA = "300,00 €", MontantTTC = "1800,00 €", Statut = "Payée",     DatePaiement = "20/02/2026", ModePaiement = "Virement", Commentaire = "Achat licences Business" },
            new FactureFournisseurDTO { Id = 2, Fournisseur = "Microsoft Ireland", Numero = "FOUR-2024-002", Date = "05/02/2024", DateEcheance = "06/03/2024", MontantHT = "766,67 €",  TVA = "153,33 €", MontantTTC = "920,00 €",  Statut = "Payée",     DatePaiement = "20/02/2026", ModePaiement = "CB",       Commentaire = "Achat licences E3" },
            new FactureFournisseurDTO { Id = 3, Fournisseur = "Software Distrib",  Numero = "FOUR-2024-003", Date = "10/03/2024", DateEcheance = "09/04/2024", MontantHT = "541,67 €",  TVA = "108,33 €", MontantTTC = "650,00 €",  Statut = "EnAttente", DatePaiement = "",           ModePaiement = "Virement", Commentaire = "Frais services complémentaires" },
            new FactureFournisseurDTO { Id = 4, Fournisseur = "Cloud Reseller",    Numero = "FOUR-2024-004", Date = "15/04/2024", DateEcheance = "15/05/2024", MontantHT = "1000,00 €", TVA = "200,00 €", MontantTTC = "1200,00 €", Statut = "EnRetard",  DatePaiement = "",           ModePaiement = "Virement", Commentaire = "Plateforme revente" }
        };

        // ── Helpers parsing ──────────────────────────────────────────────
        private static double ParseMontant(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            string propre = s.Replace("€", "").Replace("\u00A0", "").Replace(" ", "").Trim();
            if (double.TryParse(propre, NumberStyles.Any, new CultureInfo("fr-FR"), out double valFr)) return valFr;
            if (double.TryParse(propre, NumberStyles.Any, CultureInfo.InvariantCulture, out double valEn)) return valEn;
            return 0;
        }

        private static string ExtraireMontantBrut(string? montantTTC)
        {
            if (string.IsNullOrWhiteSpace(montantTTC)) return "0";
            return montantTTC.Replace("€", "").Replace("\u00A0", "").Replace(" ", "").Trim();
        }

        // ── Lecture ──────────────────────────────────────────────────────

        public List<FactureClientDTO> GetFacturesByClientId(int clientId)
            => _factures.Where(f => f.ClientId == clientId).ToList();

        public List<FactureClientDTO> GetToutesLesFactures()
            => _factures.ToList();

        public int GetNombreFacturesEnAttente()
            => _factures.Count(f => f.Statut == "EnAttente" ^ f.Statut == "EnRetard");

        public FactureClientDTO? GetFactureById(int id)
            => _factures.FirstOrDefault(f => f.Id == id);

        public List<FactureFournisseurDTO> GetToutesLesFacturesFournisseurs()
            => _facturesFournisseurs.ToList();

        public FactureFournisseurDTO? GetFactureFournisseurById(int id)
            => _facturesFournisseurs.FirstOrDefault(f => f.Id == id);

        // ── Écriture factures clients ────────────────────────────────────

        public void AjouterFacture(FactureClientDTO f)
        {
            f.Id = _factures.Any() ? _factures.Max(x => x.Id) + 1 : 1;
            _factures.Add(f);
        }

        public void ModifierFacture(FactureClientDTO f)
        {
            int idx = _factures.FindIndex(x => x.Id == f.Id);
            if (idx >= 0) _factures[idx] = f;
        }

        public void SupprimerFacture(int id)
            => _factures.RemoveAll(x => x.Id == id);

        // ── Écriture factures fournisseurs ───────────────────────────────

        public void AjouterFactureFournisseur(FactureFournisseurDTO f)
        {
            f.Id = _facturesFournisseurs.Any() ? _facturesFournisseurs.Max(x => x.Id) + 1 : 1;
            _facturesFournisseurs.Add(f);
        }

        public void ModifierFactureFournisseur(FactureFournisseurDTO f)
        {
            int idx = _facturesFournisseurs.FindIndex(x => x.Id == f.Id);
            if (idx >= 0) _facturesFournisseurs[idx] = f;
        }

        public void SupprimerFactureFournisseur(int id)
            => _facturesFournisseurs.RemoveAll(x => x.Id == id);

        // ── Dashboard : résumé financier ─────────────────────────────────

        public List<LigneInfoDTO> GetLignesResume()
        {
            double ventes = _factures.Sum(f => ParseMontant(f.MontantTTC));
            double achat = _facturesFournisseurs.Sum(f => ParseMontant(f.MontantTTC));
            double marge = Math.Round(ventes - achat, 2);
            double taux = ventes != 0 ? Math.Round(marge / ventes * 100, 2) : 0;

            return new List<LigneInfoDTO>
            {
                new LigneInfoDTO { Label = "Achat Fournisseur", Valeur = $"{achat:N2} €",  TypeValeur = TypeValeur.Info },
                new LigneInfoDTO { Label = "Ventes Client",     Valeur = $"{ventes:N2} €", TypeValeur = TypeValeur.Positif },
                new LigneInfoDTO { Label = "Marge Brute",       Valeur = $"{marge:N2} €",  TypeValeur = marge >= 0 ? TypeValeur.Positif : TypeValeur.Negatif },
                new LigneInfoDTO { Label = "Taux de Marge",     Valeur = $"{taux:F1} %",   TypeValeur = TypeValeur.Neutre }
            };
        }
    }
}