using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wiki.PriceSender.Service;
using Wiki.PriceSender.Service.Models;
using Wiki.PriceSender.Service.PriceSender;

namespace MainTest
{
    [TestClass]
    public class GeneratePriceTestLoad
    {

        [TestMethod]
        [TestCategory("local")]
        public void GetPrices()
        {
            if(Environment.MachineName!="VSAVINOV")
                return;
            var repo = ServiceFactory.GetPriceSchedulerRepository();
            var settings = repo.GetSchedulers();
            //var time = repo.GetLastSendDate(settings[0].Id);
            //var nextTime = settings[0].GetNextTime(time);

            return;
            //var prRepo = ServiceFactory.GetPriceSchedulerRepository();
            //GetPricesFromDb(prRepo, settings.OrderBy(x => x.ClientId).Take(1).Select(x=>x.ClientId).ToList());
            //GetPricesFromDb(prRepo, settings.OrderBy(x => x.Email).Take(10).Select(x=>x.ClientId).ToList());
            //GetPricesFromDb(prRepo, settings.OrderBy(x => x.FileName).Take(100).Select(x=>x.ClientId).ToList());

        }




        [TestMethod]
        [TestCategory("local")]
        public void TestSendPrice()
        {
            if (Environment.MachineName != "VSAVINOV")
                return;

            var repo = ServiceFactory.GetPriceSchedulerRepository();
            var setting = repo.GetSchedulers().First();
            setting.Email = "0vassav0@gmail.com";
            //var priceScheduler=new PriceScheduler();
            //priceScheduler.ProcessItem(setting);
            Thread.Sleep(30000);

        }
        [TestMethod]
        [TestCategory("local")]
        public void GetPriceFiles()
        {
            var repo = ServiceFactory.GetPriceSchedulerRepository();
            var settings = repo.GetSchedulers();
            GetPricesFile( settings.OrderBy(x => x.ClientId).Take(1).Select(x=>x.ClientId).ToList());
            GetPricesFile( settings.OrderBy(x => x.Email).Take(10).Select(x=>x.ClientId).ToList());
            GetPricesFile( settings.OrderBy(x => x.FileName).Take(100).Select(x=>x.ClientId).ToList());

        }

        private void GetPricesFile( List<int> ids)
        {
            var sw = Stopwatch.StartNew();
            var repo = ServiceFactory.GetPriceSchedulerRepository();

            var tasks = ids.Select(x => Task.Factory.StartNew(() => new PriceMailSender(x,repo).CreatePrice())).ToArray();
            Task.WaitAll(tasks);

            Debug.WriteLine("GetPricesFile. time:{1}, Ids:{0} ", ids.Count, sw.ElapsedMilliseconds / 1000D);
        }

        //private void GetPricesFromDb(SchedulerRepository repo, List<int> ids)
        //{
        //    var sw = Stopwatch.StartNew();

        //    var tasks = ids.Select(x => Task.Factory.StartNew(() => repo.GetPriceItems(x))).ToArray();
        //    Task.WaitAll(tasks);

        //    Console.WriteLine("GetPriceFromDb. time:{1}, Ids:{0} ", string.Join(",",ids.Select(x=>x.ToString())),sw.ElapsedMilliseconds/1000D);
        //}

    }
}