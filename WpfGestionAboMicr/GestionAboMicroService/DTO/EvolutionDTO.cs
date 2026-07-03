using System;
using System.Collections.Generic;
using System.Text;

namespace GestionAboMicroService.DTO
{
    public class EvolutionDTO
    {
        public string Mois { get; set; } = string.Empty;
        public double NombreAbos { get; set; }
    }
}