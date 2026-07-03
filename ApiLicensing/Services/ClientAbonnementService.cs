using System;
using System.Collections.Generic;
using System.Globalization;   // ← le manquant
using System.Linq;
using ApiLicensing.DTO;
namespace ApiLicensing.Services
{
    public partial class AppService
    {
        // ── Données mock locales (simule un endpoint GET /abonnements) ─
        private readonly List<ClientAbonnementDTO> _abonnements = new()
        {
            new ClientAbonnementDTO { Id = 1,  ClientId = 1,  TypeAbonnement = "Microsoft 365 Business Standard", Quantite = 10, DateDebut = "01/01/2024", DateFin = "31/12/2026", ProchainRenouvellement = "01/01/2027", Statut = "Actif"     },
            new ClientAbonnementDTO { Id = 2,  ClientId = 1,  TypeAbonnement = "Microsoft 365 Business Premium",  Quantite = 5,  DateDebut = "01/03/2024", DateFin = "28/02/2026", ProchainRenouvellement = "01/03/2026", Statut = "Actif"     },
            new ClientAbonnementDTO { Id = 3,  ClientId = 1,  TypeAbonnement = "Microsoft 365 E3",                Quantite = 2,  DateDebut = "15/06/2023", DateFin = "14/06/2025", ProchainRenouvellement = "15/06/2025", Statut = "Expiré"    },
            new ClientAbonnementDTO { Id = 4,  ClientId = 2,  TypeAbonnement = "Microsoft 365 Business Standard", Quantite = 8,  DateDebut = "01/02/2025", DateFin = "31/01/2026", ProchainRenouvellement = "01/02/2026", Statut = "Actif"     },
            new ClientAbonnementDTO { Id = 5,  ClientId = 2,  TypeAbonnement = "Microsoft 365 E5",                Quantite = 3,  DateDebut = "01/04/2024", DateFin = "31/03/2026", ProchainRenouvellement = "01/04/2026", Statut = "Actif"     },
            new ClientAbonnementDTO { Id = 6,  ClientId = 3,  TypeAbonnement = "Microsoft 365 E3",                Quantite = 15, DateDebut = "10/01/2024", DateFin = "09/01/2027", ProchainRenouvellement = "10/01/2027", Statut = "Actif"     },
            new ClientAbonnementDTO { Id = 7,  ClientId = 3,  TypeAbonnement = "Microsoft 365 Business Premium",  Quantite = 7,  DateDebut = "01/07/2024", DateFin = "30/06/2026", ProchainRenouvellement = "01/07/2026", Statut = "Actif"     },
            new ClientAbonnementDTO { Id = 8,  ClientId = 4,  TypeAbonnement = "Microsoft 365 Business Standard", Quantite = 4,  DateDebut = "01/11/2023", DateFin = "31/10/2024", ProchainRenouvellement = "01/11/2024", Statut = "Expiré"    },
            new ClientAbonnementDTO { Id = 9,  ClientId = 5,  TypeAbonnement = "Microsoft 365 E5",                Quantite = 6,  DateDebut = "15/03/2025", DateFin = "14/03/2027", ProchainRenouvellement = "15/03/2027", Statut = "Actif"     },
            new ClientAbonnementDTO { Id = 10, ClientId = 6,  TypeAbonnement = "Microsoft 365 Business Standard", Quantite = 3,  DateDebut = "01/06/2025", DateFin = "31/05/2026", ProchainRenouvellement = "01/06/2026", Statut = "EnAttente" },
            new ClientAbonnementDTO { Id = 11, ClientId = 7,  TypeAbonnement = "Microsoft 365 Business Premium",  Quantite = 2,  DateDebut = "01/09/2024", DateFin = "31/08/2026", ProchainRenouvellement = "01/09/2026", Statut = "Actif"     },
            new ClientAbonnementDTO { Id = 12, ClientId = 8,  TypeAbonnement = "Microsoft 365 E3",                Quantite = 1,  DateDebut = "01/05/2023", DateFin = "30/04/2024", ProchainRenouvellement = "01/05/2024", Statut = "Expiré"    },
            new ClientAbonnementDTO { Id = 13, ClientId = 9,  TypeAbonnement = "Microsoft 365 Business Standard", Quantite = 6,  DateDebut = "01/01/2025", DateFin = "31/12/2026", ProchainRenouvellement = "01/01/2027", Statut = "Actif"     },
            new ClientAbonnementDTO { Id = 14, ClientId = 10, TypeAbonnement = "Microsoft 365 E5",                Quantite = 8,  DateDebut = "01/04/2025", DateFin = "31/03/2027", ProchainRenouvellement = "01/04/2027", Statut = "EnAttente" },
            new ClientAbonnementDTO { Id = 15, ClientId = 11, TypeAbonnement = "Microsoft 365 E3",                Quantite = 10, DateDebut = "01/02/2024", DateFin = "31/01/2025", ProchainRenouvellement = "01/02/2025", Statut = "Expiré"    },
            new ClientAbonnementDTO { Id = 16, ClientId = 12, TypeAbonnement = "Microsoft 365 Business Standard", Quantite = 5,  DateDebut = "01/07/2025", DateFin = "30/06/2027", ProchainRenouvellement = "01/07/2027", Statut = "Actif"     },
        };

