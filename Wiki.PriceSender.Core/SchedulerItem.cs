using System;

namespace Wiki.PriceSender.Dto
{
    public class SchedulerItem
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int ClientId { get; set; }
        public string Email { get; set; }
        public string DaysSend { get; set; }
        public string TimesSend { get; set; }
        public string FileName { get; set; }
        public string FileConfig { get; set; }
        public string FileType { get; set; }
        public bool IsEnabled { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int ProfileId { get; set; }

        public DateTime NextSend { get; set; }
        public DateTime LastSend { get; set; }

        //public EmailSetting EmailSetting { get; set; }
    }
}