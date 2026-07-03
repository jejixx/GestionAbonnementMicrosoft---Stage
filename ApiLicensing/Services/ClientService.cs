using ApiLicensing.DTO;

namespace ApiLicensing.Services
{
    public partial class AppService
    {
        // ── Données mock locales ──────────────────────────────────────
        private readonly Dictionary<int, ClientDTO> _clients = new()
        {
            [1] = new ClientDTO { Id = 1, Nom = "Alpha Solutions", Email = "contact@alpha-solutions.mock", Telephone = "01 23 45 67 89", EtatFacturation = "AJour", Adresse = "12 rue des Mockups, 75000 Paris", ContactPrincipal = "Jean Dupont", ProchainRenouvellement = "15/09/2026" },
            [2] = new ClientDTO { Id = 2, Nom = "Beta Conseil", Email = "info@beta-conseil.mock", Telephone = "09 87 65 43 21", EtatFacturation = "EnAttente", Adresse = "8 avenue des Tests, 69000 Lyon", ContactPrincipal = "Marie Martin", ProchainRenouvellement = "01/07/2026" },
            [3] = new ClientDTO { Id = 3, Nom = "Gamma Industrie", Email = "contact@gamma-industrie.mock", Telephone = "02 12 34 56 78", EtatFacturation = "AJour", Adresse = "3 impasse des Données, 44000 Nantes", ContactPrincipal = "Pierre Bernard", ProchainRenouvellement = "20/11/2026" },
            [4] = new ClientDTO { Id = 4, Nom = "Delta Services", Email = "contact@delta-services.mock", Telephone = "01 23 34 56 78", EtatFacturation = "EnRetard", Adresse = "45 boulevard du Code, 75008 Paris", ContactPrincipal = "Sophie Leroy", ProchainRenouvellement = "10/03/2025" },
            [5] = new ClientDTO { Id = 5, Nom = "Epsilon Informatique", Email = "bonjour@epsilon-info.mock", Telephone = "01 12 54 76 59", EtatFacturation = "AJour", Adresse = "17 rue de la Compile, 31000 Toulouse", ContactPrincipal = "Lucas Moreau", ProchainRenouvellement = "05/10/2026" },
            [6] = new ClientDTO { Id = 6, Nom = "Zeta Corp", Email = "contact@zeta-corp.mock", Telephone = "05 44 55 66 77", EtatFacturation = "EnAttente", Adresse = "2 allée des Branches, 33000 Bordeaux", ContactPrincipal = "Emma Petit", ProchainRenouvellement = "18/07/2026" },
            [7] = new ClientDTO { Id = 7, Nom = "Eta Solutions", Email = "contact@eta-solutions.mock", Telephone = "06 52 63 74 85", EtatFacturation = "AJour", Adresse = "9 rue du Déploiement, 67000 Strasbourg", ContactPrincipal = "Hugo Simon", ProchainRenouvellement = "30/12/2026" },
            [8] = new ClientDTO { Id = 8, Nom = "Theta Group", Email = "contact@theta-group.mock", Telephone = "07 52 75 84 96", EtatFacturation = "EnRetard", Adresse = "22 place du Serveur, 59000 Lille", ContactPrincipal = "Camille Rousseau", ProchainRenouvellement = "01/01/2025" },
            [9] = new ClientDTO { Id = 9, Nom = "Iota Digital", Email = "contact@iota-digital.mock", Telephone = "01 45 78 96 32", EtatFacturation = "AJour", Adresse = "56 rue des Variables, 06000 Nice", ContactPrincipal = "Nathan Girard", ProchainRenouvellement = "14/08/2026" },
            [10] = new ClientDTO { Id = 10, Nom = "Kappa Tech", Email = "contact@kappa-tech.mock", Telephone = "03 11 22 33 44", EtatFacturation = "EnAttente", Adresse = "11 rue du Framework, 21000 Dijon", ContactPrincipal = "Léa Lambert", ProchainRenouvellement = "22/07/2026" },
            [11] = new ClientDTO { Id = 11, Nom = "Lambda Expert", Email = "contact@lambda-expert.mock", Telephone = "04 98 76 54 32", EtatFacturation = "EnRetard", Adresse = "34 avenue du Repository, 13000 Marseille", ContactPrincipal = "Tom Fontaine", ProchainRenouvellement = "05/11/2024" },
            [12] = new ClientDTO { Id = 12, Nom = "Mu Services", Email = "contact@mu-services.mock", Telephone = "06 10 20 30 40", EtatFacturation = "AJour", Adresse = "7 chemin des Commits, 35000 Rennes", ContactPrincipal = "Inès Chevalier", ProchainRenouvellement = "10/01/2027" },
        };

