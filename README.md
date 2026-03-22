# Weather Station Monitor

**Module:** Interface Software Development | **Variant:** 16
**Student:** Alan O'Connell (R00243626) | **Institution:** MTU

WPF app in C# that loads weather data from two Met Eireann stations (Sherkin Island and Roches Point) and plots them on a LiveCharts chart for comparison.

## Status

Core features working: data loading, parameter selection, dual-station plotting. Slider/range and differences file still in progress. See the [progress report](docs/Weather%20Station%20Project%20Progress%20-%20AOC.pdf) for a full breakdown.

## Screenshots

![Temperature comparison](docs/Screenshots/Screenshot%202026-03-17%20001644.png)
*Temperature — both stations plotted with full 12-hour range*

![Filtering bug](docs/Screenshots/Screenshot%202026-03-17%20000147.png)
*Known bug — extra .txt files (Rainfall1, test) showing in Sherkin ListBox*

## How it works

1. **Load Data** scans `SherkinWeatherData/` and `RochesWeatherData/` folders and fills two ListBoxes with available parameters (Temperature, Speed, Rainfall). Time files are excluded.
2. **Double-click** a parameter to read the .txt file and store the readings in a `SortedDictionary<DateTime, double>`.
3. **Plot** draws both stations as `LineSeries` on a `CartesianChart`, with time on the X-axis.
4. **Slider** adjusts a Count value from 1 to max data points.
5. **Range** plots a subset based on Count on a second chart.
6. **Differences file** writes the per-timestamp difference between stations to a .txt file.
7. **Menu bar** has Load Stations, Plot Stations, and Exit Application.

## Tech

- C# / .NET 8 / WPF
- LiveCharts.Wpf 0.9.7
- Visual Studio 2022
- Data from [Met Eireann](https://www.met.ie)

## Project structure

```
Interface-Dev-Weather-Monitor/
├── WeatherStation/
│   ├── WeatherStation.sln
│   └── WeatherStation/
│       ├── MainWindow.xaml          # UI layout (XAML)
│       ├── MainWindow.xaml.cs       # all the C# logic
│       ├── App.xaml / App.xaml.cs
│       ├── WeatherStation.csproj
│       ├── SherkinWeatherData/     # Sherkin .txt files
│       └── RochesWeatherData/      # Roches .txt files
├── Versions/                           # older builds
├── docs/
│   ├── Screenshots/
│   ├── Data/
│   ├── Weather Station Project Progress - AOC.pdf
│   └── OConnell Alan R00243626.pdf
├── .gitignore
└── README.md
```

The main code is in `WeatherStation/WeatherStation/MainWindow.xaml.cs`. The XAML layout is in `MainWindow.xaml`. Weather data files sit alongside the project in `SherkinWeatherData/` and `RochesWeatherData/`.

## Run it

```
git clone https://github.com/Alan64578/Interface-Dev-Weather-Monitor.git
```

1. Open `WeatherStation/WeatherStation.sln` in Visual Studio 2022
2. Restore NuGet packages (`LiveCharts.Wpf` 0.9.7)
3. Build and run (F5)

The data folders (`SherkinWeatherData` and `RochesWeatherData`) are copied to the output directory automatically via the .csproj config.

## Data source

- [Sherkin Island — Met Eireann](https://www.met.ie/latest-reports/observations/download/sherkin-island)
- [Roches Point — Met Eireann](https://www.met.ie/latest-reports/observations/download/roches-point)

## Docs

- [Progress report](docs/Weather%20Station%20Project%20Progress%20-%20AOC.pdf) — weekly timeline, testing, outstanding work
- [Project brief](docs/OConnell%20Alan%20R00243626.pdf) — original assignment spec (Variant 16)

## Note

AI tools were used to help format this README. All project code was written by the author.
