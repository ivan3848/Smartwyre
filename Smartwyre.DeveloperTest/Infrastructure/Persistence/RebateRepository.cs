using Smartwyre.DeveloperTest.Domain.Entities;
using Smartwyre.DeveloperTest.Domain.Interfaces;
using Smartwyre.DeveloperTest.Infrastructure.Data;

namespace Smartwyre.DeveloperTest.Infrastructure.Persistence;

/// <summary>
/// Repository implementation for Rebate persistence
/// Clean Architecture - Infrastructure layer implements domain interfaces
/// Uses mock data for demonstration - in production would connect to actual database
/// </summary>
public class RebateRepository : IRebateRepository
{
    public Rebate GetById(string rebateIdentifier)
    {
        // Get rebate from mock data store
        if (MockDataStore.Rebates.TryGetValue(rebateIdentifier, out var rebate))
        {
            return rebate;
        }

        // Return null if rebate not found
        return null;
    }

    public void SaveCalculationResult(Rebate rebate, decimal rebateAmount)
    {
        // Store calculation result in mock data store
        // In production, this would update the database
        System.Console.WriteLine($"💾 Saving calculation result: Rebate={rebate.Identifier}, Amount=${rebateAmount:F2}");

        // Note: In a real implementation, this would update the rebate record
        // For now, we just log the operation
    }
}
