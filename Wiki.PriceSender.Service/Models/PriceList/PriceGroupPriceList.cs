using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Service.Models.PriceList

{
    /// <summary>
    /// модель прайс листа
    /// </summary>
    public class PriceGroupPriceList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ListOrder { get; set; }
    }
}
