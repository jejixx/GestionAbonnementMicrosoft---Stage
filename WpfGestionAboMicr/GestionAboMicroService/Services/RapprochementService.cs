using GestionAboMicroService.DTO;
using System.Text.Json;

namespace GestionAboMicroService.Services
{
    public partial class AppService
    {
        public async Task<List<RapprochementDTO>> GetRapprochements()
        {
            try
            {
                var response = await _http.GetAsync("/api/rapprochement");
                await VerifierReponseAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var liste = JsonSerializer.Deserialize<List<RapprochementDTO>>(json, _jsonOptions) ?? new();

                System.Diagnostics.Debug.WriteLine($"✅ [Rapprochement] GetRapprochements → {liste.Count} entrée(s)");
                return liste;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Rapprochement] GetRapprochements → {ex.Message}");
                throw;
            }
        }
    }
}