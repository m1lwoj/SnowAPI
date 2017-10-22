namespace EmailSender
{
    public class EmailConfigurationModel
    {
        public string SMTPhost { get; set; }
        public int SMTPport { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
    }
}
