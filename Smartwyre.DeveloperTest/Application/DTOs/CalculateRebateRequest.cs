namespace Smartwyre.DeveloperTest.Application.DTOs;

/// <summary>
/// Data Transfer Object for rebate calculation requests
/// Part of Application layer - used to communicate between layers
/// </summary>
public class CalculateRebateRequest
{
    public string RebateIdentifier { get; set; }
    public string ProductIdentifier { get; set; }
    public decimal Volume { get; set; }
}
