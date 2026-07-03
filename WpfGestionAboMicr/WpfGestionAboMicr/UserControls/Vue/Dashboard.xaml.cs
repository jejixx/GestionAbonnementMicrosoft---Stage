using GestionAboMicroService.DTO;
using GestionAboMicroService.Services;
using Syncfusion.SfSkinManager;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfGestionAboMicr.UserControls.Vue
{
    public class TypeValeurToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is TypeValeur type ? type switch
            {
                TypeValeur.Positif => Brushes.Green,
                TypeValeur.Negatif => Brushes.Red,
                TypeValeur.Info => Brushes.Blue,
                TypeValeur.Neutre => Brushes.Gray,
                _ => Brushes.Black
            } : Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class StatutToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string statut ? statut switch
            {
                "Actif" => Brushes.Green,
                "EnRetard" => Brushes.Red,
                "Bientot" => Brushes.Orange,
                _ => Brushes.Gray
            } : Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public partial class Dashboard : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void Notify(string prop) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public ObservableCollection<AbonnementDTO> DataEvolution { get; } = new();
        public ObservableCollection<EvolutionDTO> EvolutionModel { get; } = new();
        public ObservableCollection<RenouvellementDTO> ListeRenouvellements { get; } = new();
        public ObservableCollection<LigneInfoDTO> ListeResume { get; } = new();

        private int _nombreClients;
        public int NombreClients
        {
            get => _nombreClients;
            set { _nombreClients = value; Notify(nameof(NombreClients)); }
        }

        private int _nombreAbonnements;
        public int NombreAbonnements
        {
            get => _nombreAbonnements;
            set { _nombreAbonnements = value; Notify(nameof(NombreAbonnements)); }
        }

        private int _nombreRenouvellements;
        public int NombreRenouvellements
        {
            get => _nombreRenouvellements;
            set { _nombreRenouvellements = value; Notify(nameof(NombreRenouvellements)); }
        }

        private int _nombreFacturesEnAttente;
        public int NombreFacturesEnAttente
        {
            get => _nombreFacturesEnAttente;
            set { _nombreFacturesEnAttente = value; Notify(nameof(NombreFacturesEnAttente)); }
        }

        public Dashboard()
        {
            InitializeComponent();
            SfSkinManager.SetTheme(this, new Theme("FluentLight"));
            DataContext = this;

            Loaded += Dashboard_Loaded;
        }

        private async void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Dashboard_Loaded;
            await RafraichirAsync();
        }

        public async Task RafraichirAsync()
        {
            try
            {
                var service = App.AppService;

                var dashboardTask = service.GetDonneesDashboard();
                var renouvellementsTask = service.GetRenouvellements();
                var resumeTask = service.GetLignesResume();
                var totalClientsTask = service.GetTotalClients();
                var totalAbonnementsTask = service.GetTotalAbonnements();
                var totalRenouvellementsTask = service.GetTotalRenouvellementsCritiques();
                var facturesEnAttenteTask = service.GetNombreFacturesEnAttente();

                await Task.WhenAll(
                    dashboardTask,
                    renouvellementsTask,
                    resumeTask,
                    totalClientsTask,
                    totalAbonnementsTask,
                    totalRenouvellementsTask,
                    facturesEnAttenteTask);

                var (abonnements, evolution) = await dashboardTask;

                Recharger(DataEvolution, abonnements);
                Recharger(EvolutionModel, evolution);
                Recharger(ListeRenouvellements, await renouvellementsTask);
                Recharger(ListeResume, await resumeTask);

                NombreClients = await totalClientsTask;
                NombreAbonnements = await totalAbonnementsTask;
                NombreRenouvellements = await totalRenouvellementsTask;
                NombreFacturesEnAttente = await facturesEnAttenteTask;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 [UI] Dashboard RafraichirAsync → {ex.Message}");
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void Recharger<T>(ObservableCollection<T> collection, System.Collections.Generic.List<T> nouvelles)
        {
            collection.Clear();

            foreach (var item in nouvelles)
                collection.Add(item);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            double fontSize = Math.Max(15, ActualWidth / 80);
            Resources["DynamicFontSize"] = fontSize;
        }
    }
}