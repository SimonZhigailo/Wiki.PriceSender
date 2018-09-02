using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Service.Models.PriceList.PriceHistory
{
    /// <summary>
    /// История изменения прайсов
    /// </summary>
    public class PriceChangeModelResponse
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
    public class PriceChangeModelRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<SortItem> Sort { get; set; }
    }
    public class SortItem
    {
        public string Field { get; set; }
        public string Dir { get; set; }
    }

}
