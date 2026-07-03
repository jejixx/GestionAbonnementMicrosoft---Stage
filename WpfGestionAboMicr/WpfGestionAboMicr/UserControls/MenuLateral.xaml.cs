using Syncfusion.UI.Xaml.Diagram;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace WpfGestionAboMicr.UserControls
{
    public partial class MenuLateral : UserControl
    {
        public MenuLateral()
        {
            InitializeComponent();
        }

        #region binding
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(
                nameof(Label),
                typeof(string),
                typeof(MenuLateral),
                new PropertyMetadata(string.Empty));
        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }
        #endregion


        public static readonly DependencyProperty IconeSourceProperty =
            DependencyProperty.Register(
                nameof(IconeSource),
                typeof(string),           // ← string au lieu de ImageSource
                typeof(MenuLateral),
                new PropertyMetadata(null));

        public static readonly DependencyProperty EstActifProperty =
                               DependencyProperty.Register(nameof(EstActif),
                                                           typeof(bool),
                                                           typeof(MenuLateral),
                                                           new PropertyMetadata(false));
                                                    


        public bool EstActif
        {
            get => (bool)GetValue(EstActifProperty);
            set => SetValue(EstActifProperty, value);
        }
        public string IconeSource
        {
            get => (string)GetValue(IconeSourceProperty);
            set => SetValue(IconeSourceProperty, value);
        }


        // ✅ Correct
        public event EventHandler<MouseButtonEventArgs> BorderClicked;

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BorderClicked.Invoke(this, e);
        }
    }
}
