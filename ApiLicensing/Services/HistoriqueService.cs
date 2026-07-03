using ApiLicensing.DTO;

namespace ApiLicensing.Services
{
    public partial class AppService
    {
        // ── Données mock locales (simule un endpoint GET /historique) ─
        private readonly List<HistoriqueDTO> _historique = new()
        {
            new HistoriqueDTO { Id = 1,  ClientId = 1,  Date = "01/01/2024", Action = "Création",   Description = "Création du compte client Alpha Solutions"      },
            new HistoriqueDTO { Id = 2,  ClientId = 1,  Date = "01/01/2024", Action = "Abonnement", Description = "Ajout Microsoft 365 Business Standard x10"      },
            new HistoriqueDTO { Id = 3,  ClientId = 1,  Date = "01/03/2024", Action = "Abonnement", Description = "Ajout Microsoft 365 Business Premium x5"        },
            new HistoriqueDTO { Id = 4,  ClientId = 1,  Date = "31/01/2024", Action = "Facture",    Description = "Facture FAC-2024-001 émise — 2 480 €"           },
            new HistoriqueDTO { Id = 5,  ClientId = 2,  Date = "01/02/2025", Action = "Création",   Description = "Création du compte client Beta Conseil"         },
            new HistoriqueDTO { Id = 6,  ClientId = 2,  Date = "01/04/2024", Action = "Abonnement", Description = "Ajout Microsoft 365 E5 x3"                      },
            new HistoriqueDTO { Id = 7,  ClientId = 3,  Date = "10/01/2024", Action = "Création",   Description = "Création du compte client Gamma Industrie"      },
            new HistoriqueDTO { Id = 8,  ClientId = 3,  Date = "10/01/2024", Action = "Abonnement", Description = "Ajout Microsoft 365 E3 x15"                     },
            new HistoriqueDTO { Id = 9,  ClientId = 4,  Date = "01/11/2023", Action = "Création",   Description = "Création du compte client Delta Services"       },
            new HistoriqueDTO { Id = 10, ClientId = 4,  Date = "01/11/2023", Action = "Abonnement", Description = "Ajout Microsoft 365 Business Standard x4"       },
            new HistoriqueDTO { Id = 11, ClientId = 5,  Date = "15/03/2025", Action = "Création",   Description = "Création du compte client Epsilon Informatique" },
            new HistoriqueDTO { Id = 12, ClientId = 6,  Date = "01/06/2025", Action = "Création",   Description = "Création du compte client Zeta Corp"            },
            new HistoriqueDTO { Id = 13, ClientId = 7,  Date = "01/09/2024", Action = "Création",   Description = "Création du compte client Eta Solutions"        },
            new HistoriqueDTO { Id = 14, ClientId = 8,  Date = "01/05/2023", Action = "Création",   Description = "Création du compte client Theta Group"          },
            new HistoriqueDTO { Id = 15, ClientId = 9,  Date = "01/01/2025", Action = "Création",   Description = "Création du compte client Iota Digital"         },
        };

        // ── Méthodes ──────────────────────────────────────────────────

        public List<HistoriqueDTO> GetHistoriqueByClientId(int clientId)
            => _historique.Where(h => h.ClientId == clientId).ToList();
    }
}