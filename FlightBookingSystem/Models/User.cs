public class User
{
    public int UserId { get; set; }
    public string FName { get; set; } = null!;
    public string LName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int CreatedDay { get; set; }
    public int CreatedMonth { get; set; }
    public int CreatedYear { get; set; }

    // Navigation
    public Admin? Admin { get; set; }
    public RewardAccount? RewardAccount { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}