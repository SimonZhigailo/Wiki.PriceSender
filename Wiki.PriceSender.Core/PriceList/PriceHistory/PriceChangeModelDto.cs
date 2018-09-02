using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Dto.PriceList.PriceHistory
{
    /// <summary>
    /// История изменения прайсов
    /// </summary>
    public class PriceChangeModelResponseDto
    {
        public int GroupId { get; set; }
        public string ManagerName { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string Time { get; set; }
        public KeyValuePair<string, string> Value { get; set; }
        public int total { get; set; }
    }
    /// <summary>
    /// запрос для фильтрации 
    /// </summary>
    public class PriceChangeModelRequestDto
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<SortItemDto> Sort { get; set; }
    }
    public class SortItemDto
    {
        public string Field { get; set; }
        public string Dir { get; set; }
    }

}
