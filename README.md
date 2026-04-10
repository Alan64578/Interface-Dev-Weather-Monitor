# Weather Station Monitor

**Module:** Interface Software Development
**Student:** Alan O'Connell (R00243626)
**Institution:** MTU

WPF app in C# that loads weather data from two Met Eireann stations (Sherkin Island and Roches Point) and plots them on a LiveCharts chart for comparison.

## Status

All features complete and working.

## How it works

1. **Load Data** scans `SherkinWeatherData/` and `RochesWeatherData/` folders and fills two ListBoxes with the available parameters (Temperature, Speed, Rainfall). Time files are left out.
2. **Double-click** a parameter to read the .txt file and store the readings in a `SortedDictionary<DateTime, double>`.
3. **Plot** draws both stations as `LineSeries` on a `CartesianChart`, with time on the X-axis.
4. **Slider** adjusts a Count value from 1 to max data points.
5. **Range** plots a subset based on Count on a second chart.
6. **Differences file** writes the per-timestamp difference between stations to a tab-separated .txt file in the exe folder.
7. **Menu bar** has Load Stations, Plot Stations, Write Differences, and Exit Application.

## Tech

- C# / .NET 8 / WPF
- LiveCharts.Wpf 0.9.7
- Visual Studio 2022
- Data from [Met Eireann](https://www.met.ie)

## Project structure

```
WeatherStation/
+-- WeatherStation.sln
+-- WeatherStation/
    +-- MainWindow.xaml
    +-- MainWindow.xaml.cs
    +-- App.xaml / App.xaml.cs
    +-- WeatherStation.csproj
    +-- SherkinWeatherData/
    +-- RochesWeatherData/
```

Main code is in `WeatherStation/WeatherStation/MainWindow.xaml.cs`. XAML layout is in `MainWindow.xaml`. Weather data files sit alongside the project in `SherkinWeatherData/` and `RochesWeatherData/`.
