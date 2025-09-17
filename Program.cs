using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public record Parcel(string Recipient, string Address, string Town, string Postcode,
    double WeightKg, string PostageClass, double Price);

class Program
{
    static List<Parcel> SentParcels = new();

    static async Task Main()
    {
        var host = CreateHostBuilder().Build();
        _ = host.RunAsync();

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("[1] Send a parcel");
            Console.WriteLine("[2] View sent parcels");
            Console.WriteLine("[3] Exit System");
            Console.Write("Your Choice: ");
            var choice = Console.ReadLine()?.Trim();

            if (choice == "1")
            {
                SendParcel();
            }

            else if (choice == "2")
            {
                await ViewParcels();
            }

            else if (choice == "3")
            {
                break;
            }

            else
            {
                Console.WriteLine("Please choose a valid option (1, 2 or 3)");
            }
        }
    }

    static void SendParcel()
    {
        Console.WriteLine();
        var textInfo = CultureInfo.CurrentCulture.TextInfo;

        Console.Write("Recipient Name: ");
        var recipient = textInfo.ToTitleCase((Console.ReadLine() ?? "").Trim().ToLowerInvariant());

        Console.Write("Address Line 1: ");
        var address1 = textInfo.ToTitleCase((Console.ReadLine() ?? "").Trim().ToLowerInvariant());

        Console.Write("Town/City: ");
        var town = textInfo.ToTitleCase((Console.ReadLine() ?? "").Trim().ToLowerInvariant());

        Console.Write("Postcode: ");
        var postcode = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

        double weightKg = 0;
        while (true)
        {
            Console.Write("Parcel Weight (kg): ");
            var w = Console.ReadLine();
            if (double.TryParse(w, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out weightKg) && weightKg >= 0)
                break;
            Console.WriteLine("Please enter a valid numeric weight (e.g. 2.5).");
        }

        int postageChoice = 2;
        while (true)
        {
            Console.Write("Postage Class (1st/2nd Class): ");
            var postageClass = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

            if (postageClass == "1st" || postageClass == "1" || postageClass == "first")
            {
                postageChoice = 1;
                break;
            }
            else if (postageClass == "2nd" || postageClass == "2" || postageClass == "second")
            {
                postageChoice = 2;
                break;
            }

            Console.WriteLine("Please choose a valid postage class (e.g. 1st, 2nd, first, second).");
        }

        double basePrice = 2.00;
        double perKg = 1.50;
        double multiplier = postageChoice == 1 ? 1.5 : 1.0;
        double price = (basePrice + perKg * weightKg) * multiplier;
        string priceStr = price.ToString("C2", CultureInfo.CreateSpecificCulture("en-GB"));

        string className = postageChoice == 1 ? "1st Class" : "2nd Class";

        var parcel = new Parcel(recipient, address1, town, postcode, weightKg, className, price);
        SentParcels.Insert(0, parcel);

        Console.WriteLine();
        Console.WriteLine("---- Shipment Summary ----");
        Console.WriteLine($"Recipient: {recipient}");
        Console.WriteLine($"Address: {address1}, {town}, {postcode}");
        Console.WriteLine($"Weight: {weightKg} kg");
        Console.WriteLine($"Postage: {className}");
        Console.WriteLine($"Price: {priceStr}");
        Console.WriteLine();
        Console.WriteLine("Processing payment...");
        Thread.Sleep(1000);
        Console.WriteLine("Parcel has been delivered.");
    }

    static async Task ViewParcels()
    {
        using var client = new WebClient();
        try
        {
            var json = await client.DownloadStringTaskAsync("http://localhost:5000/parcels/recent");
            var parcels = JsonSerializer.Deserialize<List<Parcel>>(json) ?? new();
            Console.WriteLine("---- Recent Parcels ----");
            if (parcels.Count == 0)
            {
                Console.WriteLine("No parcels sent yet.");
            }

            else
            {
                foreach (var p in parcels)
                {
                    var roundedPrice = Math.Round(p.Price, 2);
                    Console.WriteLine($"{p.Recipient} | {p.Address}, {p.Town}, {p.Postcode} | {p.PostageClass} | £{roundedPrice:F2}");
                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving parcels: {ex.Message}");
        }
    }

    static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices(s => { });
                webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/parcels/recent", async context =>
                        {
                            var last5 = SentParcels.Take(5).ToList();
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonSerializer.Serialize(last5));
                        });
                    });
                });
            });
}