namespace LibraryService.EmailConfiguration
{
    public interface IEmailConfiguration
    {
        string Host { get; }
        int Port { get; }
        string SenderName { get; set; }
        string SenderEmail { get; set; }
        string Password { get; set; }
    }
}
