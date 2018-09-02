using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wiki.PriceSender.Dto.PriceList.PriceHistory;

namespace Wiki.PriceSender.Dto.PriceList.PriceHistory
{
    public class PriceSendErrorModelResponseDto
    {
        public string Message { get; set; }
        public string ProfileId { get; set; }
        public string EmailTo { get; set; }
        public string ClientId { get; set; }
        public string GroupId { get; set; }
        public int total { get; set; }
    }

    public class PriceSendErrorModelRequestDto
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<SortItemDto> Sort { get; set; }
    }
}
