using System.ComponentModel.DataAnnotations;

public class PriceRule
{
    [Key]
    public int RuleId { get; set; }
    public string ConditionType { get; set; } = null!;
    public decimal Multiplier { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Navigation
    public ICollection<FlightRule> FlightRules { get; set; } = new List<FlightRule>();
}