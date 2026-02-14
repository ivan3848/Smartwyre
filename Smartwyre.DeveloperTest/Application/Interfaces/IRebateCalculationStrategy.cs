using Smartwyre.DeveloperTest.Application.DTOs;
using Smartwyre.DeveloperTest.Domain.Entities;
using Smartwyre.DeveloperTest.Domain.Enums;

namespace Smartwyre.DeveloperTest.Application.Interfaces;

/// <summary>
/// Strategy interface for calculating different types of rebates
/// Part of Application layer - defines calculation strategy contract
/// </summary>
public interface IRebateCalculationStrategy
{
    IncentiveType SupportedIncentiveType { get; }
    bool CanCalculate(Rebate rebate, Product product, CalculateRebateRequest request);
    decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request);
}
