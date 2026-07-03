using System;
using System.Collections.Generic;
using System.Text;

namespace GestionAboMicroService.DTO
{
    // HistoriqueDTO.cs
    public class HistoriqueDTO
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Date { get; set; } = "";
        public string Action { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
