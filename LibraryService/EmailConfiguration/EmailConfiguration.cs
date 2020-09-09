namespace LibraryService.EmailConfiguration
{
    public class EmailConfiguration : IEmailConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Password { get; set; }
    }
}
