// Alan O'Connell R00243626
// Interface Software Development L8 - Variant 16

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;

namespace WeatherStationApp
{
    public partial class MainWindow : Window
    {
        // paths to the two data folders, set in the constructor
        private string sherkinFolder;
        private string rochesFolder;

        // SortedDictionary stores key/value pairs - key is the time string, value is the reading
        // the brief says "construct key/value pairs for the chosen parameter's values"
        // https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.sorteddictionary-2
        private SortedDictionary<string, double> sherkinData = new SortedDictionary<string, double>();
        private SortedDictionary<string, double> rochesData  = new SortedDictionary<string, double>();

        // stores the selected parameter name e.g. "Temperature"
        private string sherkinParam = "";
        private string rochesParam  = "";

        public MainWindow()
        {
            InitializeComponent();

            // BaseDirectory is where the exe is built
            // the csproj copies the data folders there so they are always beside the exe
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            sherkinFolder = Path.Combine(baseDir, "SherkinWeatherData");
            rochesFolder  = Path.Combine(baseDir, "RochesWeatherData");
        }

        // menu handlers - call the same methods as the buttons
        private void MenuLoadStations_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void MenuPlot_Click(object sender, RoutedEventArgs e)
        {
            PlotMain();
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnLoadData_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void BtnPlot_Click(object sender, RoutedEventArgs e)
        {
            PlotMain();
        }

        private void BtnRange_Click(object sender, RoutedEventArgs e)
        {
            PlotRange();
        }

        // scans both folders and fills the list boxes
        // Time files are excluded - the brief says not to show them
        // https://learn.microsoft.com/en-us/dotnet/api/system.io.directory.getfiles
        private void LoadData()
        {
            ListBox1.Items.Clear();
            ListBox2.Items.Clear();

            try
            {
                if (!Directory.Exists(sherkinFolder))
                    throw new DirectoryNotFoundException("SherkinWeatherData folder not found:\n" + sherkinFolder);

                foreach (string file in Directory.GetFiles(sherkinFolder, "*.txt"))
                {
                    string name = Path.GetFileNameWithoutExtension(file);
                    if (!name.Contains("Time"))
                        ListBox1.Items.Add(name);
                }

                if (!Directory.Exists(rochesFolder))
                    throw new DirectoryNotFoundException("RochesWeatherData folder not found:\n" + rochesFolder);

                foreach (string file in Directory.GetFiles(rochesFolder, "*.txt"))
                {
                    string name = Path.GetFileNameWithoutExtension(file);
                    if (!name.Contains("Time"))
                        ListBox2.Items.Add(name);
                }

                TbStatus.Text = "Folders loaded. Double-click a parameter in each list.";
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Folder not found", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Access denied", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // double-click on sherkin list - loads that parameter into the SortedDictionary
        private void ListBox1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBox1.SelectedItem == null) return;

            string picked = ListBox1.SelectedItem.ToString();
            sherkinParam = picked.Replace("Sherkin ", "").Trim();

            if (ExtractData(sherkinFolder, picked, ref sherkinData))
            {
                LblSherkinParam.Text = sherkinParam;
                UpdateSlider();
                TbStatus.Text = "Sherkin " + sherkinParam + " loaded (" + sherkinData.Count + " readings). Now pick a Roches parameter.";
            }
        }

        // double-click on roches list
        private void ListBox2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBox2.SelectedItem == null) return;

            string picked = ListBox2.SelectedItem.ToString();
            rochesParam = picked.Replace("Roches ", "").Trim();

            if (ExtractData(rochesFolder, picked, ref rochesData))
            {
                LblRochesParam.Text = rochesParam;
                UpdateSlider();
                TbStatus.Text = "Roches " + rochesParam + " loaded (" + rochesData.Count + " readings). Press Plot.";
            }
        }

