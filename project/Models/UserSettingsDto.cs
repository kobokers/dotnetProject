using System.Text.Json;

namespace project.Models;

public class UserSettingsDto
{
    public string Theme { get; set; } = "oled-dark"; // default matches current client side default
    public int FontSize { get; set; } = 14;
    public string AccentColor { get; set; } = "#0066ff";
    public bool CompactView { get; set; } = false;
    public string SidebarPosition { get; set; } = "left";

    public string Language { get; set; } = "en";
    public string DateFormat { get; set; } = "MM/dd/yyyy";
    public string NumberFormat { get; set; } = "comma";

    public bool NotifyPush { get; set; } = true;
    public bool NotifyEmail { get; set; } = false;
    public string NotificationSound { get; set; } = "ding.wav";
    public string DndStart { get; set; } = "22:00";
    public string DndEnd { get; set; } = "07:00";
    public bool BadgeCount { get; set; } = true;

    public bool ProfilePublic { get; set; } = true;
    public bool AdPersonalization { get; set; } = true;

    public bool Newsletter { get; set; } = true;
    public bool MarketingEmail { get; set; } = false;
    public bool SmsAlerts { get; set; } = false;
    public string PreferredContact { get; set; } = "email";
}
