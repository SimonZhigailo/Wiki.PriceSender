using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Service.Models.PriceList.PriceHistory
{
    public class PriceSendErrorModelResponse
    {
        public string Message { get; set; }
        public string ProfileId { get; set; }
        public string EmailTo { get; set; }
        public string ClientId { get; set; }
        public string GroupId { get; set; }
        public DateTime Time { get; set; }
        public int total { get; set; }
    }

    public class PriceSendErrorModelRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<SortItem> Sort { get; set; }
    }
}
