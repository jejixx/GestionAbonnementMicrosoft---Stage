using GestionAboMicroService.DTO;
using GestionAboMicroService.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfGestionAboMicr.UserControls.Vue
{
    public class EtatFacturationToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string etat ? etat switch
            {
                "AJour" => Brushes.Green,
                "EnAttente" => Brushes.Orange,
                "EnRetard" => Brushes.Red,
                _ => Brushes.Gray
            } : Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public partial class Clients : UserControl
    {
        private readonly AppService _clientService = App.AppService;

        public event Action<ClientDTO>? DetailClientRequested;
        public event Action? NouveauClientRequested;

        public Clients()
        {
            InitializeComponent();
            DataContext = this;

            CboElementsParPage.ItemsSource = new[] { 5, 10, 15, 20 };
            CboElementsParPage.SelectedItem = 10;

            Loaded += Clients_Loaded;
        }

        private async void Clients_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Clients_Loaded;
            await ChargerClientsAsync();
        }

        // ══════════════════════════════════════════════════════════════
        //  CHARGEMENT
        // ══════════════════════════════════════════════════════════════

        private async Task ChargerClientsAsync()
        {
            try
            {
                var liste = await _clientService.GetClientsSummary();
                DataPager.Source = new ObservableCollection<ClientSummary>(liste);
                DataPager.PageIndex = 0;
                MettreAJourTextePagination();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] ChargerClientsAsync → {ex.Message}");
                MessageBox.Show(ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task RechercherClientsAsync(string texte)
        {
            try
            {
                var liste = await _clientService.GetClientsSummary(texte);
                DataPager.Source = new ObservableCollection<ClientSummary>(liste);
                DataPager.PageIndex = 0;
                MettreAJourTextePagination();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] RechercherClientsAsync → {ex.Message}");
            }
        }

        public async Task RafraichirAsync()
        {
            await ChargerClientsAsync();
        }

        // ══════════════════════════════════════════════════════════════
        //  PAGINATION
        // ══════════════════════════════════════════════════════════════

        private void MettreAJourTextePagination()
        {
            int total = DataPager.Source?.Cast<object>().Count() ?? 0;
            int pageSize = DataPager.PageSize;
            int page = DataPager.PageIndex;

            if (total == 0)
            {
                TxtPagination.Text = "0-0 sur 0";
                return;
            }

            int debut = (page * pageSize) + 1;
            int fin = Math.Min((page + 1) * pageSize, total);
            TxtPagination.Text = $"{debut}-{fin} sur {total}";
        }

        private void DataPager_PageIndexChanged(object sender,
            Syncfusion.UI.Xaml.Controls.DataPager.PageIndexChangedEventArgs e)
        {
            MettreAJourTextePagination();
        }

        private void CboElementsParPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboElementsParPage.SelectedItem is int valeur)
            {
                DataPager.PageSize = valeur;
                MettreAJourTextePagination();
            }
        }

        // ══════════════════════════════════════════════════════════════
        //  TOOLBAR
        // ══════════════════════════════════════════════════════════════

        private async void TxtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texte = TxtRecherche.Text.Trim();

            if (string.IsNullOrWhiteSpace(texte))
                await ChargerClientsAsync();
            else
                await RechercherClientsAsync(texte);
        }

        private async void BtnActualiser_Click(object sender, RoutedEventArgs e)
        {
            TxtRecherche.TextChanged -= TxtRecherche_TextChanged;
            TxtRecherche.Text = string.Empty;
            TxtRecherche.TextChanged += TxtRecherche_TextChanged;

            await ChargerClientsAsync();
        }

        private void BtnNouveau_Click(object sender, RoutedEventArgs e)
        {
            NouveauClientRequested?.Invoke();
        }

        private async void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is not ClientSummary summary)
            {
                MessageBox.Show("Sélectionnez un client à modifier.", "Modification",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                ClientDTO client = await _clientService.GetClientById(summary.Id);
                DetailClientRequested?.Invoke(client);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] BtnModifier_Click → {ex.Message}");
                MessageBox.Show(ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is not ClientSummary summary)
                return;

            try
            {
                int total = await _clientService.GetTotalAbonnementsParClient(summary.Id);

                if (total == 0)
                {
                    var result = MessageBox.Show(
                        $"Supprimer le client \"{summary.Nom}\" ?",
                        "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result != MessageBoxResult.Yes)
                        return;

                    await _clientService.SupprimerClient(summary.Id);
                }
                else
                {
                    var result = MessageBox.Show(
                        $"⚠️ \"{summary.Nom}\" possède {total} abonnement(s).\n\n" +
                        "Confirmer la suppression du client ET de ses abonnements ?",
                        "Suppression en cascade",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result != MessageBoxResult.Yes)
                        return;

                    await _clientService.SupprimerClient(summary.Id, force: true);
                }

                await ChargerClientsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] BtnSupprimer_Click → {ex.Message}");
                MessageBox.Show(ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ClientsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ClientsGrid.SelectedItem is not ClientSummary summary)
                return;

            try
            {
                var client = await _clientService.GetClientById(summary.Id);
                DetailClientRequested?.Invoke(client);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] ClientsGrid_MouseDoubleClick → {ex.Message}");
                MessageBox.Show(ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}