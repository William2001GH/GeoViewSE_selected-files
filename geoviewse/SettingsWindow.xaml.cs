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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }
        private void ChkBattles_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.battleLayer.Enabled = true;
        }

        private void ChkBattles_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.battleLayer.Enabled = false;
        }

        private void ChkCaves_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.caveLayer.Enabled = true;
        }

        private void ChkCaves_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.caveLayer.Enabled = false;
        }
        private void ChkHist_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
            {
                mw._historicalLayer.MapLayer.Enabled = true;
                mw.TimeSlider.Visibility = Visibility.Visible;
                mw.TimeShuttle.Visibility = Visibility.Visible;
            }
        }

        private void ChkHist_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
            {
                mw._historicalLayer.MapLayer.Enabled = false;
                mw.TimeSlider.Visibility = Visibility.Hidden;
                mw.TimeShuttle.Visibility = Visibility.Hidden;
            }
        }
        private void ChkTC_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.townsLayer.Enabled = true;
        }

        private void ChkTC_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.townsLayer.Enabled = false;
        }
        private void ChkIceA_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.IceALayer.Enabled = true;
        }

        private void ChkIceA_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.IceALayer.Enabled = false;
        }

        private void ChkIceB_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.IceBLayer.Enabled = true;
        }

        private void ChkIceB_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.IceBLayer.Enabled = false;
        }

        private void ChkIceC_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.IceCLayer.Enabled = true;
        }

        private void ChkIceC_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.IceCLayer.Enabled = false;
        }

        private void ChkIceD_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.IceDLayer.Enabled = true;
        }

        private void ChkIceD_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.IceDLayer.Enabled = false;
        }
        private void ChkPbdb_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.pdbdLayer.Enabled = true;
        }
        private void ChkPbdb_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.pdbdLayer.Enabled = false;
        }
        private void ClientPhotoLayer_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.clientPhotoLayer.Enabled = true;
        }
        private void ClientPhotoLayer_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.clientPhotoLayer.Enabled = false;
        }
        private void ChkUas_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.uasLayer.Enabled = true;
        }

        private void ChkUas_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.uasLayer.Enabled = false;
        }
        private void ChkChurchFull_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.churchFullLayer.Enabled = true;
        }

        private void ChkChurchFull_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.churchFullLayer.Enabled = false;
        }
        private void ChkHarbours_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.harbourLayer.Enabled = true;
        }

        private void ChkHarbours_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.harbourLayer.Enabled = false;
        }
        private void ChkMarine_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.worldMarineHeritageSitesLayer.Enabled = true;
        }

        private void ChkMarine_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.worldMarineHeritageSitesLayer.Enabled = false;
        }

        private void ChkVattendistrikt_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.GetLayerByName("Vattendistrikt").Enabled = true;
        }

        private void ChkVattendistrikt_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.GetLayerByName("Vattendistrikt").Enabled = false;
        }
        private void ChkDamLayer_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.damLayer.Enabled = true;
        }
        private void ChkDamLayer_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.damLayer.Enabled = false;
        }
        private void ChkOsparLayer_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.osparLayer.Enabled = true;
        }
        private void ChkOsparLayer_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.osparLayer.Enabled = false;
        }
        private void ChkTrawling_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
            {
                mw.trawlingLayer.Enabled = true;
            }
        }
        private void ChkTrawling_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.trawlingLayer.Enabled = false;
        }
        private void ChkHabitat_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.measuringHabitat = true;
        }
        private void ChkGradnatLayer_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.gradnatLayer != null)
                mw.gradnatLayer.Enabled = true;
        }

        private void ChkGradnatLayer_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.gradnatLayer != null)
                mw.gradnatLayer.Enabled = false;
        }
        private void ChkFireClass_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.fireClassLayer != null)
            {
                mw.fireClassLayer.Enabled = true;
                mw.BrandLegend.Visibility = Visibility.Visible;
            }
        }

        private void ChkFireClass_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.fireClassLayer != null)
            {
                mw.fireClassLayer.Enabled = false;
                mw.BrandLegend.Visibility = Visibility.Collapsed;
            }
        }
        private void ChkEtymologi_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.etymologiLayer != null)
                mw.etymologiLayer.Enabled = true;
        }

        private void ChkEtymologi_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.etymologiLayer != null)
                mw.etymologiLayer.Enabled = false;
        }
        private void ChkTv_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.tvLayer != null)
                mw.tvLayer.Enabled = true;
        }

        private void ChkTv_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.tvLayer != null)
                mw.tvLayer.Enabled = false;
        }
        private void ChkFilm_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.filmLayer != null)
                mw.filmLayer.Enabled = true;
        }

        private void ChkFilm_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.filmLayer != null)
                mw.filmLayer.Enabled = false;
        }
        private void ChkFootball_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.footballLayer != null)
                mw.footballLayer.Enabled = true;
        }
        private void ChkOrt_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.OrtNamnselementOverlay != null)
                mw.OrtNamnselementOverlay.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void ChkOrt_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.OrtNamnselementOverlay != null)
                mw.ShowOrtNamnselement();
        }

        private void ChkFootball_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.footballLayer != null)
                mw.footballLayer.Enabled = false;
        }
        private void ChkMarkTemp_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.heatLayer != null)
                mw.heatLayer.Enabled = true;
        }

        private void ChkMarkTemp_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.heatLayer != null)
                mw.heatLayer.Enabled = false;
        }
        private void ChkRuinsLayer_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.ruinLayer != null)
                mw.ruinLayer.Enabled = true;
        }

        private void ChkRuinsLayer_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.ruinLayer != null)
                mw.ruinLayer.Enabled = false;
        }
        private void ChkEbh_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
            {
                mw.ebhLayer0.Enabled = true;
                mw.ebhLayer1.Enabled = true;
                mw.ebhLayer2.Enabled = true;
                mw.ebhLayer3.Enabled = true;
                mw.ebhLayer4.Enabled = true;
            }
        }

        private void ChkEbh_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
            {
                mw.ebhLayer0.Enabled = false;
                mw.ebhLayer1.Enabled = false;
                mw.ebhLayer2.Enabled = false;
                mw.ebhLayer3.Enabled = false;
                mw.ebhLayer4.Enabled = false;
            }
        }

        private void ChkStationLayer_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.stationLayer != null)
                mw.stationLayer.Enabled = true;
        }

        private void ChkStationLayer_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.stationLayer != null)
                mw.stationLayer.Enabled = false;
        }
        private void ChkRailLayer_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.railwayLayer != null)
                mw.railwayLayer.Enabled = true;
        }

        private void ChkRailLayer_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.railwayLayer != null)
                mw.railwayLayer.Enabled = false;
        }
        private void ChkFyr_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.fyrLayer != null)
                mw.fyrLayer.Enabled = true;
        }
        private void ChkFyr_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.fyrLayer != null)
                mw.fyrLayer.Enabled = false;
        }
        private void ChkGolf_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.golfLayer != null)
                mw.golfLayer.Enabled = true;
        }
        private void ChkGolf_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.golfLayer != null)
                mw.golfLayer.Enabled = false;
        }
        private void ChkBomb_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.bombLayer != null)
                mw.bombLayer.Enabled = true;
        }
        private void ChkBomb_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw && mw.bombLayer != null)
                mw.bombLayer.Enabled = false;
        }
        private void ChkHabitat_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.measuringHabitat = false;
        }
        private void OpenLogWindow_Click(object sender, RoutedEventArgs e)
        {
            if (LogWindow.Instance == null)
            {
                new LogWindow().Show();
            }
            else
            {
                LogWindow.Instance.Activate();
            }
        }
        private void ChkStudies_Checked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.studyHandler.Enabled = true;
        }

        private void ChkStudies_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
                mw.studyHandler.Enabled = false;
        }
        private void TxtStudyTagFilter_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtStudyTagFilter.Text == "Tag-filter (valfritt)")
            {
                TxtStudyTagFilter.Text = "";
                TxtStudyTagFilter.Foreground = Brushes.Black;
            }
        }

        private void TxtStudyTagFilter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Owner is MainWindow mw)
            {
                if (TxtStudyTagFilter.Text == "Tag-filter (valfritt)" ||
                    string.IsNullOrWhiteSpace(TxtStudyTagFilter.Text))
                {
                    mw.studyHandlerTagFilter = null;
                }
                else
                {
                    mw.studyHandlerTagFilter = TxtStudyTagFilter.Text.Trim();
                }
            }

            // placeholder-logik
            if (string.IsNullOrWhiteSpace(TxtStudyTagFilter.Text))
            {
                TxtStudyTagFilter.Text = "Tag-filter (valfritt)";
                TxtStudyTagFilter.Foreground = Brushes.Gray;
            }
        }
        private async void RunTimeSeries_Click(object sender, RoutedEventArgs e)
        {
            var mw = Owner as MainWindow;
            if (mw == null)
            {
                mw.manualActivation = true;

                await mw.TimeSerieLauncher("data/geojson/debug.geojson");

            }
        }
        private void CleanDb_Click(object sender, RoutedEventArgs e)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string dbPath = System.IO.Path.Combine(appData, "GeoViewSE", "timeseries.db");

            if (File.Exists(dbPath))
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                File.Delete(dbPath);
                MessageBox.Show("Tidsserie-databasen har rensats.");
            }

            else
            {
                MessageBox.Show("Ingen databasfil hittades.");
            }
        }

    }
}
