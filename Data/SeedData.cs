using SushiInventorySystem.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SushiInventorySystem.Data
{
    internal static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            var branches = LoadBranches(Path.Combine(baseDir, "Data/Branches.txt"));
            var items = LoadItems(Path.Combine(baseDir, "Data/Items.txt"));
            var stocks = LoadStocks(Path.Combine(baseDir, "Data/Stocks.txt"));
            var transfers = LoadTransfers(Path.Combine(baseDir, "Data/Transfer.txt"));

            // Seed only if tables are empty
            if (!context.Branches.Any())
                context.Branches.AddRange(branches);

            if (!context.Items.Any())
                context.Items.AddRange(items);

            if (!context.Stocks.Any())
                context.Stocks.AddRange(stocks);

            if (!context.Transfers.Any())
                context.Transfers.AddRange(transfers);

            context.SaveChanges();
            Console.WriteLine("Seed data successfully loaded into DB.");
        }

        // ===== Entity Loaders =====
        private static List<Branch> LoadBranches(string filePath)
        {
            return File.ReadAllLines(filePath)
                .Select(line => line.Split('|'))
                .Where(p => p.Length >= 5)
                .Select(p => new Branch
                {
                    BranchId = p[0],
                    BranchName = p[1],
                    Address = p[2],
                    Postcode = p[3],
                    Phone = p[4]
                }).ToList();
        }

        private static List<Item> LoadItems(string filePath)
        {
            return File.ReadAllLines(filePath)
                .Select(line => line.Split('|'))
                .Where(p => p.Length >= 8)
                .Select(p => new Item
                {
                    ItemId = p[0],
                    ItemName = p[1],
                    Category = p[2],
                    Unit = p[3],
                    Supplier = p[4],
                    CostPerUnit = decimal.Parse(p[5]),
                    MinStock = int.Parse(p[6]),
                    MaxStock = int.Parse(p[7])
                }).ToList();
        }

        private static List<Stock> LoadStocks(string filePath)
        {
            return File.ReadAllLines(filePath)
                .Select(line => line.Split('|'))
                .Where(p => p.Length >= 4)
                .Select(p => new Stock
                {
                    StockId = p[0],
                    ItemId = p[1],
                    BranchId = p[2],
                    Quantity = int.Parse(p[3])
                }).ToList();
        }

        private static List<Transfer> LoadTransfers(string filePath)
        {
            return File.ReadAllLines(filePath)
                .Select(line => line.Split('|'))
                .Where(p => p.Length >= 7)
                .Select(p => new Transfer
                {
                    TransferId = p[0],
                    ItemId = p[1],
                    Quantity = double.Parse(p[2]),
                    Unit = p[3],
                    FromBranch = p[4],
                    ToBranch = p[5],
                    TransferDate = DateTime.ParseExact(p[6], "yyyy-MM-dd", CultureInfo.InvariantCulture)
                }).ToList();
        }
    }
}
