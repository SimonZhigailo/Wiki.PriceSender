using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wiki.PriceSender.Dto.PriceList;

namespace Wiki.PriceSender.Service.Models.PriceList

{
    //то же самое что и SchedulerItem, без EmailSettings/то что приходит в сервис
    public class PriceSendModel
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
        public EmailSetting EmailSetting { get; set; }
    }


}
