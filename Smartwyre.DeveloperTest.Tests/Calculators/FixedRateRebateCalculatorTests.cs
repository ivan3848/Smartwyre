using Smartwyre.DeveloperTest.Services.Calculators;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Calculators;

public class FixedRateRebateCalculatorTests
{
    private readonly FixedRateRebateCalculator _calculator;

    public FixedRateRebateCalculatorTests()
    {
        _calculator = new FixedRateRebateCalculator();
    }

    [Fact]
    public void SupportedIncentiveType_ReturnsFixedRateRebate()
    {
        Assert.Equal(IncentiveType.FixedRateRebate, _calculator.SupportedIncentiveType);
    }

    [Fact]
    public void CanCalculate_WhenRebateIsNull_ReturnsFalse()
    {
        var product = CreateValidProduct();
        var request = CreateValidRequest();

        var result = _calculator.CanCalculate(null, product, request);

        Assert.False(result);
    }

    [Fact]
    public void CanCalculate_WhenProductIsNull_ReturnsFalse()
    {
        var rebate = CreateValidRebate();
        var request = CreateValidRequest();

        var result = _calculator.CanCalculate(rebate, null, request);

        Assert.False(result);
    }

    [Fact]
    public void CanCalculate_WhenProductDoesNotSupportIncentive_ReturnsFalse()
    {
        var rebate = CreateValidRebate();
        var product = new Product
        {
            Identifier = "PROD001",
            Price = 100m,
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount
        };
        var request = CreateValidRequest();

        var result = _calculator.CanCalculate(rebate, product, request);

        Assert.False(result);
    }

    [Fact]
    public void CanCalculate_WhenRebatePercentageIsZero_ReturnsFalse()
    {
        var rebate = new Rebate
        {
            Identifier = "REB001",
            Incentive = IncentiveType.FixedRateRebate,
            Percentage = 0m,
            Amount = 100m
        };
        var product = CreateValidProduct();
        var request = CreateValidRequest();

        var result = _calculator.CanCalculate(rebate, product, request);

        Assert.False(result);
    }

    [Fact]
    public void CanCalculate_WhenProductPriceIsZero_ReturnsFalse()
    {
        var rebate = CreateValidRebate();
        var product = new Product
        {
            Identifier = "PROD001",
            Price = 0m,
            SupportedIncentives = SupportedIncentiveType.FixedRateRebate
        };
        var request = CreateValidRequest();

        var result = _calculator.CanCalculate(rebate, product, request);

        Assert.False(result);
    }

    [Fact]
    public void CanCalculate_WhenVolumeIsZero_ReturnsFalse()
    {
        var rebate = CreateValidRebate();
        var product = CreateValidProduct();
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "REB001",
            ProductIdentifier = "PROD001",
            Volume = 0m
        };

        var result = _calculator.CanCalculate(rebate, product, request);

        Assert.False(result);
    }

    [Fact]
    public void CanCalculate_WhenAllConditionsAreMet_ReturnsTrue()
    {
        var rebate = CreateValidRebate();
        var product = CreateValidProduct();
        var request = CreateValidRequest();

        var result = _calculator.CanCalculate(rebate, product, request);

        Assert.True(result);
    }

    [Fact]
    public void Calculate_ReturnsCorrectRebateAmount()
    {
        var rebate = new Rebate
        {
            Identifier = "REB001",
            Incentive = IncentiveType.FixedRateRebate,
            Percentage = 0.1m
        };
        var product = new Product
        {
            Identifier = "PROD001",
            Price = 100m,
            SupportedIncentives = SupportedIncentiveType.FixedRateRebate
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "REB001",
            ProductIdentifier = "PROD001",
            Volume = 10m
        };

        var result = _calculator.Calculate(rebate, product, request);

        // Expected: 100 (price) * 0.1 (percentage) * 10 (volume) = 100
        Assert.Equal(100m, result);
    }

    private Rebate CreateValidRebate()
    {
        return new Rebate
        {
            Identifier = "REB001",
            Incentive = IncentiveType.FixedRateRebate,
            Amount = 100m,
            Percentage = 0.15m
        };
    }

    private Product CreateValidProduct()
    {
        return new Product
        {
            Identifier = "PROD001",
            Price = 200m,
            SupportedIncentives = SupportedIncentiveType.FixedRateRebate
        };
    }

    private CalculateRebateRequest CreateValidRequest()
    {
        return new CalculateRebateRequest
        {
            RebateIdentifier = "REB001",
            ProductIdentifier = "PROD001",
            Volume = 5m
        };
    }
}
