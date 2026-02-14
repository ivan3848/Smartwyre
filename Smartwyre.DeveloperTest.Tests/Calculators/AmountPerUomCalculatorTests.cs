using Smartwyre.DeveloperTest.Services.Calculators;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Calculators;

public class AmountPerUomCalculatorTests
{
    private readonly AmountPerUomCalculator _calculator;

    public AmountPerUomCalculatorTests()
    {
        _calculator = new AmountPerUomCalculator();
    }

    [Fact]
    public void SupportedIncentiveType_ReturnsAmountPerUom()
    {
        Assert.Equal(IncentiveType.AmountPerUom, _calculator.SupportedIncentiveType);
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
    public void CanCalculate_WhenRebateAmountIsZero_ReturnsFalse()
    {
        var rebate = new Rebate
        {
            Identifier = "REB001",
            Incentive = IncentiveType.AmountPerUom,
            Amount = 0m
        };
        var product = CreateValidProduct();
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
            Incentive = IncentiveType.AmountPerUom,
            Amount = 5m
        };
        var product = CreateValidProduct();
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "REB001",
            ProductIdentifier = "PROD001",
            Volume = 20m
        };

        var result = _calculator.Calculate(rebate, product, request);

        // Expected: 5 (amount) * 20 (volume) = 100
        Assert.Equal(100m, result);
    }

    private Rebate CreateValidRebate()
    {
        return new Rebate
        {
            Identifier = "REB001",
            Incentive = IncentiveType.AmountPerUom,
            Amount = 10m,
            Percentage = 0m
        };
    }

    private Product CreateValidProduct()
    {
        return new Product
        {
            Identifier = "PROD001",
            Price = 100m,
            Uom = "kg",
            SupportedIncentives = SupportedIncentiveType.AmountPerUom
        };
    }

    private CalculateRebateRequest CreateValidRequest()
    {
        return new CalculateRebateRequest
        {
            RebateIdentifier = "REB001",
            ProductIdentifier = "PROD001",
            Volume = 15m
        };
    }
}
