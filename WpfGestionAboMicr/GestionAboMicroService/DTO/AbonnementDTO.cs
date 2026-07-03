using System;
using System.Collections.Generic;
using System.Text;

namespace GestionAboMicroService.DTO
{
    public class AbonnementDTO
    {
        public string TypeAbonnement { get; set; } = string.Empty;
        public double NombreAbos { get; set; }
        public double Total { get; set; }

        public double Percentage => Total > 0 ? (NombreAbos / Total) * 100 : 0;

    }
}