namespace FlightBookingSystem.Models;
using System.ComponentModel.DataAnnotations;

public class RewardAccount
{
    [Key]
    public int AccountId { get; set; }
    public int PointsBalance { get; set; }
    public int UserId { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<PointsTransaction> Transactions { get; set; } = new List<PointsTransaction>();
}