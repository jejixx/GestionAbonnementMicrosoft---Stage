using GestionAboMicroService.Services;
using System.Configuration;
using System.Data;
using System.Windows;

namespace WpfGestionAboMicr
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // ✅ Instance unique partagée par toute l'application
        public static AppService AppService { get; } = new AppService();
    }
}
