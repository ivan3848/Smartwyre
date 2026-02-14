using System;
using Smartwyre.DeveloperTest.Application.DTOs;
using Smartwyre.DeveloperTest.Application.Interfaces;
using Smartwyre.DeveloperTest.Domain.Interfaces;

namespace Smartwyre.DeveloperTest.Application.UseCases;

/// <summary>
/// Use Case: Calculate Rebate
/// Implements the core business logic for calculating rebates
/// Clean Architecture - Application layer orchestrates domain logic
/// </summary>
public class CalculateRebateUseCase : ICalculateRebateUseCase
{
    private readonly IRebateRepository _rebateRepository;
    private readonly IProductRepository _productRepository;
    private readonly IRebateCalculationStrategyFactory _strategyFactory;

    public CalculateRebateUseCase(
        IRebateRepository rebateRepository,
        IProductRepository productRepository,
        IRebateCalculationStrategyFactory strategyFactory)
    {
        _rebateRepository = rebateRepository ?? throw new ArgumentNullException(nameof(rebateRepository));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _strategyFactory = strategyFactory ?? throw new ArgumentNullException(nameof(strategyFactory));
    }

    public CalculateRebateResult Execute(CalculateRebateRequest request)
    {
        // Step 1: Retrieve domain entities
        var rebate = _rebateRepository.GetById(request.RebateIdentifier);
        if (rebate == null)
        {
            return new CalculateRebateResult 
            { 
                Success = false,
                ErrorMessage = $"Rebate '{request.RebateIdentifier}' not found."
            };
        }

        var product = _productRepository.GetById(request.ProductIdentifier);
        if (product == null)
        {
            return new CalculateRebateResult 
            { 
                Success = false,
                ErrorMessage = $"Product '{request.ProductIdentifier}' not found."
            };
        }

        try
        {
            // Step 2: Get the appropriate calculation strategy
            var strategy = _strategyFactory.GetStrategy(rebate.Incentive);

            // Step 3: Validate if calculation can be performed
            if (!strategy.CanCalculate(rebate, product, request))
            {
                return new CalculateRebateResult 
                { 
                    Success = false,
                    ErrorMessage = "Rebate calculation validation failed. Check rebate and product compatibility."
                };
            }

            // Step 4: Perform the calculation
            var calculatedAmount = strategy.Calculate(rebate, product, request);

            // Step 5: Persist the result
            _rebateRepository.SaveCalculationResult(rebate, calculatedAmount);

            // Step 6: Return success result
            return new CalculateRebateResult
            {
                Success = true,
                CalculatedAmount = calculatedAmount
            };
        }
        catch (NotSupportedException ex)
        {
            return new CalculateRebateResult 
            { 
                Success = false,
                ErrorMessage = $"Incentive type not supported: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new CalculateRebateResult 
            { 
                Success = false,
                ErrorMessage = $"An error occurred: {ex.Message}"
            };
        }
    }
}
