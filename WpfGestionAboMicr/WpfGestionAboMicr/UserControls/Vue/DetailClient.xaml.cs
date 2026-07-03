using GestionAboMicroService.DTO;
using GestionAboMicroService.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfGestionAboMicr.UserControls.Vue
{
    public partial class DetailClient : UserControl
    {
        private readonly AppService _clientService = App.AppService;
        private ObservableCollection<ClientAbonnementDTO> _abonnements = new();

        public event EventHandler? DemandeRetour;

        public DetailClient(ClientDTO? client = null)
        {
            InitializeComponent();

            var c = client ?? new ClientDTO { Id = 0 };
            DataContext = c;

            Loaded += DetailClient_Loaded;

            BtnEnregistrer.IsEnabled = c.Id == 0 ? false : true;
        }

        private async void DetailClient_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= DetailClient_Loaded;

            try
            {
                if (DataContext is ClientDTO client && client.Id > 0)
                {
                    await ChargerAbonnementsAsync();
                    await ChargerFacturationAsync();
                    await ChargerHistoriqueAsync();
                }
                else
                {
                    AbonnementsGrid.ItemsSource = _abonnements;
                    MettreAJourProchainRenouvellementClient();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] DetailClient_Loaded → {ex.Message}");
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Chargement

        private async Task ChargerAbonnementsAsync()
        {
            try
            {
                if (DataContext is not ClientDTO client || client.Id <= 0)
                {
                    AbonnementsGrid.ItemsSource = _abonnements;
                    MettreAJourProchainRenouvellementClient();
                    return;
                }

                var liste = await _clientService.GetAbonnementsByClientId(client.Id);
                _abonnements = new ObservableCollection<ClientAbonnementDTO>(liste);

                AbonnementsGrid.ItemsSource = _abonnements;
                MettreAJourProchainRenouvellementClient();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] ChargerAbonnementsAsync → {ex.Message}");
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ChargerFacturationAsync()
        {
            try
            {
                if (DataContext is not ClientDTO client || client.Id <= 0) return;

                var liste = await _clientService.GetFacturesByClientId(client.Id);
                FacturationGrid.ItemsSource = liste;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] ChargerFacturationAsync → {ex.Message}");
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ChargerHistoriqueAsync()
        {
            try
            {
                if (DataContext is not ClientDTO client || client.Id <= 0) return;

                var liste = await _clientService.GetHistoriqueByClientId(client.Id);
                HistoriqueList.ItemsSource = liste;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] ChargerHistoriqueAsync → {ex.Message}");
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Renouvellement

        private void MettreAJourProchainRenouvellementClient()
        {
            if (DataContext is not ClientDTO client) return;

            var dateLaPlusProche = _abonnements
                .Where(a => !string.IsNullOrWhiteSpace(a.ProchainRenouvellement))
                .Select(a =>
                {
                    bool ok = DateTime.TryParseExact(
                        a.ProchainRenouvellement,
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var date);

                    return new { Ok = ok, Date = date };
                })
                .Where(x => x.Ok)
                .OrderBy(x => x.Date)
                .Select(x => x.Date)
                .FirstOrDefault();

            client.ProchainRenouvellement = dateLaPlusProche == default
                ? string.Empty
                : dateLaPlusProche.ToString("dd/MM/yyyy");
        }

        #endregion

        #region Validation

        private bool ValiderClient(ClientDTO client)
        {
            if (string.IsNullOrWhiteSpace(client.Nom))
            {
                MessageBox.Show("Le nom du client est obligatoire.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(client.Email))
            {
                MessageBox.Show("L'email est obligatoire.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(client.Telephone))
            {
                MessageBox.Show("Le téléphone est obligatoire.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(client.ContactPrincipal))
            {
                MessageBox.Show("Le contact principal est obligatoire.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(client.Adresse))
            {
                MessageBox.Show("L'adresse est obligatoire.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(client.EtatFacturation))
            {
                MessageBox.Show("L'état de facturation est obligatoire.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private bool ValiderAbonnements()
        {
            foreach (var abo in _abonnements)
            {
                if (string.IsNullOrWhiteSpace(abo.TypeAbonnement))
                {
                    MessageBox.Show("Chaque abonnement doit avoir un produit.", "Validation",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (abo.Quantite <= 0)
                {
                    MessageBox.Show($"La quantité de \"{abo.TypeAbonnement}\" doit être > 0.", "Validation",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(abo.Statut))
                {
                    MessageBox.Show($"Le statut de \"{abo.TypeAbonnement}\" est obligatoire.", "Validation",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Barre d'actions

        private async void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not ClientDTO client) return;
            if (!ValiderClient(client)) return;
            if (!ValiderAbonnements()) return;

            try
            {
                MettreAJourProchainRenouvellementClient();

                if (client.Id == 0)
                {
                    var clientCree = await _clientService.AjouterClient(client);

                    if (clientCree == null || clientCree.Id == 0)
                    {
                        MessageBox.Show("Erreur : impossible de créer le client.", "Erreur",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    client.Id = clientCree.Id;
                    DataContext = clientCree;
                    client = clientCree;
                }
                else
                {
                    await _clientService.ModifierClient(client);
                }

                foreach (var abo in _abonnements.ToList())
                {
                    abo.ClientId = client.Id;

                    if (abo.Id == 0)
                        await _clientService.AjouterAbonnement(abo);
                    else
                        await _clientService.ModifierAbonnement(abo);
                }

                await ChargerAbonnementsAsync();
                MettreAJourProchainRenouvellementClient();
                BtnEnregistrer.IsEnabled = false;

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
            try
            {
                if (DataContext is ClientDTO client && client.Id > 0)
                {
                    var frais = await _clientService.GetClientById(client.Id);
                    if (frais != null)
                        DataContext = frais;
                }

                await ChargerAbonnementsAsync();
                await ChargerFacturationAsync();
                await ChargerHistoriqueAsync();
                MettreAJourProchainRenouvellementClient();
                BtnEnregistrer.IsEnabled = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] BtnActualiser_Click → {ex.Message}");
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnImprimer_Click(object sender, RoutedEventArgs e)
        {
            var printDlg = new PrintDialog();
            if (printDlg.ShowDialog() == true)
                printDlg.PrintVisual(this, "Client " + (DataContext as ClientDTO)?.Nom);
        }

        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            DemandeRetour?.Invoke(this, EventArgs.Empty);
        }

        private void OnClientChampModifie(object sender, TextChangedEventArgs e)
        {
            BtnEnregistrer.IsEnabled = true;
        }

        private void OnClientChampModifie_Combo(object sender, SelectionChangedEventArgs e)
        {
            BtnEnregistrer.IsEnabled = true;
        }

        #endregion

        #region Saisie numérique

        private void TxtNumerique_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void TxtNumerique_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox txt) return;

            txt.TextChanged -= TxtNumerique_TextChanged;

            var chiffres = new string(txt.Text.Where(char.IsDigit).ToArray());
            string formatted = txt.Tag?.ToString() switch
            {
                "telephone" => FormatTelephone(chiffres),
                _ => chiffres
            };

            if (txt.Text != formatted)
            {
                txt.Text = formatted;
                txt.CaretIndex = formatted.Length;
            }

            txt.TextChanged += TxtNumerique_TextChanged;
            BtnEnregistrer.IsEnabled = true;
        }

        private static string FormatTelephone(string chiffres)
        {
            var parts = new List<string>();

            for (int i = 0; i < chiffres.Length && i < 10; i += 2)
                parts.Add(chiffres.Substring(i, Math.Min(2, chiffres.Length - i)));

            return string.Join(" ", parts);
        }

        #endregion

        #region Abonnements

        private void BtnAjouterAbonnement_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not ClientDTO client) return;

            var nouvelAbo = new ClientAbonnementDTO
            {
                ClientId = client.Id,
                TypeAbonnement = string.Empty,
                Quantite = 1,
                Statut = "Actif",
                DateDebut = string.Empty,
                DateFin = string.Empty,
                ProchainRenouvellement = string.Empty
            };

            _abonnements.Insert(0, nouvelAbo);
            AbonnementsGrid.ItemsSource = _abonnements;
            AbonnementsGrid.SelectedItem = nouvelAbo;
            BtnEnregistrer.IsEnabled = true;
        }

        private async void BtnSupprimerAbonnement_Click(object sender, RoutedEventArgs e)
        {
            if (AbonnementsGrid.SelectedItem is not ClientAbonnementDTO abo)
            {
                MessageBox.Show("Sélectionnez un abonnement à supprimer.",
                    "Suppression", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Voulez-vous supprimer l'abonnement \"{abo.TypeAbonnement}\" ?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                if (abo.Id == 0)
                {
                    _abonnements.Remove(abo);
                    MettreAJourProchainRenouvellementClient();
                }
                else
                {
                    await _clientService.SupprimerAbonnement(abo.Id);
                    await ChargerAbonnementsAsync();
                }

                BtnEnregistrer.IsEnabled = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] BtnSupprimerAbonnement_Click → {ex.Message}");
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}