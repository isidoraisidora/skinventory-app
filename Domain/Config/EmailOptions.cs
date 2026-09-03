namespace Domain.Config;

public class EmailOptions
{
    public string SmtpHost { get; set; } = "";
    public int SmtpPort { get; set; }
    public string SenderEmail { get; set; } = "";
    public string SenderName { get; set; } = "Skincare Inventory";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}