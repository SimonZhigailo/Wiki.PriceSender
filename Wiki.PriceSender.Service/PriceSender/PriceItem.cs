using System.Collections.Generic;

namespace Wiki.PriceSender.Service.PriceSender
{
    public class PriceItem
    {
        public string Catalog { get; set; }
        public string Number { get; set; }
        public string Analog { get; set; }
        public string Name{ get; set; }
        public int? Quantity { get; set; }
        public int? MinOrder { get; set; }
        public decimal Price { get; set; }
        public int DeliveryDays { get; set; }
        public string BarCode { get; set; }
    }

    //нужно посмотреть используетс ли локальные прайсы в PriceSender
    //public class LocalPrice
    //{
    //    public string FileName { get; set; }
    //    public string Subject { get; set; }
    //    public List<PriceItem> PriceItems { get; set; }
    //    public byte[] FileContent { get; set; }
    //}
}