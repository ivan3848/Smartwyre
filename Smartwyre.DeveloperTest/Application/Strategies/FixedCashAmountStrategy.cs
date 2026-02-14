using System;
using Smartwyre.DeveloperTest.Application.DTOs;
using Smartwyre.DeveloperTest.Application.Interfaces;
using Smartwyre.DeveloperTest.Domain.Entities;
using Smartwyre.DeveloperTest.Domain.Enums;

namespace Smartwyre.DeveloperTest.Application.Strategies;

/// <summary>
/// Strategy for calculating Fixed Cash Amount rebates
/// Clean Architecture - Application layer implements business rules
/// </summary>
public class FixedCashAmountStrategy : IRebateCalculationStrategy
{
    public IncentiveType SupportedIncentiveType => IncentiveType.FixedCashAmount;

    public bool CanCalculate(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        if (rebate == null)
            return false;

        if (product == null)
            return false;

        if (product.SupportedIncentives != Domain.Enums.SupportedIncentiveType.FixedCashAmount)
            return false;

        if (rebate.Amount == 0)
            return false;

        return true;
    }

    public decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        return rebate.Amount;
    }
}
