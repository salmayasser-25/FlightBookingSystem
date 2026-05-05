using FlightBookingSystem.Models;

namespace FlightBookingSystem.Services
{
    public interface IUserService
    {
        Task<ProfileViewModel?> GetProfileAsync(int userId);

        Task<EditProfileViewModel?> GetEditModelAsync(int userId);

        Task<bool> UpdateProfileAsync(int userId, EditProfileViewModel model);

        Task<(bool Success, string? Error)> ChangePasswordAsync(int userId, ChangePasswordViewModel model);

        Task<int> CountActiveBookingsAsync(int userId);

        Task<(bool Success, string? Error, int ActiveBookingCount)> DeleteAccountAsync(int userId, string passwordPlain);

        Task<MyBookingsViewModel> GetBookingsAsync(int userId);

        Task<UserDashboardViewModel?> GetDashboardAsync(int userId);
    }
}
