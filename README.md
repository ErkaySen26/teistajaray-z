# Player Management System - Windows Forms Application

## Project Description
This project is a **Windows Forms Application** developed for managing player data. It allows users to input, store, and manipulate player information such as name, surname, height, weight, attack, and defense statistics. The application uses a simple user interface to add, update, and save player data. Additionally, it supports importing and exporting player data in JSON format.

## Features
- **Add/Update Players:** Input and modify player details including personal information and statistics (attack, defense).
- **Display Players:** Display a list of all players in a data grid.
- **Search Players:** Search for players by name and surname.
- **Save Data:** Save player data to a JSON file.
- **Import Data:** Import player data from a JSON file.
- **Input Validation:** Ensures that player data (name, surname, height, and weight) is valid before saving.

## Technologies Used
- **C#** (Programming language)
- **Windows Forms** (UI framework)
- **JSON** (For saving and loading data)
- **Newtonsoft.Json** (For JSON serialization and deserialization)

## Installation

### Prerequisites
Ensure you have the following installed:
- **Visual Studio** (or any C# compatible IDE)
- **.NET Framework** (for building and running Windows Forms applications)

### Steps to run the project

1. **Clone the repository:**
git clone https://github.com/ErkaySen26/teistajaray-z.git

2. **Open the project in Visual Studio** (or your preferred IDE).

3. **Build and run the project** in Visual Studio:
- Press `F5` or use the **Run** button to execute the program.

4. **Use the application** to add, view, and manage players.

## Usage
- **Adding a Player:**
- Enter the player's name, surname, height, weight, attack, and defense values.
- Click **Save** to store the player in the system.

- **Displaying Player List:**
- The data grid will display a list of all players with their details.

- **Saving Data:**
- Save the data to a JSON file using the **Save** button.

- **Importing Data:**
- Import player data from a JSON file by selecting **Import** in the menu and choosing the file.

- **Search Players:**
- Use the search box to filter players by name or surname.

## Example Data Model (JSON Format)

```json
{
"SavingDate": "29.10.2023",
"Players": [
 {
   "Name": "John",
   "Surname": "Doe",
   "Height": 180,
   "Weight": 75,
   "Attack": 50,
   "Defense": 40
 }
]
}
Contributing
If you would like to contribute to this project, feel free to fork the repository and submit a pull request with your improvements.
License
This project is licensed under the MIT License.

