# Interface Development — Weather Station Monitor

> **Module:** Interface Software Development | **Level:** 8 | **Variant:** 16  
> **Author:** Alan O'Connell | **Student No:** R00243626 | **Institution:** Munster Technological University

A WPF desktop application written in C# that reads, parses and compares weather data from two Met Éireann stations — **Sherkin Island** and **Roches Point** — displaying temperature, wind speed, and rainfall over a 12-hour period on an interactive chart.

🌐 **[View Project Page (GitHub Pages)](https://alan64578.github.io/Interface-Dev-Weather-Monitor/)**

---

## Features

- Load weather data from two stations via the **[Load Data]** button
- Scan `SherkinWeatherData` and `RochesWeatherData` folders and populate separate ListBoxes
- Double-click a parameter in a ListBox to select it for extraction
- Extract time-stamped readings and store them as `SortedDictionary<string, double>` key/value pairs
- Display the selected parameter name in a label
- **[Plot]** button renders both station series on the same LiveCharts chart
- Potentiometer (Slider) control adjusts a Count value from 1 to the number of data points
- **[Range]** button plots a subset of data based on the current Count value
- Writes parameter difference values (Sherkin vs Roches) to a `differences.txt` file
- Menu bar with **[Load Stations]**, **[Plot Stations]**, and **[Exit Application]** options

---

## Technologies

| Technology | Details |
|---|---|
| Language | C# / .NET 8 |
| UI Framework | WPF (Windows Presentation Foundation) |
| Charting | LiveCharts v0 (`LiveCharts.Wpf` 0.9.7) |
| Data Structure | `SortedDictionary<string, double>` |
| File I/O | `StreamReader`, `StreamWriter`, `Directory.GetFiles` |
| IDE | Visual Studio 2022 |
| Data Source | Met Éireann open observations |

---

## Project Structure

```
Interface-Dev-Weather-Monitor/
│
├── Versions/
│   └── WeatherStationApp/
│       └── WeatherStationApp/
│           ├── MainWindow.xaml
│           ├── MainWindow.xaml.cs
│           ├── App.xaml / App.xaml.cs
│           ├── WeatherStationApp.csproj
│           ├── SherkinWeatherData/
│           │   ├── Sherkin Temperature.txt
│           │   ├── Sherkin Speed.txt
│           │   ├── Sherkin Rainfall.txt
│           │   └── Sherkin Time.txt
│           └── RochesWeatherData/
│               ├── Roches Temperature.txt
│               ├── Roches Speed.txt
│               ├── Roches Rainfall.txt
│               └── Roches Time.txt
│
├── Docs/
│   └── index.html          ← GitHub Pages project site
│
├── Data/
│   ├── sherkin-island.csv  ← Raw Met Éireann data
│   └── roches-point.csv
│
└── README.md
```

---

## How It Works

1. **Data Preparation** — Weather readings from Met Éireann are split into separate `.txt` files by parameter (Temperature, Speed, Rainfall, Time) for each station.
2. **Load Data** — `Directory.GetFiles()` scans each station folder and populates the ListBoxes with the available parameter files (Time is excluded from display).
3. **Selection** — Double-clicking a ListBox item reads the corresponding file using `StreamReader` and pairs each time value (from the Time file) with the parameter reading into a `SortedDictionary`.
4. **Plotting** — Both station datasets are bound to a LiveCharts `CartesianChart` as separate `LineSeries`, plotted against date/time on the X-axis.
5. **Range Control** — A `Slider` maps a Count value (1 to max elements) to control how many data points are shown on the Range chart.
6. **Difference File** — On plotting, the per-timestamp difference between the two stations is written to `differences.txt` using `StreamWriter`.

---

## Running the Project

```bash
git clone https://github.com/Alan64578/Interface-Dev-Weather-Monitor.git
```

1. Open `Versions/WeatherStationApp/WeatherStationApp.slnx` in **Visual Studio 2022**
2. Ensure `LiveCharts.Wpf` (v0.9.7) is installed via NuGet
3. Build and run (`F5`)
4. The `SherkinWeatherData` and `RochesWeatherData` folders must be present alongside the executable (they are included in the repo)

---

## Data Source

Raw observation data downloaded from:
- https://www.met.ie/latest-reports/observations/download/sherkin-island
- https://www.met.ie/latest-reports/observations/download/roches-point

Raw CSV files are stored in the `Data/` folder of this repository.

---

## Disclaimer

AI tools were used to help format and present this GitHub repository (README and GitHub Pages site). All project code and implementation were written manually by the author.
