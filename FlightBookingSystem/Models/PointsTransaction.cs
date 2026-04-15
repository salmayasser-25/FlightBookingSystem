using System.ComponentModel.DataAnnotations;

public class PointsTransaction
{
    [Key]
    public int TransId { get; set; }
    public int AccountId { get; set; }
    public int Points { get; set; }
    public string Type { get; set; } = null!;
    public DateTime Date { get; set; }

    // Navigation
    public RewardAccount RewardAccount { get; set; } = null!;
}