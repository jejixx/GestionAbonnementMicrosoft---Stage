using GestionAboMicroService.DTO;
using GestionAboMicroService.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace WpfGestionAboMicr.UserControls.Vue
{
    public partial class ClientAbonnements : UserControl
    {
        private readonly AppService _clientService = App.AppService;
        private List<ClientAbonnementDTO> _tousLesAbonnements = new();
        private List<ClientDTO> _clients = new();

        public ClientAbonnements()
        {
            InitializeComponent();

            CboElementsParPage.ItemsSource = new[] { 5, 10, 20, 50 };
            CboElementsParPage.SelectedItem = 10;

            Loaded += ClientAbonnements_Loaded;
        }

        private async void ClientAbonnements_Loaded(object sender, RoutedEventArgs e)
        {
            await ChargerClientsAsync();
            await ChargerAsync();
        }

        #region Chargement

        private async Task ChargerClientsAsync()
        {
            try
            {
                _clients = await _clientService.GetClients();
                ColClient.ItemsSource = _clients;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] ChargerClientsAsync → {ex.Message}");
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ChargerAsync()
        {
            try
            {
                _tousLesAbonnements = await _clientService.GetTousLesAbonnements();
                DataPager.Source = new ObservableCollection<ClientAbonnementDTO>(_tousLesAbonnements);
                DataPager.PageIndex = 0;
                MettreAJourTexte();
                BtnEnregistrer.IsEnabled = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] ChargerAsync → {ex.Message}");
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MettreAJourTexte()
        {
            if (DataPager.Source is not IEnumerable<ClientAbonnementDTO> source)
            {
                TxtPagination.Text = "Aucun résultat";
                return;
            }

            int total = source.Count();
            int pageSize = (int)DataPager.PageSize;

            if (total == 0)
            {
                TxtPagination.Text = "Aucun résultat";
                return;
            }

            int debut = DataPager.PageIndex * pageSize + 1;
            int fin = Math.Min(debut + pageSize - 1, total);
            TxtPagination.Text = $"{debut}-{fin} sur {total}";
        }

        private List<ClientAbonnementDTO> GetPageCourante()
        {
            var result = new List<ClientAbonnementDTO>();

            if (AbonnementsGrid.View?.Records == null)
                return result;

            foreach (var record in AbonnementsGrid.View.Records)
            {
                if (record.Data is ClientAbonnementDTO dto)
                    result.Add(dto);
            }

            return result;
        }

        #endregion

        #region Pagination

        private void DataPager_PageIndexChanged(object sender,
            Syncfusion.UI.Xaml.Controls.DataPager.PageIndexChangedEventArgs e)
        {
            MettreAJourTexte();
        }

        private async void CboElementsParPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboElementsParPage.SelectedItem is int taille)
            {
                DataPager.PageSize = taille;
                await ChargerAsync();
            }
        }

        #endregion

        #region Barre d'actions

        private async void TxtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string texte = TxtRecherche.Text.Trim();

                var liste = string.IsNullOrWhiteSpace(texte)
                    ? _tousLesAbonnements
                    : await _clientService.RechercherAbonnements(texte);

                DataPager.Source = new ObservableCollection<ClientAbonnementDTO>(liste);
                DataPager.PageIndex = 0;
                MettreAJourTexte();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] TxtRecherche_TextChanged → {ex.Message}");
            }
        }

        private bool ValiderAbonnement(ClientAbonnementDTO abo)
        {
            if (abo.ClientId <= 0)
            {
                MessageBox.Show("Le client est obligatoire.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(abo.TypeAbonnement))
            {
                MessageBox.Show("Le produit est obligatoire.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (abo.Quantite <= 0)
            {
                MessageBox.Show("La quantité doit être supérieure à 0.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(abo.Statut))
            {
                MessageBox.Show("Le statut est obligatoire.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private async void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var pageVisible = GetPageCourante();

                foreach (var abo in pageVisible)
                {
                    if (!ValiderAbonnement(abo))
                        return;
                }

                foreach (var abo in pageVisible.Where(a => a.Id == 0).ToList())
                {
                    await _clientService.AjouterAbonnement(abo);
                }

                foreach (var abo in pageVisible.Where(a => a.Id > 0).ToList())
                {
                    await _clientService.ModifierAbonnement(abo);
                }

                await ChargerAsync();

                MessageBox.Show("Enregistrement effectué.", "Succès",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] BtnEnregistrer_Click → {ex.Message}");
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnActualiser_Click(object sender, RoutedEventArgs e)
        {
            TxtRecherche.Text = string.Empty;
            await ChargerClientsAsync();
            await ChargerAsync();
        }

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            var nouvel = new ClientAbonnementDTO
            {
                Id = 0,
                ClientId = 0,
                TypeAbonnement = string.Empty,
                Quantite = 1,
                DateDebut = string.Empty,
                DateFin = string.Empty,
                ProchainRenouvellement = string.Empty,
                Statut = "Actif"
            };

            _tousLesAbonnements.Insert(0, nouvel);
            DataPager.Source = new ObservableCollection<ClientAbonnementDTO>(_tousLesAbonnements);
            DataPager.PageIndex = 0;
            TxtRecherche.Text = string.Empty;
            MettreAJourTexte();

            AbonnementsGrid.SelectedIndex = 0;
            BtnEnregistrer.IsEnabled = true;
        }

        private async void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AbonnementsGrid.SelectedItem is not ClientAbonnementDTO abo)
                {
                    MessageBox.Show("Sélectionnez un abonnement à supprimer.",
                        "Suppression", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var confirm = MessageBox.Show(
                    $"Supprimer l'abonnement \"{abo.TypeAbonnement}\" ?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirm != MessageBoxResult.Yes)
                    return;

                if (abo.Id == 0)
                {
                    _tousLesAbonnements.Remove(abo);
                    DataPager.Source = new ObservableCollection<ClientAbonnementDTO>(_tousLesAbonnements);
                    MettreAJourTexte();
                }
                else
                {
                    await _clientService.SupprimerAbonnement(abo.Id);
                    await ChargerAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] BtnSupprimer_Click → {ex.Message}");
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Édition inline

        private void AbonnementsGrid_CurrentCellEndEdit(object sender,
            Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            BtnEnregistrer.IsEnabled = true;
        }

        #endregion
    }
}