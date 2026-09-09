public class EmailSettingDto
{
    public string SmtpHost { get; set; } = "localhost";
    public int SmtpPort { get; set; } = 3000;
    public string FromEmail { get; set; } = "noreply@myjobportal.com";
    public string FromName { get; set; } = "My Job Portal";
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public bool UseSsl { get; set; } = false;

}