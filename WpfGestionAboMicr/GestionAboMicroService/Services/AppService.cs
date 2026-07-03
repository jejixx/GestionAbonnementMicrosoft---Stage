using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace GestionAboMicroService.Services
{
    public partial class AppService
    {
        private readonly HttpClient _http = new HttpClient()
        {
            BaseAddress = new Uri("https://localhost:7085")
        };

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private async Task VerifierReponseAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return;

            var contenu = await response.Content.ReadAsStringAsync();
            string messTmp;

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    messTmp = "Invalid request";
                    break;

                case HttpStatusCode.NotFound:
                    messTmp = "Not found";
                    break;

                case HttpStatusCode.InternalServerError:
                    messTmp = "Server error";
                    break;

                default:
                    messTmp = "Error";
                    break;
            }
            throw new Exception(messTmp);

        }
    }
}