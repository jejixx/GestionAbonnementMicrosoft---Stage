using System;
using System.Collections.Generic;
using System.Text;

namespace GestionAboMicroService.DTO
{
    public class LigneInfoDTO
    {
        public string? Label { get; set; }
        public string? Valeur { get; set; }
        public TypeValeur TypeValeur { get; set; } = TypeValeur.Neutre;
    }

    public enum TypeValeur { Neutre, Positif, Negatif, Info }
}