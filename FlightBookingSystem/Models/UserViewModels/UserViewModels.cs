using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystem.Models;


// 1. PROFILE (read-only display)

public class ProfileViewModel
{
    public int UserId { get; set; }
    public string FName { get; set; } = null!;
    public string LName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool EmailConfirmed { get; set; }
    public int CreatedDay { get; set; }
    public int CreatedMonth { get; set; }
    public int CreatedYear { get; set; }
    public int? RewardPoints { get; set; }
    public bool IsAdmin { get; set; }

    public string FullName => $"{FName} {LName}".Trim();
    public string JoinedDateText => $"{CreatedDay:00}/{CreatedMonth:00}/{CreatedYear:0000}";
}


// 2. EDIT OWN PROFILE

public class EditProfileViewModel
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    [Display(Name = "First Name")]
    public string FName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
    [Display(Name = "Last Name")]
    public string LName { get; set; } = null!;

    // Read-only on the form — email changes need re-confirmation, out of scope here
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;
}


// 3. CHANGE PASSWORD

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Current password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Current Password")]
    public string CurrentPassword { get; set; } = null!;

    [Required(ErrorMessage = "New password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage = "Please confirm your new password.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm New Password")]
    public string ConfirmNewPassword { get; set; } = null!;
}


// 4. DELETE ACCOUNT (self-service)

public class DeleteAccountViewModel
{
    [Required(ErrorMessage = "Please enter your password to confirm.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;

    [Display(Name = "I understand this action is permanent and cannot be undone.")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must confirm before continuing.")]
    public bool ConfirmDelete { get; set; }

    // For the GET view to surface a soft warning
    public int ActiveBookingCount { get; set; }
}


// 5. MY BOOKINGS

public class MyBookingsViewModel
{
    public List<BookingSummaryViewModel> Bookings { get; set; } = new();
    public int TotalCount => Bookings.Count;
}

public class BookingSummaryViewModel
{
    public int BookingId { get; set; }
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = null!;
    public decimal TotalPrice { get; set; }
    public int PassengerCount { get; set; }
    public int TicketCount { get; set; }
    public string? RouteSummary { get; set; }
}


// 6. DASHBOARD

public class UserDashboardViewModel
{
    public string FName { get; set; } = "";
    public string LName { get; set; } = "";
    public string Email { get; set; } = "";
    public string FullName => $"{FName} {LName}".Trim();
    public string Initials =>
        ((FName.Length > 0 ? FName[0].ToString() : "") +
         (LName.Length > 0 ? LName[0].ToString() : "")).ToUpper();

    public int RewardPoints { get; set; }
    public int FlightsTaken { get; set; }
    public int CitiesVisited { get; set; }
    public List<RecentFlightSummary> RecentFlights { get; set; } = new();
}

public class RecentFlightSummary
{
    public int BookingId { get; set; }
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = "";
    public string DepartureCode { get; set; } = "";
    public string ArrivalCode { get; set; } = "";
    public string FlightNumber { get; set; } = "";
    public string Pnr { get; set; } = "";
}