        // reads the Time file and the parameter file together line by line
        // builds a SortedDictionary<string, double> - key = time, value = reading
        // using block ensures StreamReaders are closed even if an error occurs
        // https://learn.microsoft.com/en-us/dotnet/api/system.io.streamreader
        private bool ExtractData(string folder, string paramFile, ref SortedDictionary<string, double> store)
        {
            store.Clear();

            string prefix   = folder.Contains("Sherkin") ? "Sherkin" : "Roches";
            string timePath = Path.Combine(folder, prefix + " Time.txt");
            string dataPath = Path.Combine(folder, paramFile + ".txt");

            try
            {
                if (!File.Exists(timePath))
                    throw new FileNotFoundException("Time file not found.", timePath);

                if (!File.Exists(dataPath))
                    throw new FileNotFoundException("Data file not found.", dataPath);

                using (StreamReader tr = new StreamReader(timePath))
                using (StreamReader dr = new StreamReader(dataPath))
                {
                    string tLine, dLine;

                    // ReadLine() returns null at end of file
                    while ((tLine = tr.ReadLine()) != null && (dLine = dr.ReadLine()) != null)
                    {
                        tLine = tLine.Trim();
                        dLine = dLine.Trim();

                        if (string.IsNullOrEmpty(tLine) || string.IsNullOrEmpty(dLine))
                            continue;

                        // TryParse skips the line if the value is not a valid number
                        double val;
                        if (double.TryParse(dLine, System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out val))
                        {
                            // SortedDictionary will throw ArgumentException if key already exists
                            // ContainsKey check avoids crashing on duplicate timestamps
                            if (!store.ContainsKey(tLine))
                                store.Add(tLine, val);
                        }
                    }
                }

                return true;
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.FileName, "File not found", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Duplicate key in data", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error reading file", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // sets the slider max to max_element and resets Count to 1
        private void UpdateSlider()
        {
            int maxEl = Math.Max(sherkinData.Count, rochesData.Count);
            if (maxEl < 1) maxEl = 1;

            SliderCount.Minimum = 1;
            SliderCount.Maximum = maxEl;
            SliderCount.Value   = 1;

            LblCount.Text = "1";
            LblMaxEl.Text = maxEl.ToString();
        }

        // keeps the Count label in sync as the slider is dragged
        private void SliderCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LblCount != null)
                LblCount.Text = ((int)SliderCount.Value).ToString();
        }