        // ── Méthodes ──────────────────────────────────────────────────

        // NombreAbonnements calculé en temps réel depuis _abonnements
        public List<ClientDTO> GetClients(string? recherche = null)
        {
            if (!string.IsNullOrWhiteSpace(recherche)) return RechercherClients(recherche);
            return _clients.Values.Select(c =>
            {
                var copie = CopierClient(c);
                copie.NombreAbonnements = _abonnements
                    .Where(a => a.ClientId == c.Id)
                    .Sum(a => a.Quantite);
                return copie;
            }).ToList();
        }


        public List<ClientSummary> GetClientsSummary(string? recherche = null)
        {
            var clients = string.IsNullOrWhiteSpace(recherche)
                ? _clients.Values
                : _clients.Values.Where(c =>
                    c.Nom.ToLower().Contains(recherche.ToLower()) ||
                    c.Email.ToLower().Contains(recherche.ToLower()));

            return clients.Select(c => new ClientSummary
            {
                Id = c.Id,
                Nom = c.Nom,
                Email = c.Email,
                Telephone = c.Telephone,
                NombreAbonnements = _abonnements.Where(a => a.ClientId == c.Id).Sum(a => a.Quantite),
                EtatFacturation = c.EtatFacturation,
            }).ToList();
        }

        public ClientDTO? GetClientById(int id)
        {
            _clients.TryGetValue(id, out var c);
            if (c == null) return null;
            var copie = CopierClient(c);
            copie.NombreAbonnements = _abonnements
                .Where(a => a.ClientId == id)
                .Sum(a => a.Quantite);
            return copie;
        }

        private List<ClientDTO> RechercherClients(string texte)
        {
            if (string.IsNullOrWhiteSpace(texte))
                return GetClients();

            texte = texte.ToLower();
            return _clients.Values
                .Where(c =>
                    c.Id.ToString().Contains(texte) ||
                    c.Nom.ToLower().Contains(texte) ||
                    c.Email.ToLower().Contains(texte) ||
                    c.Telephone.Contains(texte) ||
                    c.EtatFacturation.ToLower().Contains(texte))
                .Select(c =>
                {
                    var copie = CopierClient(c);
                    copie.NombreAbonnements = _abonnements
                        .Where(a => a.ClientId == c.Id)
                        .Sum(a => a.Quantite);
                    return copie;
                })
                .ToList();
        }

        public ClientDTO AjouterClient(ClientDTO client)
        {
            int nouvelId = _clients.Keys.Any() ? _clients.Keys.Max() + 1 : 1;
            client.Id = nouvelId;
            _clients[nouvelId] = CopierClient(client);
            return client;  // ✅ retourne avec l'Id
        }

        public void ModifierClient(ClientDTO client)
        {
            if (_clients.ContainsKey(client.Id))
                _clients[client.Id] = CopierClient(client);
        }

        public bool SupprimerClient(int id, bool force = false)
        {
            int total = GetTotalAbonnementsParClient(id);

            if (total > 0 && !force)
                return false; // ❌ Refus sans confirmation explicite

            _abonnements.RemoveAll(a => a.ClientId == id); // Cascade
            _clients.Remove(id);
            return true;
        }

        public int GetTotalClients()
            => _clients.Count;

        private static ClientDTO CopierClient(ClientDTO c) => new()
        {
            Id = c.Id,
            Nom = c.Nom,
            Email = c.Email,
            Telephone = c.Telephone,
            NombreAbonnements = c.NombreAbonnements,
            EtatFacturation = c.EtatFacturation,
            Adresse = c.Adresse,
            ContactPrincipal = c.ContactPrincipal,
            ProchainRenouvellement = c.ProchainRenouvellement,
        };
    }
}