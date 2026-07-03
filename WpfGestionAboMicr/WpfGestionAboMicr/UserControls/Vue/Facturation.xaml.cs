using GestionAboMicroService.DTO;
using GestionAboMicroService.Services;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfGestionAboMicr.UserControls.Vue
{
    public class StatutColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() switch
            {
                "Payée" => new SolidColorBrush(Color.FromRgb(22, 163, 74)),
                "EnAttente" => new SolidColorBrush(Color.FromRgb(217, 119, 6)),
                "EnRetard" => new SolidColorBrush(Color.FromRgb(220, 38, 38)),
                _ => new SolidColorBrush(Color.FromRgb(17, 24, 39))
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public partial class Facturation : UserControl
    {
        private readonly AppService _service = App.AppService;

        private List<FactureClientDTO> _facturesClients = new();
        private List<FactureFournisseurDTO> _facturesFournisseurs = new();
        private List<ClientDTO> _clients = new();
        private List<string> _nomsFournisseurs = new();

        private bool _busy = false;
        private bool _nouveau = false;

        private int _idClientSelectionne = -1;
        private int _idFournisseurSelectionne = -1;

        public Facturation()
        {
            InitializeComponent();
            Loaded += Facturation_Loaded;
        }

        private async void Facturation_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Facturation_Loaded;
            await ChargerAsync();
        }

        private async Task ChargerAsync()
        {
            try
            {
                _busy = true;
                _nouveau = false;

                _clients = await _service.GetClients();
                _facturesClients = await _service.GetToutesLesFactures();
                _facturesFournisseurs = await _service.GetToutesLesFacturesFournisseurs();

                _nomsFournisseurs = _facturesFournisseurs
                    .Select(f => f.Fournisseur ?? string.Empty)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Distinct()
                    .OrderBy(n => n)
                    .ToList();

                GridClients.ItemsSource = new ObservableCollection<FactureClientDTO>(_facturesClients);
                GridFourn.ItemsSource = new ObservableCollection<FactureFournisseurDTO>(_facturesFournisseurs);

                ModeEdition(false);

                if (TabFactures.SelectedIndex <= 0)
                {
                    PrepCboTiersClient();
                    RestaurerSelectionClient();
                }
                else
                {
                    PrepCboTiersFournisseur();
                    RestaurerSelectionFournisseur();
                }

                MajBoutons();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] Facturation ChargerAsync → {ex.Message}");
                MessageBox.Show(ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _busy = false;
            }
        }

        public Task RafraichirAsync() => ChargerAsync();

        private void RestaurerSelectionClient()
        {
            var cible = _idClientSelectionne > 0
                ? _facturesClients.FirstOrDefault(f => f.Id == _idClientSelectionne)
                : null;

            cible ??= _facturesClients.FirstOrDefault();

            if (cible != null)
            {
                GridClients.SelectedItem = cible;
                AfficherClient(cible);
            }
            else
            {
                Vider();
            }
        }

        private void RestaurerSelectionFournisseur()
        {
            var cible = _idFournisseurSelectionne > 0
                ? _facturesFournisseurs.FirstOrDefault(f => f.Id == _idFournisseurSelectionne)
                : null;

            cible ??= _facturesFournisseurs.FirstOrDefault();

            if (cible != null)
            {
                GridFourn.SelectedItem = cible;
                AfficherFourn(cible);
            }
            else
            {
                Vider();
            }
        }

        private void MajBoutons()
        {
            BtnNouveau.IsEnabled = !_nouveau;
            BtnSauvegarder.Visibility = _nouveau ? Visibility.Visible : Visibility.Collapsed;
            BtnAnnuler.Visibility = _nouveau ? Visibility.Visible : Visibility.Collapsed;
            TxtRecherche.IsEnabled = !_nouveau;
            TabFactures.IsEnabled = !_nouveau;
            BtnExporter.IsEnabled = !_nouveau;

            MajBtnMarquerPaye();
        }

        private void MajBtnMarquerPaye()
        {
            if (_nouveau)
            {
                BtnMarquerPaye.IsEnabled = false;
                return;
            }

            bool peutPayer = false;

            if (TabFactures.SelectedIndex == 0
                && GridClients.SelectedItem is FactureClientDTO fc
                && fc.Statut != "Payée")
            {
                peutPayer = true;
            }
            else if (TabFactures.SelectedIndex == 1
                && GridFourn.SelectedItem is FactureFournisseurDTO ff
                && ff.Statut != "Payée")
            {
                peutPayer = true;
            }

            BtnMarquerPaye.IsEnabled = peutPayer;
        }

        private async void BtnMarquerPaye_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TabFactures.SelectedIndex == 0)
                {
                    if (GridClients.SelectedItem is not FactureClientDTO f)
                        return;

                    f.Statut = "Payée";
                    f.DatePaiement = DateTime.Now.ToString("dd/MM/yyyy");

                    await _service.ModifierFacture(f);

                    _facturesClients = await _service.GetToutesLesFactures();
                    _idClientSelectionne = f.Id;
                    GridClients.ItemsSource = new ObservableCollection<FactureClientDTO>(_facturesClients);
                    RestaurerSelectionClient();
                }
                else
                {
                    if (GridFourn.SelectedItem is not FactureFournisseurDTO f)
                        return;

                    f.Statut = "Payée";
                    f.DatePaiement = DateTime.Now.ToString("dd/MM/yyyy");

                    await _service.ModifierFactureFournisseur(f);

                    _facturesFournisseurs = await _service.GetToutesLesFacturesFournisseurs();
                    _idFournisseurSelectionne = f.Id;
                    GridFourn.ItemsSource = new ObservableCollection<FactureFournisseurDTO>(_facturesFournisseurs);
                    RestaurerSelectionFournisseur();
                }

                MajBtnMarquerPaye();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] BtnMarquerPaye_Click → {ex.Message}");
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExporter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new SaveFileDialog
                {
                    Title = "Exporter",
                    Filter = "Fichier CSV (*.csv)|*.csv",
                    DefaultExt = "csv",
                    AddExtension = true,
                    FileName = TabFactures.SelectedIndex == 0
                        ? $"FacturesClients_{DateTime.Now:yyyyMMdd_HHmm}"
                        : $"FacturesFournisseurs_{DateTime.Now:yyyyMMdd_HHmm}"
                };

                if (dlg.ShowDialog() != true)
                    return;

                if (TabFactures.SelectedIndex == 0)
                    ExportCsvFacturesClients(dlg.FileName);
                else
                    ExportCsvFacturesFournisseurs(dlg.FileName);

                MessageBox.Show("Exportation réalisée avec succès.",
                    "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'exportation : " + ex.Message,
                    "Export", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportCsvFacturesClients(string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Nfacture;Client;Date;Échéance;Montant HT;TVA;Montant TTC;ModePaiement;Statut;Commentaire;DatePaiment");

            foreach (var f in _facturesClients)
            {
                sb.AppendLine(
                    $"{Esc(f.Numero)};" +
                    $"{Esc(f.Client)};" +
                    $"{Esc(f.Date)};" +
                    $"{Esc(f.DateEcheance)};" +
                    $"{Esc(f.MontantHT)};" +
                    $"{Esc(f.TVA)};" +
                    $"{Esc(f.MontantTTC)};" +
                    $"{Esc(f.ModePaiement)};" +
                    $"{Esc(f.Statut)};" +
                    $"{Esc(f.Commentaire)};" +
                    $"{Esc(f.DatePaiement)}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        private void ExportCsvFacturesFournisseurs(string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Nfacture;Fournisseur;Date;Échéance;Montant HT;TVA;Montant TTC;ModePaiement;Statut;Commentaire;DatePaiment");

            foreach (var f in _facturesFournisseurs)
            {
                sb.AppendLine(
                    $"{Esc(f.Numero)};" +
                    $"{Esc(f.Fournisseur)};" +
                    $"{Esc(f.Date)};" +
                    $"{Esc(f.DateEcheance)};" +
                    $"{Esc(f.MontantHT)};" +
                    $"{Esc(f.TVA)};" +
                    $"{Esc(f.MontantTTC)};" +
                    $"{Esc(f.ModePaiement)};" +
                    $"{Esc(f.Statut)};" +
                    $"{Esc(f.Commentaire)};" +
                    $"{Esc(f.DatePaiement)}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        private static string Esc(string? s)
            => (s ?? string.Empty)
                .Replace(";", ",")
                .Replace("\r", " ")
                .Replace("\n", " ");

        private void ModeEdition(bool on)
        {
            CboTiers.IsEnabled = on;
            TxtNumero.IsReadOnly = !on;
            TxtDate.IsReadOnly = !on;
            TxtEcheance.IsReadOnly = !on;
            TxtMontantHT.IsReadOnly = !on;
            TxtTVA.IsReadOnly = !on;
            TxtMontantTTC.IsReadOnly = !on;
            CboModePaiement.IsEnabled = on;
            TxtDatePaiement.IsReadOnly = !on;
            CboStatut.IsEnabled = on;
            TxtNote.IsReadOnly = !on;
        }

        private async void BtnActualiser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TxtRecherche.Text = string.Empty;

                if (GridClients.SelectedItem is FactureClientDTO fc)
                    _idClientSelectionne = fc.Id;

                if (GridFourn.SelectedItem is FactureFournisseurDTO ff)
                    _idFournisseurSelectionne = ff.Id;

                await ChargerAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] BtnActualiser_Click → {ex.Message}");
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNouveau_Click(object sender, RoutedEventArgs e)
        {
            _nouveau = true;

            if (TabFactures.SelectedIndex == 0)
            {
                if (!_clients.Any())
                {
                    MessageBox.Show("Aucun client disponible.", "Information",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    _nouveau = false;
                    return;
                }

                LblTiers.Text = "Client";
                PrepCboTiersClient();
                CboTiers.SelectedIndex = 0;
                TxtNumero.Text = $"FAC-{DateTime.Now.Year}-{_facturesClients.Count + 1:000}";
            }
            else
            {
                if (!_nomsFournisseurs.Any())
                {
                    MessageBox.Show("Aucun fournisseur disponible.", "Information",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    _nouveau = false;
                    return;
                }

                LblTiers.Text = "Fournisseur";
                PrepCboTiersFournisseur();
                CboTiers.SelectedIndex = 0;
                TxtNumero.Text = $"FAF-{DateTime.Now.Year}-{_facturesFournisseurs.Count + 1:000}";
            }

            TxtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TxtEcheance.Text = DateTime.Now.AddDays(30).ToString("dd/MM/yyyy");
            TxtDatePaiement.Text = string.Empty;
            TxtMontantHT.Text = "0,00";
            TxtTVA.Text = "0,00";
            TxtMontantTTC.Text = "0,00";
            TxtNote.Text = string.Empty;

            SetCombo(CboStatut, "EnAttente");
            SetCombo(CboModePaiement, "Virement");

            ModeEdition(true);
            MajBoutons();
        }

        private async void BtnSauvegarder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TabFactures.SelectedIndex == 0)
                {
                    if (CboTiers.SelectedItem is not ClientDTO client)
                    {
                        MessageBox.Show("Sélectionnez un client.", "Obligatoire",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(TxtNumero.Text))
                    {
                        MessageBox.Show("Le numéro est obligatoire.", "Obligatoire",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var f = new FactureClientDTO
                    {
                        ClientId = client.Id,
                        Client = client.Nom,
                        Numero = TxtNumero.Text.Trim(),
                        Date = TxtDate.Text.Trim(),
                        DateEcheance = TxtEcheance.Text.Trim(),
                        DatePaiement = TxtDatePaiement.Text.Trim(),
                        MontantHT = TxtMontantHT.Text.Trim(),
                        TVA = TxtTVA.Text.Trim(),
                        MontantTTC = TxtMontantTTC.Text.Trim(),
                        Statut = GetCombo(CboStatut),
                        ModePaiement = GetCombo(CboModePaiement),
                        Commentaire = TxtNote.Text.Trim()
                    };

                    await _service.AjouterFacture(f);
                    _facturesClients = await _service.GetToutesLesFactures();
                    _idClientSelectionne = f.Id;
                }
                else
                {
                    if (CboTiers.SelectedItem is not string nomFourn ||
                        string.IsNullOrWhiteSpace(nomFourn))
                    {
                        MessageBox.Show("Sélectionnez un fournisseur.", "Obligatoire",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(TxtNumero.Text))
                    {
                        MessageBox.Show("Le numéro est obligatoire.", "Obligatoire",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var f = new FactureFournisseurDTO
                    {
                        Fournisseur = nomFourn,
                        Numero = TxtNumero.Text.Trim(),
                        Date = TxtDate.Text.Trim(),
                        DateEcheance = TxtEcheance.Text.Trim(),
                        DatePaiement = TxtDatePaiement.Text.Trim(),
                        MontantHT = TxtMontantHT.Text.Trim(),
                        TVA = TxtTVA.Text.Trim(),
                        MontantTTC = TxtMontantTTC.Text.Trim(),
                        Statut = GetCombo(CboStatut),
                        ModePaiement = GetCombo(CboModePaiement),
                        Commentaire = TxtNote.Text.Trim()
                    };

                    await _service.AjouterFactureFournisseur(f);
                    _facturesFournisseurs = await _service.GetToutesLesFacturesFournisseurs();
                    _nomsFournisseurs = _facturesFournisseurs
                        .Select(x => x.Fournisseur ?? string.Empty)
                        .Where(n => !string.IsNullOrWhiteSpace(n))
                        .Distinct()
                        .OrderBy(n => n)
                        .ToList();
                    _idFournisseurSelectionne = f.Id;
                }

                _nouveau = false;
                ModeEdition(false);

                if (TabFactures.SelectedIndex <= 0)
                {
                    GridClients.ItemsSource = new ObservableCollection<FactureClientDTO>(_facturesClients);
                    RestaurerSelectionClient();
                }
                else
                {
                    GridFourn.ItemsSource = new ObservableCollection<FactureFournisseurDTO>(_facturesFournisseurs);
                    RestaurerSelectionFournisseur();
                }

                MajBoutons();

                MessageBox.Show("Enregistrement effectué.", "Succès",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] BtnSauvegarder_Click → {ex.Message}");
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            _nouveau = false;
            ModeEdition(false);

            if (TabFactures.SelectedIndex <= 0)
                RestaurerSelectionClient();
            else
                RestaurerSelectionFournisseur();

            MajBoutons();
        }

        private void TxtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_busy || _nouveau)
                return;

            string t = TxtRecherche.Text.Trim().ToLower();
            _busy = true;

            if (TabFactures.SelectedIndex == 0)
            {
                var r = string.IsNullOrWhiteSpace(t)
                    ? _facturesClients
                    : _facturesClients.Where(f =>
                        Ok(f.Numero, t) || Ok(f.Client, t) ||
                        Ok(f.Date, t) || Ok(f.DateEcheance, t) ||
                        Ok(f.Statut, t) || Ok(f.MontantTTC, t)).ToList();

                GridClients.ItemsSource = new ObservableCollection<FactureClientDTO>(r);
                var cible = r.FirstOrDefault(f => f.Id == _idClientSelectionne) ?? r.FirstOrDefault();

                if (cible != null)
                {
                    GridClients.SelectedItem = cible;
                    AfficherClient(cible);
                }
                else Vider();
            }
            else
            {
                var r = string.IsNullOrWhiteSpace(t)
                    ? _facturesFournisseurs
                    : _facturesFournisseurs.Where(f =>
                        Ok(f.Numero, t) || Ok(f.Fournisseur, t) ||
                        Ok(f.Date, t) || Ok(f.DateEcheance, t) ||
                        Ok(f.Statut, t) || Ok(f.MontantTTC, t)).ToList();

                GridFourn.ItemsSource = new ObservableCollection<FactureFournisseurDTO>(r);
                var cible = r.FirstOrDefault(f => f.Id == _idFournisseurSelectionne) ?? r.FirstOrDefault();

                if (cible != null)
                {
                    GridFourn.SelectedItem = cible;
                    AfficherFourn(cible);
                }
                else Vider();
            }

            MajBtnMarquerPaye();
            _busy = false;
        }

        private void TabFactures_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!IsLoaded || _busy || _nouveau)
                return;

            if (e.OriginalSource is not TabControl)
                return;

            _busy = true;
            TxtRecherche.Text = string.Empty;

            if (TabFactures.SelectedIndex == 0)
            {
                GridClients.ItemsSource = new ObservableCollection<FactureClientDTO>(_facturesClients);
                RestaurerSelectionClient();
            }
            else
            {
                GridFourn.ItemsSource = new ObservableCollection<FactureFournisseurDTO>(_facturesFournisseurs);
                RestaurerSelectionFournisseur();
            }

            MajBoutons();
            _busy = false;
        }

        private void GridClients_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (_busy || _nouveau)
                return;

            if (GridClients.SelectedItem is FactureClientDTO f)
            {
                _idClientSelectionne = f.Id;
                AfficherClient(f);
            }

            MajBtnMarquerPaye();
        }

        private void GridFourn_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (_busy || _nouveau)
                return;

            if (GridFourn.SelectedItem is FactureFournisseurDTO f)
            {
                _idFournisseurSelectionne = f.Id;
                AfficherFourn(f);
            }

            MajBtnMarquerPaye();
        }

        private void AfficherClient(FactureClientDTO f)
        {
            _busy = true;

            LblTiers.Text = "Client";
            PrepCboTiersClient();

            CboTiers.SelectedIndex = -1;
            CboTiers.SelectedValue = f.ClientId;

            TxtNumero.Text = f.Numero ?? string.Empty;
            TxtDate.Text = f.Date ?? string.Empty;
            TxtEcheance.Text = f.DateEcheance ?? string.Empty;
            TxtMontantHT.Text = f.MontantHT ?? string.Empty;
            TxtTVA.Text = f.TVA ?? string.Empty;
            TxtMontantTTC.Text = f.MontantTTC ?? string.Empty;
            TxtDatePaiement.Text = f.DatePaiement ?? string.Empty;
            TxtNote.Text = f.Commentaire ?? string.Empty;

            SetCombo(CboModePaiement, f.ModePaiement);
            SetCombo(CboStatut, f.Statut);

            _busy = false;
        }

        private void AfficherFourn(FactureFournisseurDTO f)
        {
            _busy = true;

            LblTiers.Text = "Fournisseur";
            PrepCboTiersFournisseur();
            CboTiers.SelectedItem = _nomsFournisseurs.FirstOrDefault(n => n == f.Fournisseur);

            TxtNumero.Text = f.Numero ?? string.Empty;
            TxtDate.Text = f.Date ?? string.Empty;
            TxtEcheance.Text = f.DateEcheance ?? string.Empty;
            TxtMontantHT.Text = f.MontantHT ?? string.Empty;
            TxtTVA.Text = f.TVA ?? string.Empty;
            TxtMontantTTC.Text = f.MontantTTC ?? string.Empty;
            TxtDatePaiement.Text = f.DatePaiement ?? string.Empty;
            TxtNote.Text = f.Commentaire ?? string.Empty;

            SetCombo(CboModePaiement, f.ModePaiement);
            SetCombo(CboStatut, f.Statut);

            _busy = false;
        }

        private void PrepCboTiersClient()
        {
            CboTiers.DisplayMemberPath = "Nom";
            CboTiers.SelectedValuePath = "Id";

            if (CboTiers.ItemsSource != _clients)
                CboTiers.ItemsSource = _clients;
        }

        private void PrepCboTiersFournisseur()
        {
            CboTiers.DisplayMemberPath = string.Empty;
            CboTiers.SelectedValuePath = string.Empty;

            if (CboTiers.ItemsSource != _nomsFournisseurs)
                CboTiers.ItemsSource = _nomsFournisseurs;
        }

        private void Vider()
        {
            if (TabFactures.SelectedIndex <= 0)
                PrepCboTiersClient();
            else
                PrepCboTiersFournisseur();

            CboTiers.SelectedIndex = -1;
            TxtNumero.Text = string.Empty;
            TxtDate.Text = string.Empty;
            TxtEcheance.Text = string.Empty;
            TxtMontantHT.Text = string.Empty;
            TxtTVA.Text = string.Empty;
            TxtMontantTTC.Text = string.Empty;
            TxtDatePaiement.Text = string.Empty;
            TxtNote.Text = string.Empty;
            CboModePaiement.SelectedIndex = -1;
            CboStatut.SelectedIndex = -1;
        }

        private static bool Ok(string? s, string t) =>
            (s ?? string.Empty).ToLower().Contains(t);

        private static string GetCombo(ComboBox c) =>
            c.SelectedItem is ComboBoxItem i
                ? i.Content?.ToString() ?? string.Empty
                : string.Empty;

        private static void SetCombo(ComboBox c, string val)
        {
            c.SelectedIndex = -1;

            foreach (var item in c.Items)
            {
                if (item is ComboBoxItem ci && ci.Content?.ToString() == val)
                {
                    c.SelectedItem = ci;
                    return;
                }
            }
        }
    }
}