using System;

namespace Wiki.PriceSender.Dto
{
    public class MailOrderSchedulerItem
    {

        public int Id { get; set; }
        public DateTime? LastTimeUpload { get; set; }
        public bool IsStandartSub { get; set; }
        public int SellerId { get; set; }
        public string MailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string DayOfWeekUpload { get; set; }
        public string TimeUpload { get; set; }
        public int MinTotal { get; set; }
        public int TemplateId { get; set; }
        public bool UseAllPrices { get; set; }
        public string SellerName { get; set; }
        public int ClientCode { get; set; }
        public string CompanyName { get; set; }

        public int[] PriceListIds { get; set; }

        public int ProfileId { get; set; }
        public bool AddSku { get; set; }

    }
}