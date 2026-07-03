using ApiLicensing.DTO;
using System.Globalization;  // ← corrige CultureInfo et DateTimeStyles

namespace ApiLicensing.Services
{
    public partial class AppService
    {
        public List<AbonnementDTO> GetAbonnements()
        {
            var groupes = _abonnements
                .GroupBy(a => a.TypeAbonnement)
                .Select(g => new AbonnementDTO
                {
                    TypeAbonnement = g.Key,
                    NombreAbos = g.Sum(a => a.Quantite),
                })
                .ToList();

            double total = groupes.Sum(d => d.NombreAbos);
            groupes.ForEach(d => d.Total = total);
            return groupes;
        }

        public List<EvolutionDTO> GetEvolutionMensuelle()
        {
            var moisNoms = new[] { "Janvier","Février","Mars","Avril","Mai","Juin",
                                   "Juillet","Août","Septembre","Octobre","Novembre","Décembre" };

            return _abonnements
                .Where(a => DateTime.TryParseExact(a.DateDebut, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                .GroupBy(a =>
                {
                    DateTime.TryParseExact(a.DateDebut, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var d);
                    return d.Month;
                })
                .OrderBy(g => g.Key)
                .Select(g => new EvolutionDTO
                {
                    Mois = moisNoms[g.Key - 1],
                    NombreAbos = g.Sum(a => a.Quantite),
                })
                .ToList();
        }

        public List<RenouvellementDTO> GetRenouvellements()
        {
            var clients = GetClients();

            return clients
                .Where(c => !string.IsNullOrWhiteSpace(c.ProchainRenouvellement))
                .Select(c => new RenouvellementDTO
                {
                    Client = c.Nom,
                    DateRenouvellement = c.ProchainRenouvellement,
                    Statut = CalculerStatut(c.ProchainRenouvellement),
                })
                .OrderBy(r => r.DateRenouvellement)
                .ToList();
        }

        public int GetTotalRenouvellements()
            => GetRenouvellements().Count(r => r.Statut == "EnRetard" || r.Statut == "Bientot");

        private static string CalculerStatut(string dateStr)
        {
            if (!DateTime.TryParseExact(dateStr, "dd/MM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                return "Inconnu";

            int jours = (date.Date - DateTime.Today).Days;
            if (jours < 0) return "EnRetard";
            if (jours <= 30) return "Bientot";
            return "Actif";
        }
    }
}