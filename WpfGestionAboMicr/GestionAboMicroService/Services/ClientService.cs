using System.Net;
using System.Text;
using System.Text.Json;
using GestionAboMicroService.DTO;

namespace GestionAboMicroService.Services
{
    public partial class AppService
    {


        public async Task<int> GetTotalClients()
        {
            var response = await _http.GetAsync("/api/clients/total");
            await VerifierReponseAsync(response);

            var json = await response.Content.ReadAsStringAsync();
            var total = JsonSerializer.Deserialize<int>(json, _jsonOptions);

            System.Diagnostics.Debug.WriteLine($"✅ [Clients] GetTotalClients → {total}");
            return total;
        }

        public async Task<List<ClientDTO>> GetClients(string? recherche = null)
        {
            string url = string.IsNullOrWhiteSpace(recherche)
                ? "/api/clients"
                : $"/api/clients/recherche?q={Uri.EscapeDataString(recherche)}";

            var response = await _http.GetAsync(url);
            await VerifierReponseAsync(response);

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ClientDTO>>(json, _jsonOptions) ?? new();
        }

        public async Task<List<ClientSummary>> GetClientsSummary(string? recherche = null)
        {
            string url = string.IsNullOrWhiteSpace(recherche)
                ? "/api/clients/summary"
                : $"/api/clients/summary?q={Uri.EscapeDataString(recherche)}";

            var response = await _http.GetAsync(url);
            await VerifierReponseAsync(response);

            var json = await response.Content.ReadAsStringAsync();
            var liste = JsonSerializer.Deserialize<List<ClientSummary>>(json, _jsonOptions) ?? new();

            System.Diagnostics.Debug.WriteLine($"✅ [Summary] {liste.Count} client(s) chargé(s)");
            return liste;
        }

        public async Task<ClientDTO> GetClientById(int id)
        {
            var response = await _http.GetAsync($"/api/clients/{id}");
            await VerifierReponseAsync(response);

            var json = await response.Content.ReadAsStringAsync();
            var client = JsonSerializer.Deserialize<ClientDTO>(json, _jsonOptions);

            System.Diagnostics.Debug.WriteLine($"✅ [ClientDTO] Chargement complet Id={id}");
            return client!;
        }

        public async Task<ClientDTO?> AjouterClient(ClientDTO client)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(client, _jsonOptions),
                Encoding.UTF8, "application/json");

            var response = await _http.PostAsync("/api/clients", content);
            await VerifierReponseAsync(response);

            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ClientDTO>(body, _jsonOptions);
        }

        public async Task ModifierClient(ClientDTO client)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(client, _jsonOptions),
                Encoding.UTF8, "application/json");

            var response = await _http.PutAsync("/api/clients", content);
            await VerifierReponseAsync(response);
        }

        public async Task SupprimerClient(int id, bool force = false)
        {
            var response = await _http.DeleteAsync($"/api/clients/{id}?force={force}");
            await VerifierReponseAsync(response);
        }
    }
}