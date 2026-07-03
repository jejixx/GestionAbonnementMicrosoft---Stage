using System;
using System.Collections.Generic;
using System.Text;

namespace GestionAboMicroService.DTO
{
    /// <summary>
    /// DTO complet — utilisé pour la vue détail d'un client
    /// </summary>
    public class ClientDTO
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public int NombreAbonnements { get; set; }
        public string EtatFacturation { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;           // ✅ une seule 'd'
        public string ContactPrincipal { get; set; } = string.Empty;
        public string ProchainRenouvellement { get; set; } = string.Empty;
    }



    /// <summary>
    /// DTO allégé — utilisé pour la liste/tableau des clients
    /// Contient uniquement ce qui est affiché dans la grille
    /// </summary>
    public class ClientSummary
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public int NombreAbonnements { get; set; }
        public string EtatFacturation { get; set; } = string.Empty;
    }


}
