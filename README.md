# Mini Parcel API

A small demonstration project combining a **C# console application** with a lightweight **REST API**. It simulates sending parcels via terminal prompts and lets you view the last five parcels via an endpoint.

## Features
- Interactive console menu with three options: Send a Parcel, View Sent Parcels, and Exit.
- REST endpoint at `http://localhost:5000/parcels/recent` returning the five most recent parcels in JSON format.
- Title‑case input formatting for names and addresses, uppercase postcodes.
- Simple postage pricing algorithm (1st Class vs 2nd Class stamps, with per kilogram costs).
- Stores sent parcels in memory and displays them as formatted output.
- Runs as a single self‑contained .NET application with an embedded Kestrel server.

## Tech Stack
- C# 11
- .NET 8.0
- ASP.NET 8.0 Core Hosting
- System.Text.Json 8.0

## Getting Started

### 1. Clone or download the repository
```bash
git clone https://github.com/yourusername/mini-parcel-api.git
cd mini-parcel-api
```
### 2. Build and run the app
```bash
dotnet restore
dotnet run
```
This will restore dependencies and run the combined console & API application.

### 3. Use the interactive menu
When the system runs, the following menu will be displayed:
```text
What would you like to do?
[1] Send a parcel
[2] View sent parcels
[3] Exit System
Your Choice:
```
Enter 1, 2 or 3 and press ```[ENTER]``` to choose an option.

## Using The App

### 1. Send A Parcel
On choosing ```[1]```, prompts for the parcel details will be displayed:
```bash
Recipient Name:
Address Line 1:
Town/City:
Postcode:
Weight (kg):
Postage Class (1st/2nd Class):
```
Enter the details of the parcel and press ```[ENTER]``` to confirm them. Once all details have been entered, the shipping cost will be calculated, and the parcel sent to the REST endpoint with a confirmation message.

### 2. View Sent Parcels
On choosing ```[2]```, the system will retrieve up to 5 of the most recent parcels from the REST endpoint, and display their details. The endpoint can also be accessed directly:
```bash
curl http://localhost:5000/parcels/recent
```

### 3. Exit The Application
To close the application, either select ```[3]``` from the main menu, or press ```[CTRL + C]```.

---
