# Interface Development — Weather Station Monitor

A WPF desktop application written in C# that reads, parses and compares weather data from two Met Éireann stations — Sherkin Island and Roches Point — displaying temperature, wind speed, and rainfall over a 12-hour period.

---

## Current Progress

| Feature | Status |
|---|---|
| WPF layout — ListBoxes, labels, menu, buttons | ✅ Complete |
| Load Data — scans folders, populates ListBoxes | ✅ Complete |
| Double-click selection, Parameter label update | ✅ Complete |
| Data extraction into SortedDictionary | ✅ Complete |
| Plot button — LiveCharts dual-series chart | ✅ Complete |
| Range / Count / Slider — subset chart | ⏳ Outstanding |
| Differences file output | ⏳ Outstanding |
| Menu item wiring + error handling | ⏳ Outstanding |

---

## Features (Completed)

- Load weather data from two stations via the **[Load Data]** button
- Scan `SherkinWeatherData` and `RochesWeatherData` folders and populate separate ListBoxes
- Double-click a parameter in a ListBox to select it for extraction
- Selected parameter name displayed in a label beneath each ListBox
- Parse Time and parameter files into `SortedDictionary<DateTime, double>` for both stations
- Plot button renders both station series on the same LiveCharts CartesianChart
- Plot button enabled automatically once a parameter is selected in both ListBoxes
- Menu bar with **[Load Stations]**, **[Plot Stations]**, and **[Exit Application]** options

---

## Technologies

| Technology | Details |
|---|---|
| Language | C# / .NET 8 |
| UI Framework | WPF (Windows Presentation Foundation) |
| Charting | LiveCharts v0 (LiveCharts.Wpf 0.9.7) |
| Data Structure | SortedDictionary\<DateTime, double\> |
| File I/O | StreamReader, Directory.GetFiles |
| IDE | Visual Studio 2022 |
| Data Source | Met Éireann open observations |

---

## Project Structure

```
Interface-Dev-Weather-Monitor/
│
├── WeatherStation/
│   ├── WeatherStation.sln
│   └── WeatherStation/
│       ├── MainWindow.xaml
│       ├── MainWindow.xaml.cs
│       ├── App.xaml
│       ├── App.xaml.cs
│       ├── WeatherStation.csproj
│       ├── SherkinWeatherData/
│       │   ├── Sherkin Temperature.txt
│       │   ├── Sherkin Speed.txt
│       │   ├── Sherkin Rainfall.txt
│       │   └── Sherkin Time.txt
│       └── RochesWeatherData/
│           ├── Roches Temperature.txt
│           ├── Roches Speed.txt
│           ├── Roches Rainfall.txt
│           └── Roches Time.txt
│
├── docs/
│   ├── OConnell Alan R00243626.pdf
│   ├── Data/
│   │   ├── sherkin-island.csv
│   │   └── roches-point.csv
│   └── Screenshots/
│
├── Versions/
├── .gitignore
└── README.md
```

---

## Running the Project

1. Clone the repo
2. Open `WeatherStation/WeatherStation.sln` in Visual Studio 2022
3. Ensure **LiveCharts.Wpf (v0.9.7)** is installed via NuGet
4. Build and run (F5)

The `SherkinWeatherData` and `RochesWeatherData` folders are included and copy to the output directory on build.

---

## Data Source

- https://www.met.ie/latest-reports/observations/download/sherkin-island
- https://www.met.ie/latest-reports/observations/download/roches-point

---

## Disclaimer

AI tools were used to help format and present this GitHub repository (README). All project code and implementation were written manually by the author.
