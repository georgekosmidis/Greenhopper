using CarbonAware.Aggregators.CarbonAware;

namespace CarbonAware.AzureFunction.Services.Models;

public class EmissionsDataForLocationsParametersDTO : CarbonAwareParametersBaseDTO
{
    /// <summary>String array of named locations</summary>
    /// <example>eastus</example>
    public override string[]? MultipleLocations { get; set; }

    /// <summary>[Optional] Start time for the data query.</summary>
    /// <example>2022-03-01T15:30:00Z</example>
    public override DateTimeOffset? Start { get; set; }

    /// <summary>[Optional] End time for the data query.</summary>
    /// <example>2022-03-01T18:30:00Z</example>
    public override DateTimeOffset? End { get; set; }
}