        // plots both stations on the same chart then writes the differences file
        private void PlotMain()
        {
            if (sherkinData.Count == 0 && rochesData.Count == 0)
            {
                MessageBox.Show("Double-click a parameter in each list first.", "No data", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string sp = sherkinParam == "" ? "?" : sherkinParam;
            string rp = rochesParam  == "" ? "?" : rochesParam;

            LblMainTitle.Text = "Parameter Chart | Sherkin: " + sp + " vs Roches: " + rp;

            // build the series and labels from the SortedDictionary the same way the slides do
            // slides show: chart.Series = new SeriesCollection { new LineSeries { Values = new ChartValues<double> { ... } } }
            SeriesCollection series = BuildSeries(sherkinData, rochesData, sp, rp);
            List<string> labels     = BuildLabels(sherkinData, rochesData);

            mainChart.Series           = series;
            mainAxisX.Labels           = labels;
            mainAxisX.Title            = "Time";
            mainAxisY.Title            = sp;

            WriteDiffs();
            TbStatus.Text = "Plotted " + sp + " (Sherkin) vs " + rp + " (Roches). Differences written to file.";
        }

        // shows only the first Count readings on the separate range chart
        private void PlotRange()
        {
            if (sherkinData.Count == 0 && rochesData.Count == 0)
            {
                MessageBox.Show("Load and extract data first.", "No data", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int count = (int)SliderCount.Value;
            int maxEl = Math.Max(sherkinData.Count, rochesData.Count);

            // copy the first 'count' entries from each SortedDictionary into subsets
            SortedDictionary<string, double> sSub = new SortedDictionary<string, double>();
            SortedDictionary<string, double> rSub = new SortedDictionary<string, double>();

            int i = 0;
            foreach (KeyValuePair<string, double> kvp in sherkinData)
            {
                if (i >= count) break;
                sSub.Add(kvp.Key, kvp.Value);
                i++;
            }

            i = 0;
            foreach (KeyValuePair<string, double> kvp in rochesData)
            {
                if (i >= count) break;
                rSub.Add(kvp.Key, kvp.Value);
                i++;
            }

            string sp = sherkinParam == "" ? "?" : sherkinParam;
            string rp = rochesParam  == "" ? "?" : rochesParam;

            LblRangeTitle.Text = "Range Chart | First " + count + " of " + maxEl
                + " readings | Sherkin: " + sp + " vs Roches: " + rp;

            SeriesCollection series = BuildSeries(sSub, rSub, sp, rp);
            List<string> labels     = BuildLabels(sSub, rSub);

            rangeChart.Series  = series;
            rangeAxisX.Labels  = labels;
            rangeAxisX.Title   = "Time";
            rangeAxisY.Title   = sp;

            TbStatus.Text = "Range chart showing first " + count + " of " + maxEl + " readings.";
        }

        // builds a SeriesCollection with two LineSeries, one per station
        // matches the slides: new SeriesCollection { new LineSeries { Title = "...", Values = new ChartValues<double> { ... } } }
        private SeriesCollection BuildSeries(SortedDictionary<string, double> sData,
                                             SortedDictionary<string, double> rData,
                                             string sLabel, string rLabel)
        {
            ChartValues<double> sValues = new ChartValues<double>();
            foreach (KeyValuePair<string, double> kvp in sData)
                sValues.Add(kvp.Value);

            ChartValues<double> rValues = new ChartValues<double>();
            foreach (KeyValuePair<string, double> kvp in rData)
                rValues.Add(kvp.Value);

            SeriesCollection collection = new SeriesCollection
            {
                new LineSeries
                {
                    Title  = "Sherkin - " + sLabel,
                    Values = sValues,
                    LineSmoothness = 0
                },
                new LineSeries
                {
                    Title  = "Roches - " + rLabel,
                    Values = rValues,
                    LineSmoothness = 0
                }
            };

            return collection;
        }

        // builds the x-axis label list from the keys of the larger dictionary
        private List<string> BuildLabels(SortedDictionary<string, double> sData,
                                         SortedDictionary<string, double> rData)
        {
            List<string> labels = new List<string>();
            SortedDictionary<string, double> bigger = sData.Count >= rData.Count ? sData : rData;

            foreach (KeyValuePair<string, double> kvp in bigger)
                labels.Add(kvp.Key);

            return labels;
        }

        // writes the value difference at each time step to a csv file
        // StreamWriter from the File Access slides
        // https://learn.microsoft.com/en-us/dotnet/api/system.io.streamwriter
        private void WriteDiffs()
        {
            if (sherkinData.Count == 0 || rochesData.Count == 0) return;

            string outPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "differences.txt");

            try
            {
                using (StreamWriter sw = new StreamWriter(outPath, false))
                {
                    sw.WriteLine("Date,Time,Sherkin " + sherkinParam + ",Roches " + rochesParam + ",Difference");

                    // iterate both dictionaries together using enumerators - shown in slides
                    SortedDictionary<string, double>.Enumerator sEnum = sherkinData.GetEnumerator();
                    SortedDictionary<string, double>.Enumerator rEnum = rochesData.GetEnumerator();

                    while (sEnum.MoveNext() && rEnum.MoveNext())
                    {
                        double s    = sEnum.Current.Value;
                        double r    = rEnum.Current.Value;
                        double diff = s - r;

                        sw.WriteLine(DateTime.Today.ToString("yyyy-MM-dd") + ","
                            + sEnum.Current.Key + "," + s + "," + r + "," + diff.ToString("F4"));
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Could not write file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
