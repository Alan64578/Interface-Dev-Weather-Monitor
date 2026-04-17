# Weather Station Monitor

Alan O'Connell, Interface Software Development, MTU

WPF app in C# that loads weather data from two Met Eireann stations (Sherkin Island and Roches Point) and plots them on a LiveCharts chart for comparison. All features complete. And Final code version found at "WeatherStation/WeatherStation".

## Screenshots

![Startup](docs/Screenshots/Screenshot%202026-04-10%20184501.png)
*Empty state on launch. Both charts waiting for data.*

![Rainfall plotted](docs/Screenshots/Screenshot%202026-04-10%20184541.png)
*Rainfall for both stations. Slider set to 7, Range chart showing the first 7 readings.*

![Mismatch error](docs/Screenshots/Screenshot%202026-04-10%20184618.png)
*Tried to plot Temperature against Speed. App catches it and shows an error.*

## How it works

**Load Data** scans the two station folders and fills the ListBoxes with Temperature, Speed and Rainfall. Time files are filtered out.

**Double-click** a parameter in each list to select it. The app reads the .txt files and stores the values in a `SortedDictionary<DateTime, double>`.

**Plot** draws both stations on the same chart. A slider underneath controls how many readings the Range chart shows.

**Write Differences** outputs a tab-separated .txt file with the per-timestamp difference between stations. This runs automatically after plotting, or you can trigger it from the menu.

**Menu bar** has Load Stations, Plot Stations, Write Differences, and Exit Application.

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
