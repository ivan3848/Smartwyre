using System;
using Moq;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Services.Calculators;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Services;

public class RebateServiceTests
{
    private readonly Mock<IRebateDataStore> _mockRebateDataStore;
    private readonly Mock<IProductDataStore> _mockProductDataStore;
    private readonly Mock<IRebateCalculatorFactory> _mockCalculatorFactory;
    private readonly RebateService _service;

    public RebateServiceTests()
    {
        _mockRebateDataStore = new Mock<IRebateDataStore>();
        _mockProductDataStore = new Mock<IProductDataStore>();
        _mockCalculatorFactory = new Mock<IRebateCalculatorFactory>();
        
        _service = new RebateService(
            _mockRebateDataStore.Object,
            _mockProductDataStore.Object,
            _mockCalculatorFactory.Object);
    }

    [Fact]
    public void Constructor_WithNullRebateDataStore_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            new RebateService(null, _mockProductDataStore.Object, _mockCalculatorFactory.Object));
    }

    [Fact]
    public void Constructor_WithNullProductDataStore_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            new RebateService(_mockRebateDataStore.Object, null, _mockCalculatorFactory.Object));
    }

    [Fact]
    public void Constructor_WithNullCalculatorFactory_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            new RebateService(_mockRebateDataStore.Object, _mockProductDataStore.Object, null));
    }

    [Fact]
    public void Calculate_WhenRebateNotFound_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "UNKNOWN",
            ProductIdentifier = "PROD001",
            Volume = 10m
        };

        _mockRebateDataStore.Setup(x => x.GetRebate("UNKNOWN")).Returns((Rebate)null);

        var result = _service.Calculate(request);

        Assert.False(result.Success);
        _mockRebateDataStore.Verify(x => x.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Calculate_WhenCalculatorCannotCalculate_ReturnsFailure()
    {
        var rebate = new Rebate
        {
            Identifier = "REB001",
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 0m
        };
        var product = new Product
        {
            Identifier = "PROD001",
            Price = 100m,
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "REB001",
            ProductIdentifier = "PROD001",
            Volume = 10m
        };

        var mockCalculator = new Mock<IRebateCalculator>();
        mockCalculator.Setup(x => x.SupportedIncentiveType).Returns(IncentiveType.FixedCashAmount);
        mockCalculator.Setup(x => x.CanCalculate(rebate, product, request)).Returns(false);

        _mockRebateDataStore.Setup(x => x.GetRebate("REB001")).Returns(rebate);
        _mockProductDataStore.Setup(x => x.GetProduct("PROD001")).Returns(product);
        _mockCalculatorFactory.Setup(x => x.GetCalculator(IncentiveType.FixedCashAmount))
            .Returns(mockCalculator.Object);

        var result = _service.Calculate(request);

        Assert.False(result.Success);
        _mockRebateDataStore.Verify(x => x.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Calculate_WhenAllConditionsMet_ReturnsSuccessAndStoresResult()
    {
        var rebate = new Rebate
        {
            Identifier = "REB001",
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 50m
        };
        var product = new Product
        {
            Identifier = "PROD001",
            Price = 100m,
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "REB001",
            ProductIdentifier = "PROD001",
            Volume = 10m
        };

        var mockCalculator = new Mock<IRebateCalculator>();
        mockCalculator.Setup(x => x.SupportedIncentiveType).Returns(IncentiveType.FixedCashAmount);
        mockCalculator.Setup(x => x.CanCalculate(rebate, product, request)).Returns(true);
        mockCalculator.Setup(x => x.Calculate(rebate, product, request)).Returns(50m);

        _mockRebateDataStore.Setup(x => x.GetRebate("REB001")).Returns(rebate);
        _mockProductDataStore.Setup(x => x.GetProduct("PROD001")).Returns(product);
        _mockCalculatorFactory.Setup(x => x.GetCalculator(IncentiveType.FixedCashAmount))
            .Returns(mockCalculator.Object);

        var result = _service.Calculate(request);

        Assert.True(result.Success);
        _mockRebateDataStore.Verify(x => x.StoreCalculationResult(rebate, 50m), Times.Once);
    }

    [Fact]
    public void Calculate_WhenCalculatorNotFound_ReturnsFailure()
    {
        var rebate = new Rebate
        {
            Identifier = "REB001",
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 50m
        };
        var product = new Product
        {
            Identifier = "PROD001",
            Price = 100m,
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "REB001",
            ProductIdentifier = "PROD001",
            Volume = 10m
        };

        _mockRebateDataStore.Setup(x => x.GetRebate("REB001")).Returns(rebate);
        _mockProductDataStore.Setup(x => x.GetProduct("PROD001")).Returns(product);
        _mockCalculatorFactory.Setup(x => x.GetCalculator(It.IsAny<IncentiveType>()))
            .Throws<NotSupportedException>();

        var result = _service.Calculate(request);

        Assert.False(result.Success);
        _mockRebateDataStore.Verify(x => x.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Calculate_FixedRateRebate_CalculatesCorrectAmount()
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

        var calculator = new FixedRateRebateCalculator();
        
        _mockRebateDataStore.Setup(x => x.GetRebate("REB001")).Returns(rebate);
        _mockProductDataStore.Setup(x => x.GetProduct("PROD001")).Returns(product);
        _mockCalculatorFactory.Setup(x => x.GetCalculator(IncentiveType.FixedRateRebate))
            .Returns(calculator);

        var result = _service.Calculate(request);

        Assert.True(result.Success);
        // Expected: 100 * 0.1 * 10 = 100
        _mockRebateDataStore.Verify(x => x.StoreCalculationResult(rebate, 100m), Times.Once);
    }

    [Fact]
    public void Calculate_AmountPerUom_CalculatesCorrectAmount()
    {
        var rebate = new Rebate
        {
            Identifier = "REB001",
            Incentive = IncentiveType.AmountPerUom,
            Amount = 5m
        };
        var product = new Product
        {
            Identifier = "PROD001",
            Price = 100m,
            SupportedIncentives = SupportedIncentiveType.AmountPerUom
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "REB001",
            ProductIdentifier = "PROD001",
            Volume = 20m
        };

        var calculator = new AmountPerUomCalculator();
        
        _mockRebateDataStore.Setup(x => x.GetRebate("REB001")).Returns(rebate);
        _mockProductDataStore.Setup(x => x.GetProduct("PROD001")).Returns(product);
        _mockCalculatorFactory.Setup(x => x.GetCalculator(IncentiveType.AmountPerUom))
            .Returns(calculator);

        var result = _service.Calculate(request);

        Assert.True(result.Success);
        // Expected: 5 * 20 = 100
        _mockRebateDataStore.Verify(x => x.StoreCalculationResult(rebate, 100m), Times.Once);
    }
}
