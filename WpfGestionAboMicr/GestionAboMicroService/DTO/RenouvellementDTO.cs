using System;
using System.Collections.Generic;
using System.Text;

namespace GestionAboMicroService.DTO
{
    public class RenouvellementDTO
    {
        public string Client { get; set; } = string.Empty;
        public string DateRenouvellement { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty; // "Actif", "EnRetard", "Bientot"
    }
}
