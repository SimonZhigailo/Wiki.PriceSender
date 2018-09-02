using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Service.Models.PriceList.PriceHistory
{
    /// <summary>
    /// История клиента
    /// </summary>
    public class ClientChangeModelResponse
    {
        public int GroupId { get; set; }
        public string ManagerName { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string Time { get; set; }
        public string DaysSend { get; set; }
        public string TimeSend { get; set; }
        public string ToEmail { get; set; }
        public string FileName { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { get; set; }
        public string FileConfig { get; set; }
        public int total { get; set; }

    }
    /// <summary>
    /// запрос для фильтрации 
    /// </summary>
    public class ClientChangeModelRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<SortItem> Sort { get; set; }
    }
}