        // ── Méthodes ──────────────────────────────────────────────────

        public List<ClientAbonnementDTO> GetTousLesAbonnements()
            => _abonnements.ToList();

        public List<ClientAbonnementDTO> GetAbonnementsByClientId(int clientId)
            => _abonnements.Where(a => a.ClientId == clientId).ToList();

        private static void CalculerDatesAbonnement(ClientAbonnementDTO abonnement, bool estCreation)
        {
            DateTime dateDebut;

            if (estCreation)
            {
                dateDebut = DateTime.Today;
            }
            else
            {
                bool dateValide = DateTime.TryParseExact(
                    abonnement.DateDebut,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dateDebut);

                if (!dateValide)
                    dateDebut = DateTime.Today;
            }

            DateTime dateFin = dateDebut.AddYears(1).AddDays(-1);
            DateTime prochainRenouvellement = dateFin.AddDays(1);

            abonnement.DateDebut = dateDebut.ToString("dd/MM/yyyy");
            abonnement.DateFin = dateFin.ToString("dd/MM/yyyy");
            abonnement.ProchainRenouvellement = prochainRenouvellement.ToString("dd/MM/yyyy");
        }

        public void AjouterAbonnement(ClientAbonnementDTO abonnement)
        {
            abonnement.Id = _abonnements.Any() ? _abonnements.Max(a => a.Id) + 1 : 1;

            CalculerDatesAbonnement(abonnement, true);

            _abonnements.Add(new ClientAbonnementDTO
            {
                Id = abonnement.Id,
                ClientId = abonnement.ClientId,
                TypeAbonnement = abonnement.TypeAbonnement,
                Quantite = abonnement.Quantite,
                DateDebut = abonnement.DateDebut,
                DateFin = abonnement.DateFin,
                ProchainRenouvellement = abonnement.ProchainRenouvellement,
                Statut = abonnement.Statut,
            });
        }

        public void ModifierAbonnement(ClientAbonnementDTO abonnement)
        {
            var existant = _abonnements.FirstOrDefault(a => a.Id == abonnement.Id);
            if (existant == null) return;

            existant.ClientId = abonnement.ClientId;
            existant.TypeAbonnement = abonnement.TypeAbonnement;
            existant.Quantite = abonnement.Quantite;
            existant.Statut = abonnement.Statut;

            if (string.IsNullOrWhiteSpace(existant.DateDebut))
                existant.DateDebut = abonnement.DateDebut;

            CalculerDatesAbonnement(existant, false);
        }

        public void SupprimerAbonnement(int id)
        {
            var abo = _abonnements.FirstOrDefault(a => a.Id == id);
            if (abo != null)
                _abonnements.Remove(abo);
        }

        public int GetTotalAbonnementsParClient(int clientId)
            => _abonnements.Where(a => a.ClientId == clientId).Sum(a => a.Quantite);

        public int GetTotalAbonnements()
            => _abonnements.Sum(a => a.Quantite);

        public int GetNombreAbonnementsActifs()
            => _abonnements.Where(a => a.Statut == "Actif").Sum(a => a.Quantite);

        public List<ClientAbonnementDTO> RechercherAbonnements(string texte)
        {
            texte = texte.ToLower();
            return _abonnements.Where(a =>
            {
                // ✅ Résout le nom du client depuis _clients
                _clients.TryGetValue(a.ClientId, out var client);
                string nomClient = client?.Nom?.ToLower() ?? string.Empty;

                return
                    a.Id.ToString().Contains(texte) ||
                    a.ClientId.ToString().Contains(texte) ||
                    nomClient.Contains(texte) ||
                    (a.TypeAbonnement ?? string.Empty).ToLower().Contains(texte) ||
                    a.Quantite.ToString().Contains(texte) ||
                    (a.DateDebut ?? string.Empty).Contains(texte) ||
                    (a.DateFin ?? string.Empty).Contains(texte) ||
                    (a.ProchainRenouvellement ?? string.Empty).Contains(texte) ||
                    (a.Statut ?? string.Empty).ToLower().Contains(texte);
            }).ToList();
        }
    }
}