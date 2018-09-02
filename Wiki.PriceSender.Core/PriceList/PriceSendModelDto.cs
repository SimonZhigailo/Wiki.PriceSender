using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Dto.PriceList

{
    public class PriceSendModelDto
    {
        public int GroupId { get; set; }
        public int ClientId { get; set; }
        public string Email { get; set; }
        public string DaysSend { get; set; }
        public string TimesSend { get; set; }
        public string FileName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FileType { get; set; }
        public string FileConfig { get; set; }
        public bool IsEnabled { get; set; }
        public int ProfileId { get; set; }
        public string EmailSetting { get; set; }
    }

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
