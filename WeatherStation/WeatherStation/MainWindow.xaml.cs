using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;

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

        private SortedDictionary<DateTime, double> _sherkinData = new SortedDictionary<DateTime, double>();
        private SortedDictionary<DateTime, double> _rochesData  = new SortedDictionary<DateTime, double>();

        public List<string> PlotLabels  { get; set; } = new List<string>();
        public List<string> RangeLabels { get; set; } = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        // ── Load Data ──────────────────────────────────────────────────────────
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

                if (fileName.EndsWith("Time", StringComparison.OrdinalIgnoreCase))
                    continue;

                string paramName = fileName.Replace(stationPrefix, "").Trim();
                listBox.Items.Add(paramName);
            }
        }

        // ── Double-click selection ─────────────────────────────────────────────
        private void LstSherkin_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LstSherkin.SelectedItem == null) return;

            string param = LstSherkin.SelectedItem.ToString()!;
            _sherkinParamFile = Path.Combine(SherkinFolder, $"Sherkin {param}.txt");
            TxtSherkinParam.Text = param;

            EnablePlotIfReady();
        }

        private void LstRoches_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LstRoches.SelectedItem == null) return;

            string param = LstRoches.SelectedItem.ToString()!;
            _rochesParamFile = Path.Combine(RochesFolder, $"Roches {param}.txt");
            TxtRochesParam.Text = param;

            EnablePlotIfReady();
        }

        private void EnablePlotIfReady()
        {
            bool ready = !string.IsNullOrEmpty(_sherkinParamFile) &&
                         !string.IsNullOrEmpty(_rochesParamFile);

            BtnPlot.IsEnabled         = ready;
            MenuPlotStations.IsEnabled = ready;
        }

        // ── Extract data from files into SortedDictionary ─────────────────────
        private SortedDictionary<DateTime, double> ExtractData(string timeFile, string paramFile)
        {
            var result = new SortedDictionary<DateTime, double>();

            try
            {
                string[] times  = File.ReadAllLines(timeFile);
                string[] values = File.ReadAllLines(paramFile);

                for (int i = 0; i < Math.Min(times.Length, values.Length); i++)
                {
                    string timeStr = DateTime.Today.ToString("yyyy-MM-dd") + " " + times[i].Trim();

                    if (DateTime.TryParse(timeStr, out DateTime dt) &&
                        double.TryParse(values[i].Trim(), out double val))
                    {
                        result[dt] = val;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading data:\n{ex.Message}",
                                "Read Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return result;
        }

        // ── Plot button ────────────────────────────────────────────────────────
        private void BtnPlot_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_sherkinParamFile) || string.IsNullOrEmpty(_rochesParamFile))
            {
                MessageBox.Show("Please double-click a parameter in each ListBox before plotting.",
                                "No Parameter Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string sherkinTimeFile = Path.Combine(SherkinFolder, "Sherkin Time.txt");
            string rochesTimeFile  = Path.Combine(RochesFolder,  "Roches Time.txt");

            _sherkinData = ExtractData(sherkinTimeFile, _sherkinParamFile);
            _rochesData  = ExtractData(rochesTimeFile,  _rochesParamFile);

            if (_sherkinData.Count == 0 || _rochesData.Count == 0)
            {
                TxtStatus.Text = "No data to plot.";
                return;
            }

            // Build labels and values from Sherkin (both share same time axis)
            PlotLabels = _sherkinData.Keys.Select(dt => dt.ToString("HH:mm")).ToList();

            var sherkinSeries = new LineSeries
            {
                Title      = "Sherkin Island",
                Values     = new ChartValues<double>(_sherkinData.Values),
                PointGeometrySize = 6,
                StrokeThickness   = 2
            };

            var rochesSeries = new LineSeries
            {
                Title      = "Roches Point",
                Values     = new ChartValues<double>(_rochesData.Values),
                PointGeometrySize = 6,
                StrokeThickness   = 2
            };

            ChartPlot.Series = new SeriesCollection { sherkinSeries, rochesSeries };

            // Update X axis labels
            if (ChartPlot.AxisX.Count > 0)
                ChartPlot.AxisX[0].Labels = PlotLabels;

            // Update chart title and status
            string paramName = TxtSherkinParam.Text;
            TxtPlotTitle.Text = $"Parameter Chart — {paramName}";
            TxtStatus.Text    = $"Plotted {_sherkinData.Count} readings for {paramName}.";

            // Enable slider and range button
            SldCount.Minimum   = 1;
            SldCount.Maximum   = _sherkinData.Count;
            SldCount.Value     = _sherkinData.Count;
            SldCount.IsEnabled = true;
            BtnRange.IsEnabled = true;

            TxtCount.Text      = _sherkinData.Count.ToString();
            TxtCountRange.Text = $"1 – {_sherkinData.Count}";

            // Write differences file
            WriteDifferences(paramName);
        }

        // ── Slider value changed ───────────────────────────────────────────────
        private void SldCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int count = (int)SldCount.Value;
            TxtCount.Text = count.ToString();
        }

        // ── Range button ───────────────────────────────────────────────────────
        private void BtnRange_Click(object sender, RoutedEventArgs e)
        {
            if (_sherkinData.Count == 0 || _rochesData.Count == 0)
            {
                MessageBox.Show("Please plot data before using Range.",
                                "No Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int count = (int)SldCount.Value;

            var sherkinSubset = _sherkinData.Take(count).ToList();
            var rochesSubset  = _rochesData.Take(count).ToList();

            RangeLabels = sherkinSubset.Select(kv => kv.Key.ToString("HH:mm")).ToList();

            var sherkinSeries = new LineSeries
            {
                Title      = "Sherkin Island",
                Values     = new ChartValues<double>(sherkinSubset.Select(kv => kv.Value)),
                PointGeometrySize = 6,
                StrokeThickness   = 2
            };

            var rochesSeries = new LineSeries
            {
                Title      = "Roches Point",
                Values     = new ChartValues<double>(rochesSubset.Select(kv => kv.Value)),
                PointGeometrySize = 6,
                StrokeThickness   = 2
            };

            ChartRange.Series = new SeriesCollection { sherkinSeries, rochesSeries };

            if (ChartRange.AxisX.Count > 0)
                ChartRange.AxisX[0].Labels = RangeLabels;

            string paramName  = TxtSherkinParam.Text;
            TxtRangeTitle.Text = $"Range Chart — {paramName} ({count} readings)";
            TxtStatus.Text     = $"Range chart showing {count} of {_sherkinData.Count} readings.";
        }

        // ── Write differences file ─────────────────────────────────────────────
        private void WriteDifferences(string paramName)
        {
            try
            {
                string outputPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "differences.txt");

                using var writer = new StreamWriter(outputPath, append: false);

                writer.WriteLine($"Parameter: {paramName}");
                writer.WriteLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine(new string('-', 40));
                writer.WriteLine("DateTime, Difference (Sherkin - Roches)");
                writer.WriteLine(new string('-', 40));

                foreach (var kvp in _sherkinData)
                {
                    if (_rochesData.TryGetValue(kvp.Key, out double rochesVal))
                    {
                        double diff = kvp.Value - rochesVal;
                        writer.WriteLine($"{kvp.Key:HH:mm}, {diff:F2}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not write differences file:\n{ex.Message}",
                                "File Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ── Menu exit ──────────────────────────────────────────────────────────
        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
