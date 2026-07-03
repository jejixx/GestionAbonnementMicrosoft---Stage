using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GestionAboMicroService.DTO;

namespace GestionAboMicroService.Services
{
    public partial class AppService
    {
        // ✅ Un seul appel HTTP — les deux méthodes réutilisent la même liste
        public async Task<(List<AbonnementDTO> abonnements, List<EvolutionDTO> evolution)> GetDonneesDashboard()
        {
            var abonnements = await GetTousLesAbonnements(); // 1 seul appel HTTP

            var groupes = abonnements
                .GroupBy(a => a.TypeAbonnement)
                .Select(g => new AbonnementDTO
                {
                    TypeAbonnement = g.Key,
                    NombreAbos = g.Sum(a => a.Quantite),
                })
                .ToList();

            double total = groupes.Sum(d => d.NombreAbos);
            groupes.ForEach(d => d.Total = total);

            var moisNoms = new[] { "Janvier","Février","Mars","Avril","Mai","Juin",
                                   "Juillet","Août","Septembre","Octobre","Novembre","Décembre" };

            var evolution = abonnements
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

            return (groupes, evolution);
        }

        public async Task<List<RenouvellementDTO>> GetRenouvellements()
        {
            var clients = await GetClients(); // 1 seul appel HTTP

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

        public async Task<int> GetTotalRenouvellementsCritiques()
        {
            var renouvellements = await GetRenouvellements();
            return renouvellements.Count(r => r.Statut == "EnRetard" || r.Statut == "Bientot");
        }

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