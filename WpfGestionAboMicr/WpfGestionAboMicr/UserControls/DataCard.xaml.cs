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

namespace WpfGestionAboMicr.UserControls
{
    public partial class DataCard : UserControl
    {
        public DataCard() => InitializeComponent();

        // Indicateur texte (ex: "▲ ce mois")
        public static readonly DependencyProperty IndicateurProperty =
            DependencyProperty.Register("Indicateur", typeof(string), typeof(DataCard), new PropertyMetadata(""));
        public string Indicateur
        {
            get => (string)GetValue(IndicateurProperty);
            set => SetValue(IndicateurProperty, value);
        }

        // Couleur de l'indicateur
        public static readonly DependencyProperty CouleurIndicateurProperty =
            DependencyProperty.Register("CouleurIndicateur", typeof(SolidColorBrush), typeof(DataCard),
                new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        public SolidColorBrush CouleurIndicateur
        {
            get => (SolidColorBrush)GetValue(CouleurIndicateurProperty);
            set => SetValue(CouleurIndicateurProperty, value);
        }

        // Source de l'icône
        public static readonly DependencyProperty IconeSourceProperty =
            DependencyProperty.Register("IconeSource", typeof(ImageSource), typeof(DataCard), new PropertyMetadata(null));
        public ImageSource IconeSource
        {
            get => (ImageSource)GetValue(IconeSourceProperty);
            set => SetValue(IconeSourceProperty, value);
        }

        // Couleur de l'icône
        public static readonly DependencyProperty CouleurIconeProperty =
            DependencyProperty.Register("CouleurIcone", typeof(Color), typeof(DataCard),
                new PropertyMetadata(Colors.Blue));
        public Color CouleurIcone
        {
            get => (Color)GetValue(CouleurIconeProperty);
            set => SetValue(CouleurIconeProperty, value);
        }

        // Valeur affichée (nombre)
        public static readonly DependencyProperty ValeurProperty =
            DependencyProperty.Register("Valeur", typeof(string), typeof(DataCard), new PropertyMetadata("0"));
        public string Valeur
        {
            get => (string)GetValue(ValeurProperty);
            set => SetValue(ValeurProperty, value);
        }

        // Label bas de carte
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(DataCard), new PropertyMetadata(""));
        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        // Épaisseur de bordure
        public static readonly DependencyProperty BordureEpaisseurProperty =
            DependencyProperty.Register("BordureEpaisseur", typeof(Thickness), typeof(DataCard),
                new PropertyMetadata(new Thickness(1, 1, 1, 1)));
        public Thickness BordureEpaisseur
        {
            get => (Thickness)GetValue(BordureEpaisseurProperty);
            set => SetValue(BordureEpaisseurProperty, value);
        }
    }
}
