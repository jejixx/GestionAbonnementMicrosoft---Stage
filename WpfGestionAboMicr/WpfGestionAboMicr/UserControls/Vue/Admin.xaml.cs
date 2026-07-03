using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfGestionAboMicr.UserControls.Vue
{
    public partial class Admin : UserControl
    {
        private readonly HttpClient _http = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7085")
        };

        public Admin()
        {
            InitializeComponent();
        }

        private async Task ExecuterTestAsync(string url, TextBlock txtStatut)
        {
            txtStatut.Text = "⏳ Chargement...";
            txtStatut.Foreground = Brushes.Gray;
            TxtReponse.Text = "";

            try
            {
                var reponse = await _http.GetAsync(url);
                var contenu = await reponse.Content.ReadAsStringAsync();

                if (reponse.IsSuccessStatusCode)
                {
                    txtStatut.Text = $"✅ {(int)reponse.StatusCode} {reponse.StatusCode}";
                    txtStatut.Foreground = Brushes.Green;
                    TxtReponse.Text = string.IsNullOrWhiteSpace(contenu)
                        ? "Requête réussie."
                        : contenu;
                }
                else
                {
                    txtStatut.Foreground = Brushes.Red;

                    switch (reponse.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            txtStatut.Text = "❌ 400 BadRequest";
                            break;

                        case HttpStatusCode.NotFound:
                            txtStatut.Text = "❌ 404 NotFound";
                            break;

                        case HttpStatusCode.InternalServerError:
                            txtStatut.Text = "❌ 500 InternalServerError";
                            break;

                        default:
                            txtStatut.Text = $"❌ {(int)reponse.StatusCode} {reponse.StatusCode}";
                            break;
                    }



                    TxtReponse.Text = string.IsNullOrWhiteSpace(contenu)
                        ? "L'API a retourné une erreur sans message."
                        : contenu;


                }
            }
            catch (Exception ex)
            {
                txtStatut.Text = "❌ Erreur de connexion";
                txtStatut.Foreground = Brushes.Red;
                TxtReponse.Text = ex.Message;
            }
        }

        private async void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            await ExecuterTestAsync("/api/clients", TxtStatutClients);
        }

        private async void BtnAbonnements_Click(object sender, RoutedEventArgs e)
        {
            await ExecuterTestAsync("/api/abonnements", TxtStatutAbonnements);
        }

        private async void BtnFactures_Click(object sender, RoutedEventArgs e)
        {
            await ExecuterTestAsync("/api/factures", TxtStatutFactures);
        }

        private async void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            await ExecuterTestAsync("/api/test/ok", TxtStatutOk);
        }

        private async void BtnBadRequest_Click(object sender, RoutedEventArgs e)
        {
            await ExecuterTestAsync("/api/test/badrequest", TxtStatutBadRequest);
        }

        private async void BtnNotFound_Click(object sender, RoutedEventArgs e)
        {
            await ExecuterTestAsync("/api/test/notfound", TxtStatutNotFound);
        }

        private async void BtnError_Click(object sender, RoutedEventArgs e)
        {
            await ExecuterTestAsync("/api/test/error", TxtStatutError);
        }
    }
}