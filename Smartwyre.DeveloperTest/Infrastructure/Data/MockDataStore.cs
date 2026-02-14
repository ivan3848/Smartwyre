using System.Collections.Generic;
using Smartwyre.DeveloperTest.Domain.Entities;
using Smartwyre.DeveloperTest.Domain.Enums;

namespace Smartwyre.DeveloperTest.Infrastructure.Data;

/// <summary>
/// Mock data for testing and demonstration purposes
/// In a real application, this would come from a database
/// </summary>
public static class MockDataStore
{
    // Mock Products
    public static readonly Dictionary<string, Product> Products = new()
    {
        ["PROD001"] = new Product
        {
            Id = 1,
            Identifier = "PROD001",
            Price = 100.00m,
            Uom = "kg",
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount | 
                                 SupportedIncentiveType.FixedRateRebate |
                                 SupportedIncentiveType.AmountPerUom
        },
        
        ["PROD002"] = new Product
        {
            Id = 2,
            Identifier = "PROD002",
            Price = 250.00m,
            Uom = "units",
            SupportedIncentives = SupportedIncentiveType.FixedRateRebate
        },
        
        ["PROD003"] = new Product
        {
            Id = 3,
            Identifier = "PROD003",
            Price = 50.00m,
            Uom = "liters",
            SupportedIncentives = SupportedIncentiveType.AmountPerUom
        },
        
        ["PROD004"] = new Product
        {
            Id = 4,
            Identifier = "PROD004",
            Price = 500.00m,
            Uom = "units",
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount
        },
        
        ["LAPTOP-X1"] = new Product
        {
            Id = 5,
            Identifier = "LAPTOP-X1",
            Price = 1200.00m,
            Uom = "units",
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount | 
                                 SupportedIncentiveType.FixedRateRebate
        }
    };

    // Mock Rebates
    public static readonly Dictionary<string, Rebate> Rebates = new()
    {
        ["REB001"] = new Rebate
        {
            Identifier = "REB001",
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 50.00m,
            Percentage = 0m
        },
        
        ["REB002"] = new Rebate
        {
            Identifier = "REB002",
            Incentive = IncentiveType.FixedRateRebate,
            Amount = 0m,
            Percentage = 0.10m  // 10% discount
        },
        
        ["REB003"] = new Rebate
        {
            Identifier = "REB003",
            Incentive = IncentiveType.AmountPerUom,
            Amount = 5.00m,
            Percentage = 0m
        },
        
        ["REB004"] = new Rebate
        {
            Identifier = "REB004",
            Incentive = IncentiveType.FixedRateRebate,
            Amount = 0m,
            Percentage = 0.15m  // 15% discount
        },
        
        ["REB005"] = new Rebate
        {
            Identifier = "REB005",
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 100.00m,
            Percentage = 0m
        },
        
        ["SUMMER2024"] = new Rebate
        {
            Identifier = "SUMMER2024",
            Incentive = IncentiveType.FixedRateRebate,
            Amount = 0m,
            Percentage = 0.20m  // 20% summer discount
        },
        
        ["BULK-DISCOUNT"] = new Rebate
        {
            Identifier = "BULK-DISCOUNT",
            Incentive = IncentiveType.AmountPerUom,
            Amount = 2.50m,
            Percentage = 0m
        }
    };

    // Calculation results storage (simulates database updates)
    public static readonly Dictionary<string, List<CalculationResult>> CalculationHistory = new();

    public class CalculationResult
    {
        public string RebateIdentifier { get; set; }
        public string ProductIdentifier { get; set; }
        public decimal Volume { get; set; }
        public decimal CalculatedAmount { get; set; }
        public System.DateTime CalculatedAt { get; set; }
    }

    /// <summary>
    /// Stores a calculation result in the mock database
    /// </summary>
    public static void StoreCalculation(string rebateIdentifier, string productIdentifier, decimal volume, decimal amount)
    {
        if (!CalculationHistory.ContainsKey(rebateIdentifier))
        {
            CalculationHistory[rebateIdentifier] = new List<CalculationResult>();
        }

        CalculationHistory[rebateIdentifier].Add(new CalculationResult
        {
            RebateIdentifier = rebateIdentifier,
            ProductIdentifier = productIdentifier,
            Volume = volume,
            CalculatedAmount = amount,
            CalculatedAt = System.DateTime.UtcNow
        });
    }

    /// <summary>
    /// Gets calculation history for a rebate
    /// </summary>
    public static List<CalculationResult> GetCalculationHistory(string rebateIdentifier)
    {
        return CalculationHistory.TryGetValue(rebateIdentifier, out var history) 
            ? history 
            : new List<CalculationResult>();
    }

    /// <summary>
    /// Prints all available mock data (for debugging/demo purposes)
    /// </summary>
    public static string GetAvailableData()
    {
        var sb = new System.Text.StringBuilder();
        
        sb.AppendLine("=== Available Products ===");
        foreach (var product in Products.Values)
        {
            sb.AppendLine($"ID: {product.Identifier}");
            sb.AppendLine($"  Price: ${product.Price:F2}");
            sb.AppendLine($"  UOM: {product.Uom}");
            sb.AppendLine($"  Supported Incentives: {product.SupportedIncentives}");
            sb.AppendLine();
        }

        sb.AppendLine("=== Available Rebates ===");
        foreach (var rebate in Rebates.Values)
        {
            sb.AppendLine($"ID: {rebate.Identifier}");
            sb.AppendLine($"  Type: {rebate.Incentive}");
            if (rebate.Amount > 0)
                sb.AppendLine($"  Amount: ${rebate.Amount:F2}");
            if (rebate.Percentage > 0)
                sb.AppendLine($"  Percentage: {rebate.Percentage:P0}");
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
