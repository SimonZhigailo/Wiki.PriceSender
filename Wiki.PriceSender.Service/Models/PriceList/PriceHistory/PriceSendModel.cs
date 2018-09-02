using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Service.Models.PriceList.PriceHistory
{
    public class PriceSendModelResponse
    {
        public string GroupId { get; set; }
        public string SendTime { get; set; }
        public string NextTime { get; set; }
        public string EmailTo { get; set; }
        public string EmailFrom { get; set; }
        public string ProfileId { get; set; }
        public string ClientId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int total { get; set; }
    }

    public class PriceSendModelRequest
    {
            public int Take { get; set; }
            public int Skip { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public List<SortItem> Sort { get; set; }
    }

}
