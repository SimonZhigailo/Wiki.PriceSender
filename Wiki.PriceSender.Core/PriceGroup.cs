using System.Collections.Generic;

namespace Wiki.PriceSender.Dto
{
    public class PriceGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<int> PriceListIds { get; set; }
    }
}