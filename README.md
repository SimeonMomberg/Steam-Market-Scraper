# Steam Market Scraper
This project is a simple web scraper designed to extract skin data (names, prices, and rarity) from the Steam Community Market for Counter-Strike: Global Offensive (CS
) items. The data is collected and stored in an Excel file.

## Features
Scrapes skin names, prices, and rarity from the Steam Market.
Supports multiple rarity categories (e.g., Blue, Purple, Pink, Red).
Handles pagination to gather data from multiple pages.
Handles HTTP request failures by retrying after a 2-minute delay.
Exports the scraped data to an Excel file (Skins.xlsx).

## Prerequisites
.NET Core SDK
EPPlus - for Excel file generation.
HtmlAgilityPack - for HTML parsing.

## NuGet Packages:
Install the necessary packages using the following commands:
dotnet add package HtmlAgilityPack
dotnet add package EPPlus

## How to Run
Clone the repository or download the source code.
git clone https://github.com/yourusername/steam-market-scraper.git
cd steam-market-scraper
Build and run the project:
dotnet build
dotnet run
The scraper will fetch the skins, process multiple pages, and save the output into an Excel file (Skins.xlsx) in the project directory.

## Console will open and you will see this
Scraping Blue skins from [URL]...
Skin: AK-47 | Redline = $22.30 | Rarity: Red
Scraping Purple skins from [URL]...
Skin: M4A1-S | Hyper Beast = $14.50 | Rarity: Purple
If an HTTP request fails, the scraper will wait for 2 minutes before retrying:

## error handeling
Request failed: 500 Internal Server Error
Waiting for 2 minutes before retrying...

## Command-Line Output
Screenshot of the terminal running the scraper
![CMD](https://github.com/user-attachments/assets/73697504-5a18-4e91-8dc3-42b980acd801)


## Excel Output
Screenshot of the generated Skins.xlsx file
![Spred cheet](https://github.com/user-attachments/assets/bc14571d-d93b-49c8-8814-e921564ee99f)


## Excel File Example
The Excel file (Skins.xlsx) will contain the following columns:

Skin Name	Price	Rarity
AK-47	Redline	$22.30
M4A1-S	Hyper Beast	$14.50
Future Improvements
Add more filters to the scraper (e.g., by weapon type or sticker capsule).
Implement rate limiting to avoid being blocked by the server.
Improve error handling and logging.
