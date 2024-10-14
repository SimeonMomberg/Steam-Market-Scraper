using HtmlAgilityPack;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace webScraper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Set the license context for EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var urls = new List<(string baseUrl, string rarity)>
            {
                // Blue
                ("https://steamcommunity.com/market/search?q=&category_730_ItemSet%5B%5D=any&category_730_ProPlayer%5B%5D=any&category_730_StickerCapsule%5B%5D=any&category_730_Tournament%5B%5D=any&category_730_TournamentTeam%5B%5D=any&category_730_Type%5B%5D=any&category_730_Weapon%5B%5D=any&category_730_Exterior%5B%5D=tag_WearCategory0&category_730_Quality%5B%5D=tag_normal&category_730_Rarity%5B%5D=tag_Rarity_Rare_Weapon&appid=730", "Blue"),
                // Purple
                ("https://steamcommunity.com/market/search?q=&category_730_ItemSet%5B%5D=any&category_730_ProPlayer%5B%5D=any&category_730_StickerCapsule%5B%5D=any&category_730_Tournament%5B%5D=any&category_730_TournamentTeam%5B%5D=any&category_730_Type%5B%5D=any&category_730_Weapon%5B%5D=any&category_730_Exterior%5B%5D=tag_WearCategory0&category_730_Quality%5B%5D=tag_normal&category_730_Rarity%5B%5D=tag_Rarity_Mythical_Weapon&appid=730", "Purple"),
                // Pink
                ("https://steamcommunity.com/market/search?q=&category_730_ItemSet%5B%5D=any&category_730_ProPlayer%5B%5D=any&category_730_StickerCapsule%5B%5D=any&category_730_Tournament%5B%5D=any&category_730_TournamentTeam%5B%5D=any&category_730_Type%5B%5D=any&category_730_Weapon%5B%5D=any&category_730_Exterior%5B%5D=tag_WearCategory0&category_730_Quality%5B%5D=tag_normal&category_730_Rarity%5B%5D=tag_Rarity_Legendary_Weapon&appid=730", "Pink"),
                // Red
                ("https://steamcommunity.com/market/search?q=&category_730_ItemSet%5B%5D=any&category_730_ProPlayer%5B%5D=any&category_730_StickerCapsule%5B%5D=any&category_730_Tournament%5B%5D=any&category_730_TournamentTeam%5B%5D=any&category_730_Type%5B%5D=any&category_730_Weapon%5B%5D=any&category_730_Exterior%5B%5D=tag_WearCategory0&category_730_Quality%5B%5D=tag_normal&category_730_Rarity%5B%5D=tag_Rarity_Ancient_Weapon&appid=730", "Red")
            };

            var httpClient = new HttpClient();
            var skins = new List<Skin>();

            foreach (var (baseUrl, rarity) in urls)
            {
                Console.WriteLine($"Scraping {rarity} skins...");
                int start = 0;
                const int count = 10; // Results per page
                bool hasMorePages = true;

                while (hasMorePages)
                {
                    // Build the full URL for the current page
                    var url = $"{baseUrl}&start={start}&count={count}";

                    try
                    {
                        var html = await httpClient.GetStringAsync(url);
                        var htmlDocument = new HtmlDocument();
                        htmlDocument.LoadHtml(html);

                        // Get all skins
                        var skinNames = htmlDocument.DocumentNode.SelectNodes("//span[@class='market_listing_item_name']");
                        var skinPrices = htmlDocument.DocumentNode.SelectNodes("//span[@class='normal_price']");

                        if (skinNames != null && skinPrices != null && skinNames.Count > 0)
                        {
                            for (int i = 0; i < skinNames.Count; i++)
                            {
                                var skinName = skinNames[i].InnerText.Trim();
                                var skinPrice = skinPrices[i].InnerText.Trim();
                                skins.Add(new Skin { Name = skinName, Price = skinPrice, Rarity = rarity });
                                Console.WriteLine($"Skin: {skinName} = {skinPrice} | Rarity: {rarity}");
                            }

                            // Move to the next page
                            start += count;
                        }
                        else
                        {
                            Console.WriteLine($"No more skins found for {rarity}, ending scraping.");
                            hasMorePages = false;
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"Request failed: {ex.Message}");

                        // Wait for 2 minutes before retrying
                        Console.WriteLine("Waiting for 2 minutes before retrying...");
                        await Task.Delay(TimeSpan.FromMinutes(2));

                        // Continue from where it left off
                        Console.WriteLine("Retrying now...");
                        continue; // This will retry the current page after the delay
                    }

                    // Optional: Delay between requests to avoid hitting rate limits
                    await Task.Delay(5000);
                }

                // Write to Excel
                WriteToExcel(skins);
            }

            static void WriteToExcel(List<Skin> skins)
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Skins");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Skin Name";
                    worksheet.Cells[1, 2].Value = "Price";
                    worksheet.Cells[1, 3].Value = "Rarity";

                    // Add data
                    for (int i = 0; i < skins.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = skins[i].Name;
                        worksheet.Cells[i + 2, 2].Value = skins[i].Price;
                        worksheet.Cells[i + 2, 3].Value = skins[i].Rarity;
                    }

                    // Save the Excel file
                    var filePath = Path.Combine(Environment.CurrentDirectory, "Skins.xlsx");
                    package.SaveAs(new FileInfo(filePath));
                    Console.WriteLine($"Data has been written to {filePath}");
                }
            }
        }

        class Skin
        {
            public string Name { get; set; }
            public string Price { get; set; }
            public string Rarity { get; set; }
        }
    }
}
