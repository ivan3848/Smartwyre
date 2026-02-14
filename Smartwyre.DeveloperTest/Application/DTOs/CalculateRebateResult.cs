namespace Smartwyre.DeveloperTest.Application.DTOs;

/// <summary>
/// Data Transfer Object for rebate calculation results
/// Part of Application layer - encapsulates operation result
/// </summary>
public class CalculateRebateResult
{
    public bool Success { get; set; }
    public decimal? CalculatedAmount { get; set; }
    public string ErrorMessage { get; set; }
}
