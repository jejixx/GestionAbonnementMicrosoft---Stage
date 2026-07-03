using GestionAboMicroService.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GestionAboMicroService.Services
{
    public partial class AppService
    {
        public async Task<List<HistoriqueDTO>> GetHistoriqueByClientId(int clientId)
        {
            try
            {
                var response = await _http.GetAsync($"/api/historique/client/{clientId}");
                await VerifierReponseAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var liste = JsonSerializer.Deserialize<List<HistoriqueDTO>>(json, _jsonOptions) ?? new();

                System.Diagnostics.Debug.WriteLine($"✅ [Historique] GetHistoriqueByClientId({clientId}) → {liste.Count} entrée(s)");
                return liste;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Historique] GetHistoriqueByClientId({clientId}) → {ex.Message}");
                throw;
            }
        }
    }
}