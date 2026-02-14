using Microsoft.Extensions.DependencyInjection;
using Smartwyre.DeveloperTest.Application.DTOs;
using Smartwyre.DeveloperTest.Application.Interfaces;
using Smartwyre.DeveloperTest.Application.Strategies;
using Smartwyre.DeveloperTest.Application.UseCases;
using Smartwyre.DeveloperTest.Domain.Interfaces;
using Smartwyre.DeveloperTest.Infrastructure.Data;
using Smartwyre.DeveloperTest.Infrastructure.Persistence;

namespace Smartwyre.DeveloperTest.Runner;

/// <summary>
/// Presentation Layer - Console Application
/// Clean Architecture: This is the outermost layer that depends on all inner layers
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        var serviceProvider = ConfigureServices();
        var calculateRebateUseCase = serviceProvider.GetRequiredService<ICalculateRebateUseCase>();

        System.Console.WriteLine("╔════════════════════════════════════════╗");
        System.Console.WriteLine("║  Smartwyre Rebate Calculator           ║");
        System.Console.WriteLine("║  Clean Architecture Implementation     ║");
        System.Console.WriteLine("╚════════════════════════════════════════╝");
        System.Console.WriteLine();

        string rebateIdentifier, productIdentifier;
        decimal volume;

        // Check if command line arguments are provided
        if (args.Length >= 3)
        {
            rebateIdentifier = args[0];
            productIdentifier = args[1];

            if (!decimal.TryParse(args[2], out volume))
            {
                System.Console.WriteLine("❌ Error: Volume must be a valid decimal number.");
                return;
            }

            System.Console.WriteLine("📋 Using command line arguments:");
        }
        else if (args.Length == 1 && (args[0] == "--help" || args[0] == "-h"))
        {
            ShowHelp();
            return;
        }
        else if (args.Length == 1 && (args[0] == "--list" || args[0] == "-l"))
        {
            ShowAvailableData();
            return;
        }
        else
        {
            // Show tip
            System.Console.WriteLine("💡 Tip: Use --list to see available products and rebates");
            System.Console.WriteLine();

            // Prompt user for inputs
            System.Console.Write("Enter Rebate Identifier (e.g., REB001, SUMMER2024): ");
            rebateIdentifier = System.Console.ReadLine() ?? string.Empty;

            System.Console.Write("Enter Product Identifier (e.g., PROD001, LAPTOP-X1): ");
            productIdentifier = System.Console.ReadLine() ?? string.Empty;

            System.Console.Write("Enter Volume: ");
            if (!decimal.TryParse(System.Console.ReadLine(), out volume))
            {
                System.Console.WriteLine("❌ Error: Invalid volume entered.");
                return;
            }
        }

        System.Console.WriteLine();
        System.Console.WriteLine("───────────────────────────────────────");
        System.Console.WriteLine($"🔖 Rebate ID: {rebateIdentifier}");
        System.Console.WriteLine($"📦 Product ID: {productIdentifier}");
        System.Console.WriteLine($"📊 Volume: {volume}");
        System.Console.WriteLine("───────────────────────────────────────");
        System.Console.WriteLine();

        var request = new CalculateRebateRequest
        {
            RebateIdentifier = rebateIdentifier,
            ProductIdentifier = productIdentifier,
            Volume = volume
        };

        System.Console.WriteLine("⚙️  Calculating rebate...");
        var result = calculateRebateUseCase.Execute(request);

        System.Console.WriteLine();
        System.Console.WriteLine("═══════════════════════════════════════");
        if (result.Success)
        {
            System.Console.WriteLine("✅ Rebate calculation SUCCEEDED!");
            System.Console.WriteLine();
            System.Console.WriteLine($"💰 Calculated Rebate Amount: ${result.CalculatedAmount:F2}");
            System.Console.WriteLine($"📊 Calculation Details:");
            System.Console.WriteLine($"   - Rebate: {rebateIdentifier}");
            System.Console.WriteLine($"   - Product: {productIdentifier}");
            System.Console.WriteLine($"   - Volume: {volume}");
            System.Console.WriteLine();
            System.Console.WriteLine("💾 Result has been saved to the database.");
        }
        else
        {
            System.Console.WriteLine("❌ Rebate calculation FAILED");
            System.Console.WriteLine();
            System.Console.WriteLine($"📝 Error Details:");
            System.Console.WriteLine($"   {result.ErrorMessage}");
            System.Console.WriteLine();
            System.Console.WriteLine("💡 Suggestions:");
            System.Console.WriteLine("   - Use --list to see valid products and rebates");
            System.Console.WriteLine("   - Ensure the product supports the rebate incentive type");
            System.Console.WriteLine("   - Check that all values are greater than zero");
        }
        System.Console.WriteLine("═══════════════════════════════════════");

        System.Console.WriteLine();
        System.Console.WriteLine("Press any key to exit...");

        if (System.Console.IsInputRedirected == false)
        {
            System.Console.ReadKey();
        }
    }

    private static void ShowHelp()
    {
        System.Console.WriteLine("Usage:");
        System.Console.WriteLine("  dotnet run [options]");
        System.Console.WriteLine("  dotnet run <rebate-id> <product-id> <volume>");
        System.Console.WriteLine();
        System.Console.WriteLine("Options:");
        System.Console.WriteLine("  --help, -h     Show this help message");
        System.Console.WriteLine("  --list, -l     List available products and rebates");
        System.Console.WriteLine();
        System.Console.WriteLine("Examples:");
        System.Console.WriteLine("  dotnet run REB001 PROD001 10");
        System.Console.WriteLine("  dotnet run SUMMER2024 LAPTOP-X1 5");
        System.Console.WriteLine("  dotnet run --list");
    }

    private static void ShowAvailableData()
    {
        System.Console.WriteLine("╔════════════════════════════════════════╗");
        System.Console.WriteLine("║      Available Test Data               ║");
        System.Console.WriteLine("╚════════════════════════════════════════╝");
        System.Console.WriteLine();

        System.Console.WriteLine(MockDataStore.GetAvailableData());

        System.Console.WriteLine("═══════════════════════════════════════");
        System.Console.WriteLine("Example Commands:");
        System.Console.WriteLine("═══════════════════════════════════════");
        System.Console.WriteLine();
        System.Console.WriteLine("💰 Fixed Cash Amount:");
        System.Console.WriteLine("   dotnet run REB001 PROD001 10");
        System.Console.WriteLine("   Expected: $50.00 (fixed amount)");
        System.Console.WriteLine();
        System.Console.WriteLine("📊 Fixed Rate Rebate:");
        System.Console.WriteLine("   dotnet run REB002 PROD001 10");
        System.Console.WriteLine("   Expected: $100.00 (10% of $100 × 10 units)");
        System.Console.WriteLine();
        System.Console.WriteLine("📦 Amount Per UOM:");
        System.Console.WriteLine("   dotnet run REB003 PROD003 20");
        System.Console.WriteLine("   Expected: $100.00 ($5 × 20 units)");
        System.Console.WriteLine();
        System.Console.WriteLine("🎁 Summer Discount:");
        System.Console.WriteLine("   dotnet run SUMMER2024 LAPTOP-X1 2");
        System.Console.WriteLine("   Expected: $480.00 (20% of $1200 × 2 units)");
        System.Console.WriteLine();
    }

    /// <summary>
    /// Dependency Injection Configuration
    /// Clean Architecture: All dependencies are registered here
    /// Dependencies flow inward: Presentation → Application → Domain
    /// </summary>
    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Domain Layer - No dependencies (core business logic)
        // Domain entities and interfaces are referenced but not registered as they're POCOs

        // Infrastructure Layer - Implements domain interfaces
        services.AddSingleton<IRebateRepository, RebateRepository>();
        services.AddSingleton<IProductRepository, ProductRepository>();

        // Application Layer - Business logic and use cases
        services.AddSingleton<IRebateCalculationStrategy, FixedCashAmountStrategy>();
        services.AddSingleton<IRebateCalculationStrategy, FixedRateRebateStrategy>();
        services.AddSingleton<IRebateCalculationStrategy, AmountPerUomStrategy>();
        services.AddSingleton<IRebateCalculationStrategyFactory, RebateCalculationStrategyFactory>();

        // Use Cases
        services.AddSingleton<ICalculateRebateUseCase, CalculateRebateUseCase>();

        return services.BuildServiceProvider();
    }
}
