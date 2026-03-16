using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace WeatherStation
{
    public partial class MainWindow : Window
    {
        private readonly string SherkinFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "SherkinWeatherData");

        private readonly string RochesFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "RochesWeatherData");

        private string _sherkinParamFile = string.Empty;
        private string _rochesParamFile  = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLoadData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadStation(SherkinFolder, LstSherkin, "Sherkin");
                LoadStation(RochesFolder,  LstRoches,  "Roches");
                TxtStatus.Text = "Data loaded.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data folders:\n" + ex.Message,
                                "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadStation(string folder, ListBox listBox, string stationPrefix)
        {
            if (!Directory.Exists(folder))
            {
                MessageBox.Show($"Folder not found:\n{folder}",
                                "Missing Folder", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            listBox.Items.Clear();

            string[] files = Directory.GetFiles(folder, "*.txt");

            foreach (string filePath in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                // skip the time file, it shouldn't appear in the list
                if (fileName.EndsWith("Time", StringComparison.OrdinalIgnoreCase))
                    continue;

                string paramName = fileName.Replace(stationPrefix, "").Trim();
                listBox.Items.Add(paramName);
            }
        }

        private void LstSherkin_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LstSherkin.SelectedItem == null) return;

            string param = LstSherkin.SelectedItem.ToString();
            _sherkinParamFile = Path.Combine(SherkinFolder, $"Sherkin {param}.txt");
            TxtSherkinParam.Text = param;
        }

        private void LstRoches_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LstRoches.SelectedItem == null) return;

            string param = LstRoches.SelectedItem.ToString();
            _rochesParamFile = Path.Combine(RochesFolder, $"Roches {param}.txt");
            TxtRochesParam.Text = param;
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
