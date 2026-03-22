# Weather Station Monitor

**Module:** Interface Software Development | **Variant:** 16
**Student:** Alan O'Connell (R00243626) | **Institution:** MTU

WPF app in C# that loads weather data from two Met Eireann stations (Sherkin Island and Roches Point) and plots them on a LiveCharts chart for comparison.

## Status

Core features working: data loading, parameter selection, dual-station plotting. Slider/range and differences file still in progress.

## Screenshot

![Temperature comparison](docs/Screenshots/Screenshot%202026-03-17%20001644.png)

## How it works

1. **Load Data** scans the station folders, fills two ListBoxes with parameters (Temperature, Speed, Rainfall). Time files excluded.
2. **Double-click** a parameter to read the .txt file and store readings in a `SortedDictionary<DateTime, double>`.
3. **Plot** draws both stations as LineSeries on a CartesianChart.
4. **Slider** adjusts a Count value from 1 to max data points.
5. **Range** plots a subset based on Count on a second chart.
6. **Differences file** writes per-timestamp difference between stations to a .txt file.
7. **Menu bar** has Load Stations, Plot Stations, and Exit Application.

## Tech

- C# / .NET 8 / WPF
- LiveCharts.Wpf 0.9.7
- Visual Studio 2022
- Data from [Met Eireann](https://www.met.ie)

## Run it

```
git clone https://github.com/Alan64578/Interface-Dev-Weather-Monitor.git
```

Open `WeatherStationApp/WeatherStationApp.slnx` in Visual Studio, restore NuGet packages, build and run (F5).

## Docs

- [Progress report](docs/Weather%20Station%20Project%20Progress%20-%20AOC.pdf)
- [Project brief](docs/OConnell%20Alan%20R00243626.pdf)

## Note

AI tools were used to format this README. All project code was written by the author.
