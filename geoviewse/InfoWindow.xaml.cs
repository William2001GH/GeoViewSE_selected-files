using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GeoViewSE_Linnaeus
{
    /// <summary>
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        private double _lat;
        private double _lon;
        public event Action<string>? CopilotRequested;
        public event Action PlaySoundRequested;


        public InfoWindow(string name, string category, string description, string picturePath, string iconPath, string coord, double lat, double lon, string environmentalData)
        {
            InitializeComponent();

            TitleText.Text = name;
            CategoryText.Text = $"Kategori: {category}";
            DescriptionText.Text = description;
            CoordText.Text = coord;
            EnvironmentalText.Text = environmentalData;
            CoordText.Visibility = string.IsNullOrEmpty(coord) ? Visibility.Collapsed : Visibility.Visible;
            _lat = lat;
            _lon = lon;


            if (!string.IsNullOrEmpty(picturePath) && File.Exists(picturePath))
            {
                PlaceImage.Source = new BitmapImage(new Uri(picturePath, UriKind.RelativeOrAbsolute));
            }
            else
            {
                PlaceImage.Visibility = Visibility.Collapsed;
            }

            if (!string.IsNullOrEmpty(iconPath) && File.Exists(iconPath)) 
            {
                CategoryIcon.Source = new BitmapImage(new Uri(iconPath, UriKind.RelativeOrAbsolute)); 
            }
            else
            { // Fallback baserat på kategori
              string fallback = category switch 
              {
                  "Slott" => "data/ikoner/castle.png",
                  "Botaniska trädgårdar" => "data/ikoner/castle.png",
                  "Kyrkor" => "data/ikoner/church.png",
                  "Runsten" => "data/ikoner/runa.png",
                  "Strand" => "data/ikoner/beach.png",
                  "Fossil" => "data/ikoner/ammonite.png",
                  "Staty" => "data/ikoner/statue.png",
                  _ => "" 
              };
                if (!string.IsNullOrEmpty(fallback) && File.Exists(fallback))
                {
                    CategoryIcon.Source = new BitmapImage(new Uri(fallback, UriKind.RelativeOrAbsolute));
                }
                else CategoryIcon.Visibility = Visibility.Collapsed; 
            }
        }
        private void OpenInGoogleMaps_Click(object sender, RoutedEventArgs e)
        {
            var url = $"https://www.google.com/maps?q={_lat.ToString(System.Globalization.CultureInfo.InvariantCulture)},{_lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
       }
        private void ContactDeveloper_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://mail.google.com/mail/?view=cm&fs=1&to=knutssongustafssonwilliam@gmail.com&su=GeoView%20Feedback";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
        }
        private void AskCopilot_Click(object sender, RoutedEventArgs e)
        {
            CopilotRequested?.Invoke(EnvironmentalText.Text);
        }
        private void PlaySound_Click(object sender, RoutedEventArgs e)
        {
            PlaySoundRequested?.Invoke(); 
        }
        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            // Bygg ihop all text som ska kopieras
            var sb = new StringBuilder();

            sb.AppendLine(TitleText.Text);
            sb.AppendLine(CategoryText.Text);
            sb.AppendLine(DescriptionText.Text);

            if (CoordText.Visibility == Visibility.Visible)
                sb.AppendLine(CoordText.Text);

            sb.AppendLine();
            sb.AppendLine("— Miljödata —");
            sb.AppendLine(EnvironmentalText.Text);

            // Kopiera till urklipp
            Clipboard.SetText(sb.ToString());
        }

    }

}
