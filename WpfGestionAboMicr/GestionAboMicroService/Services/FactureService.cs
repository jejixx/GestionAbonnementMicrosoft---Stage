using GestionAboMicroService.DTO;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GestionAboMicroService.Services
{
    public partial class AppService
    {
        // ── Lecture clients ───────────────────────────────────────────────

        public async Task<List<FactureClientDTO>> GetToutesLesFactures()
        {
            try
            {
                var response = await _http.GetAsync("/api/factures");
                await VerifierReponseAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var liste = JsonSerializer.Deserialize<List<FactureClientDTO>>(json, _jsonOptions) ?? new();

                System.Diagnostics.Debug.WriteLine($"✅ [Factures] GetToutesLesFactures → {liste.Count} facture(s)");
                return liste;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Factures] GetToutesLesFactures → {ex.Message}");
                throw;
            }
        }

        public async Task<List<FactureClientDTO>> GetFacturesByClientId(int clientId)
        {
            try
            {
                var response = await _http.GetAsync($"/api/factures/client/{clientId}");
                await VerifierReponseAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var liste = JsonSerializer.Deserialize<List<FactureClientDTO>>(json, _jsonOptions) ?? new();

                System.Diagnostics.Debug.WriteLine($"✅ [Factures] GetFacturesByClientId({clientId}) → {liste.Count} facture(s)");
                return liste;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Factures] GetFacturesByClientId({clientId}) → {ex.Message}");
                throw;
            }
        }

        public async Task<int> GetNombreFacturesEnAttente()
        {
            try
            {
                var response = await _http.GetAsync("/api/factures/attente/total");
                await VerifierReponseAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var total = JsonSerializer.Deserialize<int>(json, _jsonOptions);

                System.Diagnostics.Debug.WriteLine($"✅ [Factures] GetNombreFacturesEnAttente → {total}");
                return total;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Factures] GetNombreFacturesEnAttente → {ex.Message}");
                throw;
            }
        }

        // ── Écriture clients ──────────────────────────────────────────────

        public async Task AjouterFacture(FactureClientDTO f)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(f, _jsonOptions), Encoding.UTF8, "application/json");
                var response = await _http.PostAsync("/api/factures", content);
                await VerifierReponseAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var retour = JsonSerializer.Deserialize<FactureClientDTO>(json, _jsonOptions);
                if (retour is not null) f.Id = retour.Id;

                System.Diagnostics.Debug.WriteLine($"✅ [Factures] AjouterFacture → OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Factures] AjouterFacture → {ex.Message}");
                throw;
            }
        }

        public async Task ModifierFacture(FactureClientDTO f)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(f, _jsonOptions), Encoding.UTF8, "application/json");
                var response = await _http.PutAsync("/api/factures", content);
                await VerifierReponseAsync(response);

                System.Diagnostics.Debug.WriteLine($"✅ [Factures] ModifierFacture({f.Id}) → OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Factures] ModifierFacture({f.Id}) → {ex.Message}");
                throw;
            }
        }

        public async Task SupprimerFacture(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"/api/factures/{id}");
                await VerifierReponseAsync(response);

                System.Diagnostics.Debug.WriteLine($"✅ [Factures] SupprimerFacture({id}) → OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Factures] SupprimerFacture({id}) → {ex.Message}");
                throw;
            }
        }

        // ── Lecture fournisseurs ──────────────────────────────────────────

        public async Task<List<FactureFournisseurDTO>> GetToutesLesFacturesFournisseurs()
        {
            try
            {
                var response = await _http.GetAsync("/api/factures/fournisseurs");
                await VerifierReponseAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var liste = JsonSerializer.Deserialize<List<FactureFournisseurDTO>>(json, _jsonOptions) ?? new();

                System.Diagnostics.Debug.WriteLine($"✅ [Factures] GetToutesLesFacturesFournisseurs → {liste.Count} facture(s)");
                return liste;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Factures] GetToutesLesFacturesFournisseurs → {ex.Message}");
                throw;
            }
        }

        // ── Écriture fournisseurs ─────────────────────────────────────────

        public async Task AjouterFactureFournisseur(FactureFournisseurDTO f)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(f, _jsonOptions), Encoding.UTF8, "application/json");
                var response = await _http.PostAsync("/api/factures/fournisseurs", content);
                await VerifierReponseAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var retour = JsonSerializer.Deserialize<FactureFournisseurDTO>(json, _jsonOptions);
                if (retour is not null) f.Id = retour.Id;

                System.Diagnostics.Debug.WriteLine($"✅ [Factures] AjouterFactureFournisseur → OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Factures] AjouterFactureFournisseur → {ex.Message}");
                throw;
            }
        }

        public async Task ModifierFactureFournisseur(FactureFournisseurDTO f)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(f, _jsonOptions), Encoding.UTF8, "application/json");
                var response = await _http.PutAsync("/api/factures/fournisseurs", content);
                await VerifierReponseAsync(response);

                System.Diagnostics.Debug.WriteLine($"✅ [Factures] ModifierFactureFournisseur({f.Id}) → OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Factures] ModifierFactureFournisseur({f.Id}) → {ex.Message}");
                throw;
            }
        }

        public async Task SupprimerFactureFournisseur(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"/api/factures/fournisseurs/{id}");
                await VerifierReponseAsync(response);

                System.Diagnostics.Debug.WriteLine($"✅ [Factures] SupprimerFactureFournisseur({id}) → OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Factures] SupprimerFactureFournisseur({id}) → {ex.Message}");
                throw;
            }
        }

        public async Task<List<LigneInfoDTO>> GetLignesResume()
        {
            try
            {
                var response = await _http.GetAsync("/api/factures/resume");
                await VerifierReponseAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                var liste = JsonSerializer.Deserialize<List<LigneInfoDTO>>(json, _jsonOptions) ?? new();

                System.Diagnostics.Debug.WriteLine($"✅ [Factures] GetLignesResume → {liste.Count} ligne(s)");
                return liste;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [Factures] GetLignesResume → {ex.Message}");
                throw;
            }
        }
    }
}