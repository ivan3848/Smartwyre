using System;
using Smartwyre.DeveloperTest.Application.DTOs;
using Smartwyre.DeveloperTest.Application.Interfaces;
using Smartwyre.DeveloperTest.Domain.Entities;
using Smartwyre.DeveloperTest.Domain.Enums;

namespace Smartwyre.DeveloperTest.Application.Strategies;

/// <summary>
/// Strategy for calculating Fixed Rate rebates
/// Clean Architecture - Application layer implements business rules
/// </summary>
public class FixedRateRebateStrategy : IRebateCalculationStrategy
{
    public IncentiveType SupportedIncentiveType => IncentiveType.FixedRateRebate;

    public bool CanCalculate(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        if (rebate == null)
            return false;

        if (product == null)
            return false;

        if (!product.SupportedIncentives.HasFlag(Domain.Enums.SupportedIncentiveType.FixedRateRebate))
            return false;

        if (rebate.Percentage == 0 || product.Price == 0 || request.Volume == 0)
            return false;

        return true;
    }

    public decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        return product.Price * rebate.Percentage * request.Volume;
    }
}
