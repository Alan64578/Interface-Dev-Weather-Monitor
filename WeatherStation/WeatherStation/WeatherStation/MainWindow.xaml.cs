using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;

namespace WeatherStation
{

    public partial class MainWindow : Window
    {
        // paths to data folders
        private string _sherkinFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "SherkinWeatherData");

        private string _rochesFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "RochesWeatherData");

        private string _sherkinParamFile = "";
        private string _rochesParamFile = "";

        private SortedDictionary<DateTime, double> _sherkinData = new SortedDictionary<DateTime, double>();
        private SortedDictionary<DateTime, double> _rochesData = new SortedDictionary<DateTime, double>();

        // labels for x-axis on both charts
        public List<string> PlotLabels { get; set; } = new List<string>();
        public List<string> RangeLabels { get; set; } = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        // resets everything and loads both station folders into the listboxes
        private void BtnLoadData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _sherkinParamFile = "";
                _rochesParamFile = "";
                TxtSherkinParam.Text = "";
                TxtRochesParam.Text = "";
                _sherkinData.Clear();
                _rochesData.Clear();

                // https://learn.microsoft.com/en-us/dotnet/api/system.windows.uielement.isenabled
                BtnPlot.IsEnabled = false;
                MenuPlotStations.IsEnabled = false;
                BtnRange.IsEnabled = false;
                SldCount.IsEnabled = false;
                MenuWriteDiff.IsEnabled = false;
                TxtCount.Text = "";
                TxtCountRange.Text = "";

                ChartPlot.Series = null;
                ChartRange.Series = null;
                TxtPlotTitle.Text = "Parameter Chart";
                TxtRangeTitle.Text = "Range Chart";

                LoadStation(_sherkinFolder, LstSherkin, "Sherkin");
                LoadStation(_rochesFolder, LstRoches, "Roches");
                TxtStatus.Text = "Data loaded.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data folders:\n" + ex.Message,
                    "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // scans folder and fills the listbox, leaves out Time
        private void LoadStation(string folder, ListBox listBox, string stationPrefix)
        {
            if (!Directory.Exists(folder))
            {
                MessageBox.Show("Folder not found:\n" + folder,
                    "Missing Folder", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            listBox.Items.Clear();

            string[] files = Directory.GetFiles(folder, "*.txt");

            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileNameWithoutExtension(files[i]);

                // dont put Time in the listbox
                // https://learn.microsoft.com/en-us/dotnet/api/system.string.tolower
                // https://learn.microsoft.com/en-us/dotnet/api/system.string.endswith
                if (fileName.ToLower().EndsWith("time"))
                    continue;

                // https://learn.microsoft.com/en-us/dotnet/api/system.string.replace
                // https://learn.microsoft.com/en-us/dotnet/api/system.string.trim
                string paramName = fileName.Replace(stationPrefix, "").Trim();
                listBox.Items.Add(paramName);
            }
        }

        // when user double clicks a param in the sherkin listbox, store the file path and check if both are ready
        // https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.control.mousedoubleclick
        private void LstSherkin_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LstSherkin.SelectedItem == null) return;

            string param = LstSherkin.SelectedItem.ToString();
            _sherkinParamFile = Path.Combine(_sherkinFolder, "Sherkin " + param + ".txt");
            TxtSherkinParam.Text = param;

            CheckReady();
        }

        // same as sherkin but for roches point
        private void LstRoches_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LstRoches.SelectedItem == null) return;

            string param = LstRoches.SelectedItem.ToString();
            _rochesParamFile = Path.Combine(_rochesFolder, "Roches " + param + ".txt");
            TxtRochesParam.Text = param;

            CheckReady();
        }

        // enables Plot once both files are picked
        private void CheckReady()
        {
            // https://learn.microsoft.com/en-us/dotnet/api/system.io.file.exists
            bool ready = _sherkinParamFile != "" &&
                         _rochesParamFile != "" &&
                         File.Exists(_sherkinParamFile) &&
                         File.Exists(_rochesParamFile);

            BtnPlot.IsEnabled = ready;
            MenuPlotStations.IsEnabled = ready;
        }

        // reads the time file and param file line by line, pairs them up into a SortedDictionary
        private SortedDictionary<DateTime, double> ExtractData(string timeFile, string paramFile)
        {
            SortedDictionary<DateTime, double> result = new SortedDictionary<DateTime, double>();
            StreamReader srTime = null;
            StreamReader srValues = null;

            try
            {
                srTime = new StreamReader(timeFile);
                srValues = new StreamReader(paramFile);

                string timeLine;
                string valueLine;

                while ((timeLine = srTime.ReadLine()) != null &&
                       (valueLine = srValues.ReadLine()) != null)
                {
                    // stick date on the front
                    // https://learn.microsoft.com/en-us/dotnet/api/system.datetime.today
                    // https://learn.microsoft.com/en-us/dotnet/api/system.datetime.parse
                    string dateTimeStr = DateTime.Today.ToString("yyyy-MM-dd") + " " + timeLine.Trim();

                    try
                    {
                        DateTime dt = DateTime.Parse(dateTimeStr);
                        // https://learn.microsoft.com/en-us/dotnet/api/system.convert.todouble
                        double val = Convert.ToDouble(valueLine.Trim());
                        result[dt] = val;
                    }
                    catch (FormatException)
                    {

                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("File not found:\n" + ex.Message,
                    "Missing File", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading data:\n" + ex.Message,
                    "Read Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (srTime != null) srTime.Close();
                if (srValues != null) srValues.Close();
            }

            return result;
        }

        // extracts data for both stations and draws two lines on the chart
        private void BtnPlot_Click(object sender, RoutedEventArgs e)
        {
            if (_sherkinParamFile == "" || _rochesParamFile == "")
            {
                MessageBox.Show("Please double-click a parameter in each ListBox before plotting.",
                    "No Parameter Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // cant plot temperature vs rainfall on same axis
            if (TxtSherkinParam.Text.ToLower() != TxtRochesParam.Text.ToLower())
            {
                MessageBox.Show("Please select the same parameter in both listboxes.");
                return;
            }

            try
            {
                string sherkinTimeFile = Path.Combine(_sherkinFolder, "Sherkin Time.txt");
                string rochesTimeFile = Path.Combine(_rochesFolder, "Roches Time.txt");

                _sherkinData = ExtractData(sherkinTimeFile, _sherkinParamFile);
                _rochesData = ExtractData(rochesTimeFile, _rochesParamFile);

                if (_sherkinData.Count == 0 || _rochesData.Count == 0)
                {
                    TxtStatus.Text = "No data to plot.";
                    return;
                }

                // x-axis labels from sherkin timestamps
                PlotLabels.Clear();
                foreach (KeyValuePair<DateTime, double> pair in _sherkinData)
                {
                    PlotLabels.Add(pair.Key.ToString("HH:mm"));
                }

                List<double> sherkinValues = new List<double>();
                foreach (KeyValuePair<DateTime, double> pair in _sherkinData)
                {
                    sherkinValues.Add(pair.Value);
                }

                List<double> rochesValues = new List<double>();
                foreach (KeyValuePair<DateTime, double> pair in _rochesData)
                {
                    rochesValues.Add(pair.Value);
                }

                LineSeries sherkinSeries = new LineSeries
                {
                    Title = "Sherkin Island",
                    Values = new ChartValues<double>(sherkinValues),
                    PointGeometrySize = 6,
                    StrokeThickness = 2
                };

                LineSeries rochesSeries = new LineSeries
                {
                    Title = "Roches Point",
                    Values = new ChartValues<double>(rochesValues),
                    PointGeometrySize = 6,
                    StrokeThickness = 2
                };

                ChartPlot.Series = new SeriesCollection { sherkinSeries, rochesSeries };

                if (ChartPlot.AxisX.Count > 0)
                    ChartPlot.AxisX[0].Labels = PlotLabels;

                string paramName = TxtSherkinParam.Text;
                TxtPlotTitle.Text = "Parameter Chart - " + paramName;
                TxtStatus.Text = "Plotted " + _sherkinData.Count + " readings for " + paramName + ".";

                // maxelement is the smaller count
                int maxElement = _sherkinData.Count;
                if (_rochesData.Count < maxElement)
                    maxElement = _rochesData.Count;

                SldCount.Minimum = 1;
                SldCount.Maximum = maxElement;
                SldCount.Value = 1;
                SldCount.IsEnabled = true;
                TxtCount.Text = "1";
                TxtCountRange.Text = "1 to " + maxElement;

                BtnRange.IsEnabled = true;
                MenuWriteDiff.IsEnabled = true;

                // write differences straight away
                WriteDifferencesFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error plotting data:\n" + ex.Message,
                    "Plot Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // updates the count label whenever the slider moves
        private void SldCount_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (TxtCount == null) return;

            int count = (int)SldCount.Value;
            TxtCount.Text = count.ToString();
        }

        // takes the first N readings based on slider value and plots them on the second chart
        private void BtnRange_Click(object sender, RoutedEventArgs e)
        {
            if (_sherkinData.Count == 0 || _rochesData.Count == 0)
            {
                MessageBox.Show("No data available. Plot data first.",
                    "No Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int count = (int)SldCount.Value;

                List<DateTime> sherkinKeys = new List<DateTime>();
                List<double> sherkinValues = new List<double>();
                int n = 0;
                foreach (KeyValuePair<DateTime, double> pair in _sherkinData)
                {
                    if (n >= count) break;
                    sherkinKeys.Add(pair.Key);
                    sherkinValues.Add(pair.Value);
                    n++;
                }

                List<double> rochesValues = new List<double>();
                n = 0;
                foreach (KeyValuePair<DateTime, double> pair in _rochesData)
                {
                    if (n >= count) break;
                    rochesValues.Add(pair.Value);
                    n++;
                }

                RangeLabels.Clear();
                for (int i = 0; i < sherkinKeys.Count; i++)
                {
                    RangeLabels.Add(sherkinKeys[i].ToString("HH:mm"));
                }

                LineSeries sherkinRange = new LineSeries
                {
                    Title = "Sherkin Island",
                    Values = new ChartValues<double>(sherkinValues),
                    PointGeometrySize = 6,
                    StrokeThickness = 2
                };

                LineSeries rochesRange = new LineSeries
                {
                    Title = "Roches Point",
                    Values = new ChartValues<double>(rochesValues),
                    PointGeometrySize = 6,
                    StrokeThickness = 2
                };

                ChartRange.Series = new SeriesCollection { sherkinRange, rochesRange };

                if (ChartRange.AxisX.Count > 0)
                    ChartRange.AxisX[0].Labels = RangeLabels;

                string paramName = TxtSherkinParam.Text;
                TxtRangeTitle.Text = "Range Chart - " + paramName + " (first " + count + " readings)";
                TxtStatus.Text = "Range: showing " + count + " of " + _sherkinData.Count + " readings.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error displaying range:\n" + ex.Message,
                    "Range Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // finds matching timestamps between both stations and writes the differences to a txt file
        private void WriteDifferencesFile()
        {
            StreamWriter sw = null;

            try
            {
                List<DateTime> commonKeys = new List<DateTime>();
                foreach (KeyValuePair<DateTime, double> pair in _sherkinData)
                {
                    if (_rochesData.ContainsKey(pair.Key))
                    {
                        commonKeys.Add(pair.Key);
                    }
                }

                if (commonKeys.Count == 0)
                {
                    TxtStatus.Text = "No matching timestamps to compute differences.";
                    return;
                }

                string paramName = TxtSherkinParam.Text;
                string diffFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    paramName + "_Differences.txt");

                sw = new StreamWriter(diffFile);
                sw.WriteLine("DateTime\tSherkin\tRoches\tDifference");

                for (int i = 0; i < commonKeys.Count; i++)
                {
                    DateTime key = commonKeys[i];
                    double sVal = _sherkinData[key];
                    double rVal = _rochesData[key];
                    double diff = sVal - rVal;

                    sw.WriteLine(key.ToString("yyyy-MM-dd HH:mm") + "\t" +
                        sVal + "\t" + rVal + "\t" + diff.ToString("F2"));
                }

                TxtStatus.Text = "Differences written to " + Path.GetFileName(diffFile) + ".";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error writing differences:\n" + ex.Message,
                    "Write Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (sw != null) sw.Close();
            }
        }

        // menu option to write the differences file
        private void MenuWriteDiff_Click(object sender, RoutedEventArgs e)
        {
            WriteDifferencesFile();
        }

        // closes the application
        // https://learn.microsoft.com/en-us/dotnet/api/system.windows.application.shutdown
        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
