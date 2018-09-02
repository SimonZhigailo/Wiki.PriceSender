﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wiki.PriceSender.Service.Models.PriceList.PriceHistory;

namespace Wiki.PriceSender.Service.Models.PriceList.MainPrice
{
    public class PriceGroupDataResponse
    {
        public int GroupId { get; set; }
        public int PriceListId { get; set; }
        public string Analog { get; set; }
        public int MinOrder { get; set; }
        public int? ErpId { get; set; }
        public string SellerCatalog { get; set; }
        public string SellerNumber { get; set; }
        public string Name { get; set; }
        public int? Qty { get; set; }
        public int total { get; set; }

    }

    public class PriceGroupDataRequest
    {
        public int GroupId { get; set; }
        public string CatalogFilter { get; set; }
        public string NumberFilter { get; set; }
        public string NameFilter { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<SortItem> Sort { get; set; }
    }

}
