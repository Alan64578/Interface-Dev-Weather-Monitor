# Interface Devlopment - Weather Station Data Visualisation

This project is a desktop application built using C# and WPF that reads weather data from two Irish weather stations and displays the data on a graph.

The program loads recorded measurements from text files, converts them into key/value datasets, and plots them so the user can compare the data visually.

This project was developed as part of the Interface Software Development module.

---

## Overview

The program reads weather data from two stations:

- Sherkin Island
- Roches Point

Each station includes recorded measurements such as:

- Temperature
- Rainfall
- Wind Speed
- Time

The application loads the data from text files and displays it using a graph.

---

## Features

- Load weather data from two different stations
- Read and parse text-based datasets
- Store data using SortedDictionary key/value pairs
- Plot weather data on a graph
- Compare data between stations
- Simple graphical interface using WPF

---

## Technologies Used

- C#
- .NET / WPF
- LiveCharts
- Visual Studio
- SortedDictionary data structures

---

## Project Structure

WeatherStationApp  
│  
├── WeatherStationApp.csproj  
├── MainWindow.xaml  
├── MainWindow.xaml.cs  
├── App.xaml  
├── App.xaml.cs  

SherkinWeatherData  
├── Sherkin Temperature.txt  
├── Sherkin Rainfall.txt  
├── Sherkin Speed.txt  
└── Sherkin Time.txt  

RochesWeatherData  
├── Roches Temperature.txt  
├── Roches Rainfall.txt  
├── Roches Speed.txt  
└── Roches Time.txt  

---

## How It Works

1. The program reads weather data from the SherkinWeatherData and RochesWeatherData folders.
2. The data is loaded from text files.
3. Time values are used as the keys and measurements are stored as values.
4. The data is stored in a SortedDictionary to keep the values ordered by time.
5. The selected dataset is plotted using LiveCharts.

---

## Running the Project

1. Clone the repository

git clone https://github.com/yourusername/weather-station-app.git

2. Open the project in Visual Studio.

3. Build and run the project.

The application will automatically load the weather data files if they are located in the correct folders.

---

## Disclaimer

AI tools were used to help format and present this GitHub repository (for example the README file).  
All project code and implementation were written manually by the author.

---

## Author

Alan O’Connell  
Electronic Engineering Student  
Munster Technological University
