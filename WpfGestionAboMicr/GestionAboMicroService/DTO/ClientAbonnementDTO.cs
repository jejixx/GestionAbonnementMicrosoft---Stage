using System;
using System.Collections.Generic;
using System.Text;

namespace GestionAboMicroService.DTO
{
    public class ClientAbonnementDTO
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string TypeAbonnement { get; set; } = string.Empty;
        public int Quantite { get; set; }
        public string DateDebut { get; set; } = string.Empty;
        public string DateFin { get; set; } = string.Empty;
        public string ProchainRenouvellement { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
    }
}
