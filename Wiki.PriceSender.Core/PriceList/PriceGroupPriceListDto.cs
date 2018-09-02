using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Dto.PriceList

{
    /// <summary>
    /// модель прайс листа
    /// </summary>
    public class PriceGroupPriceListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ListOrder { get; set; }
    }
}
