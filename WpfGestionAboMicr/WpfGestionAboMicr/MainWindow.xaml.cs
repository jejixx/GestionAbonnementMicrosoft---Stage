using GestionAboMicroService.DTO;
using System.Windows;
using System.Windows.Controls;
using WpfGestionAboMicr.UserControls;
using WpfGestionAboMicr.UserControls.Vue;

namespace WpfGestionAboMicr
{
    public partial class MainWindow : Window
    {
        // ─── Vues ─────────────────────────────────────────────────────
        private readonly Dashboard _dashboard = new Dashboard();
        private readonly Clients _clients = new Clients();
        private readonly ClientAbonnements _clientAbo = new ClientAbonnements();
        private readonly Facturation _facturation = new Facturation();
        private readonly Rapprochement _rapprochement = new Rapprochement();
        private readonly Renouvellement _renouvellement = new Renouvellement();
        private readonly Admin _admin = new Admin();

        // ─── Items menu ───────────────────────────────────────────────
        private MenuLateral? _itemActif;
        private MenuLateral _itemDashboard = new MenuLateral { IconeSource = "/Resources/tableauDeBord.png", Label = "Tableau de bord", Tag = "Dashboard" };
        private MenuLateral _itemClients = new MenuLateral { IconeSource = "/Resources/clients.png", Label = "Clients", Tag = "Clients" };
        private MenuLateral _itemClientAbo = new MenuLateral { IconeSource = "/Resources/clientAbonnements.png", Label = "Client Abonnements", Tag = "ClientAbo" };
        private MenuLateral _itemFacturation = new MenuLateral { IconeSource = "/Resources/facturation.png", Label = "Facturation", Tag = "Facturation" };
        private MenuLateral _itemRapprochement = new MenuLateral { IconeSource = "/Resources/rapprochement.png", Label = "Rapprochement", Tag = "Rapprochement" };
        private MenuLateral _itemRenouvellement = new MenuLateral { IconeSource = "/Resources/renouvellement.png", Label = "Renouvellement", Tag = "Renouvellement" };

        public MainWindow()
        {
            InitializeComponent();

            MonMenu.AjouterItemBas(new MenuLateral { IconeSource = "/Resources/deconnexion.png", Label = "Déconnexion", Tag = "Deconnexion" });
            MonMenu.AjouterItemBas(new MenuLateral { IconeSource = "/Resources/admin.png", Label = "Admin", Tag = "Admin" });

            MonMenu.AjouterItem(_itemDashboard);
            MonMenu.AjouterItem(_itemClients);
            MonMenu.AjouterItem(_itemClientAbo);
            MonMenu.AjouterItem(_itemFacturation);
            MonMenu.AjouterItem(_itemRapprochement);
            MonMenu.AjouterItem(_itemRenouvellement);

            DataContext = this;

            _clients.DetailClientRequested += OnDetailClientRequested;
            _clients.NouveauClientRequested += OnNouveauClientRequested;

            Loaded += MainWindow_Loaded;
        }

        // ══════════════════════════════════════════════════════════════
        //  NAVIGATION
        // ══════════════════════════════════════════════════════════════

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            NaviguerVers(_dashboard);
            SetItemActif(_itemDashboard);
            await _dashboard.RafraichirAsync();
        }

        private async void MonMenu_NavigationRequested(object sender, string tag)
        {
            switch (tag)
            {
                case "Dashboard":
                    NaviguerVers(_dashboard);
                    SetItemActif(_itemDashboard);
                    await _dashboard.RafraichirAsync();
                    break;

                case "Clients":
                    NaviguerVers(_clients);
                    SetItemActif(_itemClients);
                    await _clients.RafraichirAsync();
                    break;

                case "ClientAbo":
                    NaviguerVers(_clientAbo);
                    SetItemActif(_itemClientAbo);
                    break;

                case "Facturation":
                    NaviguerVers(_facturation);
                    SetItemActif(_itemFacturation);
                    break;

                case "Rapprochement":
                    NaviguerVers(_rapprochement);
                    SetItemActif(_itemRapprochement);
                    break;

                case "Renouvellement":
                    NaviguerVers(_renouvellement);
                    SetItemActif(_itemRenouvellement);
                    break;

                case "Admin":
                    NaviguerVers(_admin);
                    SetItemActif(null);
                    break;

                default:
                    MessageBox.Show("Page introuvable");
                    break;
            }
        }

        private void OnDetailClientRequested(ClientDTO client)
        {
            var vue = new DetailClient(client);

            vue.DemandeRetour += async (s, e) =>
            {
                await _clients.RafraichirAsync();
                await _dashboard.RafraichirAsync();
                NaviguerVers(_clients);
                SetItemActif(_itemClients);
            };

            NaviguerVers(vue);
            SetItemActif(_itemClients);
        }

        private void OnNouveauClientRequested()
        {
            var vue = new DetailClient();

            vue.DemandeRetour += async (s, e) =>
            {
                await _clients.RafraichirAsync();
                await _dashboard.RafraichirAsync();
                NaviguerVers(_clients);
                SetItemActif(_itemClients);
            };

            NaviguerVers(vue);
            SetItemActif(_itemClients);
        }

        private void NaviguerVers(UserControl vue)
        {
            conteneur.Children.Clear();
            conteneur.Children.Add(vue);
        }

        private void SetItemActif(MenuLateral? item)
        {
            if (_itemActif != null)
                _itemActif.EstActif = false;

            _itemActif = item;

            if (_itemActif != null)
                _itemActif.EstActif = true;
        }
    }
}