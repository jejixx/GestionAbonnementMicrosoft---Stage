using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfGestionAboMicr.UserControls
{
    public partial class MenuTemplate : UserControl
    {
        public event EventHandler<string>? NavigationRequested;

        public MenuTemplate()
        {
            InitializeComponent();
        }


        public void AjouterItem(MenuLateral item)
        {
            item.Height = 40;
            item.BorderClicked += Item_BorderClicked;
            DockPanel.SetDock(item, Dock.Top);
            ItemsHost.Children.Add(item);
        }

        // Ajoute un espace vide qui pousse ce qui suit vers le bas
        public void AjouterSeparateur()
        {
            var spacer = new Border { Background = null };
            DockPanel.SetDock(spacer, Dock.Top);
            spacer.Height = 20; // espace visuel entre les groupes
            ItemsHost.Children.Add(spacer);
        }

        private void Item_BorderClicked(object? sender, EventArgs e)
        {
            var item = sender as MenuLateral;
            string? tag = item?.Tag as string;

            if (tag == "Deconnexion")
            {
                var result = MessageBox.Show(
                    "Voulez-vous vraiment vous déconnecter ?",
                    "Déconnexion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    Application.Current.Shutdown();
                return;
            }

            NavigationRequested?.Invoke(this, tag ?? string.Empty);
        }

        #region Binding
        public static readonly DependencyProperty HeaderMenuProperty =
                DependencyProperty.Register(
                nameof(HeaderMenu),
                typeof(string),
                typeof(MenuTemplate),
                new PropertyMetadata(string.Empty));

        public string HeaderMenu
        {
            get => (string)GetValue(HeaderMenuProperty);
            set => SetValue(HeaderMenuProperty, value);
        }
        public static readonly DependencyProperty VersionProperty =
                DependencyProperty.Register(
                nameof(Version),
                typeof(string),
                typeof(MenuTemplate),
                new PropertyMetadata(string.Empty));

        public string Version
        {
            get => (string)GetValue(VersionProperty);
            set => SetValue(VersionProperty, value);
        }

        public static readonly DependencyProperty AppNameProperty =
                DependencyProperty.Register(
                nameof(AppName),
                typeof(string),
                typeof(MenuTemplate),
                new PropertyMetadata(string.Empty));

        public string AppName
        {
            get => (string)GetValue(AppNameProperty);
            set => SetValue(AppNameProperty, value);
        }

        #endregion


        // Item collé en BAS du menu
        public void AjouterItemBas(MenuLateral item)
        {
            item.Height = 40;
            item.BorderClicked += Item_BorderClicked;
            DockPanel.SetDock(item, Dock.Bottom);
            ItemsHost.Children.Add(item);
        }
    }

}