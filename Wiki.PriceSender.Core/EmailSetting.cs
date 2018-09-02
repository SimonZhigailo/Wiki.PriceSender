namespace Wiki.PriceSender.Dto
{
    public class EmailSetting
    {
        public int Id { get; set; }
        private const string EmailSettingKey = "EmailSetting";

        public string SmtpServer { get; set; }

        public int SmtpPort { get; set; }

        public bool UseSsl { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }



    }
}