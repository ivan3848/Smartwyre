using System;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.Calculators;

public class AmountPerUomCalculator : IRebateCalculator
{
    public IncentiveType SupportedIncentiveType => IncentiveType.AmountPerUom;

    public bool CanCalculate(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        if (rebate == null)
            return false;

        if (product == null)
            return false;

        if (!product.SupportedIncentives.HasFlag(Types.SupportedIncentiveType.AmountPerUom))
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
