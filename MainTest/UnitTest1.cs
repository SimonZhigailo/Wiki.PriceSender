using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wiki.PriceSender.Service;
using Wiki.PriceSender.Service.Models;
using Wiki.PriceSender.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace MainTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetPrices()
        {
            if (Environment.MachineName != "DESINER")
                return;
            //PriceListsClient cl = PriceListsClient.CreateClient();

            //SortItem item = new SortItem
            //{
            //    Dir = "asc",
            //    Field = "PriceListId"
            //};
            ////var request = new ClientChangeModelRequest
            ////{
            ////    PageSize = 100,
            ////    Skip = 0,
            ////    Take = 100,
            ////    Page = 1,
            ////    Sort = new List<SortItem>()
            ////};


            ////var request = new PriceGroupDataRequest
            ////{
            ////    GroupId = 1,
            ////    PageSize = 100,
            ////    Skip = 0,
            ////    Take = 100,
            ////    Page = 1,
            ////    Sort = new List<SortItem>()
            ////};

            ////request.Sort.Add(item);

            ////var c = cl.GetPriceGroupData(request).Result;

            ////var с = cl.GetPricesHistory(request).Result;
            ////var с = cl.GetClientsHistory(request).Result;

            ////var c = cl.InsertCatalog(10, 72).Result;

            ////var d = cl.GetGroups().Result;

            //var c = cl.GetExcelPriceGroup(1);

            //File.WriteAllBytes(@"C:\\Temp\\PriceGroup.xls", c.Result);

            //return;

            //var cl = new PriceListsClient("http://127.0.0.1:9100", "service", "servicepasswd");

            //var result = cl.GetGroups().Result;

            //bool x = cl.SendPrice(378).Result;
        }
    }
}
