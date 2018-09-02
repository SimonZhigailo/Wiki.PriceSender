using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Dto.PriceList

{
    /// <summary>
    /// Модель клиента
    /// </summary>
    public class PriceGroupToClientDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string ManagerName { get; set; }
        public int ClientId { get; set; }
        public int ClientCode { get; set; }
        public string ClientName { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public char[] DaysSend { get; set; }
        public string[] TimeSend { get; set; }
        public string FileName { get; set; }
        public bool IsEnabled { get; set; }
        public string MailSubject { get; set; }
        public int ProfileId { get; set; }
        public string FileType { get; set; }
        public string MailBody { get; set; }
        public string FileConfig { get; set; }

    }

}
