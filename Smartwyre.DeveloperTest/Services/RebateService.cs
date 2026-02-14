using System;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services.Calculators;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;
    private readonly IRebateCalculatorFactory _calculatorFactory;

    public RebateService(
        IRebateDataStore rebateDataStore,
        IProductDataStore productDataStore,
        IRebateCalculatorFactory calculatorFactory)
    {
        _rebateDataStore = rebateDataStore ?? throw new ArgumentNullException(nameof(rebateDataStore));
        _productDataStore = productDataStore ?? throw new ArgumentNullException(nameof(productDataStore));
        _calculatorFactory = calculatorFactory ?? throw new ArgumentNullException(nameof(calculatorFactory));
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        var rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        var product = _productDataStore.GetProduct(request.ProductIdentifier);

        var result = new CalculateRebateResult();

        if (rebate == null)
        {
            result.Success = false;
            return result;
        }

        try
        {
            var calculator = _calculatorFactory.GetCalculator(rebate.Incentive);

            if (!calculator.CanCalculate(rebate, product, request))
            {
                result.Success = false;
                return result;
            }

            var rebateAmount = calculator.Calculate(rebate, product, request);
            _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);

            result.Success = true;
        }
        catch (NotSupportedException)
        {
            result.Success = false;
        }

        return result;
    }
}
