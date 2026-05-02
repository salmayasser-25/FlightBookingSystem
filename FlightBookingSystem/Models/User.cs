using FlightBookingSystem.Models;

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

    // ===== AUTH FIELDS =====
    public bool EmailConfirmed { get; set; } = false;
    public string? EmailConfirmToken { get; set; }
    public DateTime? EmailConfirmTokenExpiry { get; set; }

    public string? OtpCode { get; set; }
    public DateTime? OtpExpiry { get; set; }
    public bool OtpVerified { get; set; } = false;

    public string? ResetPasswordToken { get; set; }
    public DateTime? ResetPasswordTokenExpiry { get; set; }

    // Navigation
    public Admin? Admin { get; set; }
    public RewardAccount? RewardAccount { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
