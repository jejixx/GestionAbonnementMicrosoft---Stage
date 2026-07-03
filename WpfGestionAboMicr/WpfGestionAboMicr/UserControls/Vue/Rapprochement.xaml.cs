using GestionAboMicroService.DTO;
using GestionAboMicroService.Services;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfGestionAboMicr.UserControls.Vue
{
    public partial class Rapprochement : UserControl
    {
        private readonly AppService _service = App.AppService;

        private List<RapprochementDTO> _listeComplete = new();
        private List<RapprochementDTO> _listeAffichee = new();

        private bool _busy = false;
        private int _idSelectionne = -1;

        public Rapprochement()
        {
            InitializeComponent();
            Loaded += Rapprochement_Loaded;
        }

        private async void Rapprochement_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Rapprochement_Loaded;
            await ChargerAsync();
        }

        private async Task ChargerAsync()
        {
            try
            {
                _busy = true;

                _listeComplete = await _service.GetRapprochements();
                _listeAffichee = _listeComplete.ToList();

                GridRapprochements.ItemsSource =
                    new ObservableCollection<RapprochementDTO>(_listeAffichee);

                RestaurerSelection();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] Rapprochement ChargerAsync → {ex.Message}");
                MessageBox.Show(ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _busy = false;
            }
        }

        public Task RafraichirAsync() => ChargerAsync();

        private void RestaurerSelection()
        {
            var cible = _idSelectionne > 0
                ? _listeAffichee.FirstOrDefault(x => x.Id == _idSelectionne)
                : null;

            cible ??= _listeAffichee.FirstOrDefault();

            if (cible != null)
            {
                GridRapprochements.SelectedItem = cible;
                Afficher(cible);
            }
            else
            {
                Vider();
            }
        }

        private void GridRapprochements_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (_busy)
                return;

            if (GridRapprochements.SelectedItem is RapprochementDTO item)
            {
                _idSelectionne = item.Id;
                Afficher(item);
            }
        }

        private void Afficher(RapprochementDTO item)
        {
            TxtClient.Text = item.Client ?? string.Empty;
            TxtPeriode.Text = item.Periode ?? string.Empty;
            TxtEcart.Text = item.Ecart ?? string.Empty;
            TxtFactureFournisseur.Text = item.FactureFournisseur ?? string.Empty;
            TxtMontantFournisseur.Text = item.MontantFournisseur ?? string.Empty;
            TxtFactureClient.Text = item.FactureClient ?? string.Empty;
            TxtEcartPourcent.Text = item.EcartPourcent ?? string.Empty;
        }

        private void Vider()
        {
            TxtClient.Text = string.Empty;
            TxtPeriode.Text = string.Empty;
            TxtEcart.Text = string.Empty;
            TxtFactureFournisseur.Text = string.Empty;
            TxtMontantFournisseur.Text = string.Empty;
            TxtFactureClient.Text = string.Empty;
            TxtEcartPourcent.Text = string.Empty;
        }

        private void TxtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_busy)
                return;

            string texte = TxtRecherche.Text.Trim().ToLower();

            _listeAffichee = string.IsNullOrWhiteSpace(texte)
                ? _listeComplete.ToList()
                : _listeComplete.Where(x =>
                    (x.Client ?? string.Empty).ToLower().Contains(texte) ||
                    (x.Date ?? string.Empty).ToLower().Contains(texte) ||
                    (x.FactureClient ?? string.Empty).ToLower().Contains(texte) ||
                    (x.FactureFournisseur ?? string.Empty).ToLower().Contains(texte) ||
                    (x.Statut ?? string.Empty).ToLower().Contains(texte))
                .ToList();

            GridRapprochements.ItemsSource =
                new ObservableCollection<RapprochementDTO>(_listeAffichee);

            RestaurerSelection();
        }

        private async void BtnActualiser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TxtRecherche.Text = string.Empty;
                await ChargerAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] BtnActualiser_Click → {ex.Message}");
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExporter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new SaveFileDialog
                {
                    Title = "Exporter les rapprochements",
                    Filter = "Fichier CSV (*.csv)|*.csv",
                    DefaultExt = "csv",
                    AddExtension = true,
                    FileName = $"Rapprochements_{DateTime.Now:yyyyMMdd_HHmm}"
                };

                if (dlg.ShowDialog() != true)
                    return;

                var sb = new StringBuilder();
                sb.AppendLine("Date;Client;Période;Facture fournisseur;Montant fournisseur;Facture client;Montant client;Écart;Écart %;Statut;Commentaire");

                foreach (var x in _listeAffichee)
                {
                    sb.AppendLine(
                        $"{Esc(x.Date)};" +
                        $"{Esc(x.Client)};" +
                        $"{Esc(x.Periode)};" +
                        $"{Esc(x.FactureFournisseur)};" +
                        $"{Esc(x.MontantFournisseur)};" +
                        $"{Esc(x.FactureClient)};" +
                        $"{Esc(x.MontantClient)};" +
                        $"{Esc(x.Ecart)};" +
                        $"{Esc(x.EcartPourcent)};" +
                        $"{Esc(x.Statut)};" +
                        $"{Esc(x.Commentaire)}");
                }

                File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);

                MessageBox.Show("Exportation réalisée avec succès.",
                    "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'exportation : " + ex.Message,
                    "Export", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string Esc(string? texte)
            => (texte ?? string.Empty)
                .Replace(";", ",")
                .Replace("\r", " ")
                .Replace("\n", " ");
    }
}