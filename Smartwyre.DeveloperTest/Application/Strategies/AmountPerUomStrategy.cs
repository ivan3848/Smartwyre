using Smartwyre.DeveloperTest.Application.DTOs;
using Smartwyre.DeveloperTest.Application.Interfaces;
using Smartwyre.DeveloperTest.Domain.Entities;
using Smartwyre.DeveloperTest.Domain.Enums;

namespace Smartwyre.DeveloperTest.Application.Strategies;

/// <summary>
/// Strategy for calculating Amount Per Unit of Measure rebates
/// Clean Architecture - Application layer implements business rules
/// </summary>
public class AmountPerUomStrategy : IRebateCalculationStrategy
{
    public IncentiveType SupportedIncentiveType => IncentiveType.AmountPerUom;

    public bool CanCalculate(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        if (rebate == null)
            return false;

        if (product == null)
            return false;

        if (product.SupportedIncentives != Domain.Enums.SupportedIncentiveType.AmountPerUom)
            return false;

        if (rebate.Amount == 0 || request.Volume == 0)
            return false;

        return true;
    }

    public decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        return rebate.Amount * request.Volume;
    }
}
