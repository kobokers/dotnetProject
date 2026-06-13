using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using project.Models;

namespace project.Services
{
    public class UserSettingsService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        public UserSettingsService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Load settings for a user; if column is null or empty, return defaults.
        public async Task<UserSettingsDto> GetAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(user.UserSettings))
                return new UserSettingsDto();
            try
            {
                return JsonSerializer.Deserialize<UserSettingsDto>(user.UserSettings, _jsonOptions) ?? new UserSettingsDto();
            }
            catch
            {
                // If JSON is malformed, fall back to defaults to avoid breaking the site.
                return new UserSettingsDto();
            }
        }

        // Update specific fields via an action that mutates the Settings DTO.
        public async Task UpdateAsync(ApplicationUser user, Action<UserSettingsDto> mutate)
        {
            var settings = await GetAsync(user);
            mutate(settings);
            user.UserSettings = JsonSerializer.Serialize(settings, _jsonOptions);
            await _userManager.UpdateAsync(user);
        }
    }
}
