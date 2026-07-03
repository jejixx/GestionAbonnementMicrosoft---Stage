using System.Text;
using System.Text.Json;
using GestionAboMicroService.DTO;

namespace GestionAboMicroService.Services
{
    public partial class AppService
    {
        public async Task<List<ClientAbonnementDTO>> GetTousLesAbonnements()
        {
            try
            {
                var response = await _http.GetAsync("/api/abonnements");
                await VerifierReponseAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var liste = JsonSerializer.Deserialize<List<ClientAbonnementDTO>>(json, _jsonOptions) ?? new List<ClientAbonnementDTO>();

                System.Diagnostics.Debug.WriteLine($"✅ [Abonnements] GetTousLesAbonnements → {liste.Count} abonnement(s)");
                return liste;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Abonnements] GetTousLesAbonnements → {ex.Message}");
                throw;
            }
        }

        public async Task<List<ClientAbonnementDTO>> GetAbonnementsByClientId(int clientId)
        {
            try
            {
                var response = await _http.GetAsync($"/api/abonnements/client/{clientId}");
                await VerifierReponseAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var liste = JsonSerializer.Deserialize<List<ClientAbonnementDTO>>(json, _jsonOptions) ?? new List<ClientAbonnementDTO>();

                System.Diagnostics.Debug.WriteLine($"✅ [Abonnements] GetAbonnementsByClientId({clientId}) → {liste.Count} abonnement(s)");
                return liste;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Abonnements] GetAbonnementsByClientId({clientId}) → {ex.Message}");
                throw;
            }
        }

        public async Task<List<ClientAbonnementDTO>> RechercherAbonnements(string texte)
        {
            try
            {
                string url = $"/api/abonnements/recherche?q={Uri.EscapeDataString(texte)}";
                var response = await _http.GetAsync(url);
                await VerifierReponseAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var liste = JsonSerializer.Deserialize<List<ClientAbonnementDTO>>(json, _jsonOptions) ?? new List<ClientAbonnementDTO>();

                System.Diagnostics.Debug.WriteLine($"✅ [Abonnements] RechercherAbonnements(\"{texte}\") → {liste.Count} résultat(s)");
                return liste;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Abonnements] RechercherAbonnements(\"{texte}\") → {ex.Message}");
                throw;
            }
        }

        public async Task<int> GetTotalAbonnements()
        {
            try
            {
                var response = await _http.GetAsync("/api/abonnements/total");
                await VerifierReponseAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var total = JsonSerializer.Deserialize<int>(json, _jsonOptions);

                System.Diagnostics.Debug.WriteLine($"✅ [Abonnements] GetTotalAbonnements → {total}");
                return total;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Abonnements] GetTotalAbonnements → {ex.Message}");
                throw;
            }
        }

        public async Task<int> GetTotalAbonnementsParClient(int clientId)
        {
            try
            {
                var response = await _http.GetAsync($"/api/abonnements/client/{clientId}/total");
                await VerifierReponseAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var total = JsonSerializer.Deserialize<int>(json, _jsonOptions);

                System.Diagnostics.Debug.WriteLine($"✅ [Abonnements] GetTotalAbonnementsParClient({clientId}) → {total}");
                return total;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Abonnements] GetTotalAbonnementsParClient({clientId}) → {ex.Message}");
                throw;
            }
        }

        public async Task<ClientAbonnementDTO?> AjouterAbonnement(ClientAbonnementDTO abonnement)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(abonnement, _jsonOptions),
                    Encoding.UTF8,
                    "application/json");

                var response = await _http.PostAsync("/api/abonnements", content);
                await VerifierReponseAsync(response);

                var body = await response.Content.ReadAsStringAsync();
                var resultat = JsonSerializer.Deserialize<ClientAbonnementDTO>(body, _jsonOptions);

                System.Diagnostics.Debug.WriteLine($"✅ [Abonnements] AjouterAbonnement → OK");
                return resultat;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Abonnements] AjouterAbonnement → {ex.Message}");
                throw;
            }
        }

        public async Task ModifierAbonnement(ClientAbonnementDTO abonnement)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(abonnement, _jsonOptions),
                    Encoding.UTF8,
                    "application/json");

                var response = await _http.PutAsync("/api/abonnements", content);
                await VerifierReponseAsync(response);

                System.Diagnostics.Debug.WriteLine($"✅ [Abonnements] ModifierAbonnement({abonnement.Id}) → OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Abonnements] ModifierAbonnement({abonnement.Id}) → {ex.Message}");
                throw;
            }
        }

        public async Task SupprimerAbonnement(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"/api/abonnements/{id}");
                await VerifierReponseAsync(response);

                System.Diagnostics.Debug.WriteLine($"✅ [Abonnements] SupprimerAbonnement({id}) → OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Abonnements] SupprimerAbonnement({id}) → {ex.Message}");
                throw;
            }
        }
    }
}