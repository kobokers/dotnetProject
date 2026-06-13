namespace project.Models;

public class SettingsViewModel
{
    public ApplicationUser User { get; set; } = null!;
    public UserSettingsDto Settings { get; set; } = new();
}