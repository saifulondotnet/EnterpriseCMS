namespace EnterpriseCMS.Web.Models;

public class NotificationModel
{
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Icon { get; set; } = "bi-info-circle";
}
