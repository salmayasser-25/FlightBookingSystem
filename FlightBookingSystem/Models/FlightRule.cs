public class FlightRule
{
    public int FlightId { get; set; }
    public int RuleId { get; set; }

    // Navigation
    public Flight Flight { get; set; } = null!;
    public PriceRule PriceRule { get; set; } = null!;
}