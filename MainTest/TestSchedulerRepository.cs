using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Wiki.PriceSender.Service;
using Wiki.PriceSender.Service.Models;

namespace MainTest
{
    [TestClass]
    public class TestSchedulerRepository
    {
        //[TestMethod]
        //public void TestPriceGroups()
        //{
        //    var repo = TestFactory.GerSchedulerRepository();

        //    var groups = repo.GetPriceGroups();

        //    Assert.AreEqual(2,groups.Count);
        //    Assert.AreEqual(2,groups[0].PriceListIds.Count);

        //    var group = repo.GetPriceGroup(2);
        //    Assert.AreEqual(2,group.Id);
        //    Assert.AreEqual(1,group.PriceListIds.Count);
        //}


        //[TestMethod]
        //public void TestScheduler()
        //{
        //    var repo = TestFactory.GerSchedulerRepository();
        //    var item=new SchedulerItem
        //    {
        //        ProfileId = 1,
        //        Body = "body text",
        //        ClientId = 1,
        //        DaysSend = "1111100",
        //        Email = "qqq@test.com",
        //        FileConfig = "",
        //        FileName = "price.xls",
        //        FileType = "Xls",
        //        GroupId = 2,
        //        IsEnabled = true,
        //        Subject = "subject",
        //        TimesSend = "10:30"
                
        //    };

        //    repo.SaveScheduler(item);

        //    Assert.AreNotEqual(0, item.Id);

        //    var dbItem = repo.GetScheduler(item.Id);

        //    Assert.IsNotNull(dbItem);
        //    Assert.AreEqual(JsonConvert.SerializeObject(item),JsonConvert.SerializeObject(dbItem));

        //    repo.DeleteScheduler(item.Id);

        //    Assert.IsNull(repo.GetScheduler(item.Id));
        //}

        //[TestMethod]
        //public void TestSaveEvent()
        //{

        //    var time = DateTime.Now.AddHours(-5);
        //    time=new DateTime(time.Ticks/ 100000* 100000);

        //    var shedulerId = -1;
        //    var sendEvent = new SendPriceEvent(shedulerId,DateTime.Now);
        //    sendEvent.FileName = "price.xls";
        //    sendEvent.FilePath = @"d:\tempt\price.xls";
        //    sendEvent.Start=time;
        //    var repo = TestFactory.GerSchedulerRepository();

        //    sendEvent.BeginStep("test",DateTime.Now.AddMilliseconds(-100));
        //    sendEvent.FinishLastStep(DateTime.Now);
        //    repo.SaveSendEvent(sendEvent);

        //    var savedTime = repo.GetLastSendDate(shedulerId);
        //    var allTimes = repo.GetLastSendDates();

        //    Assert.AreEqual(time,savedTime);

        //    Assert.AreEqual(time,allTimes[shedulerId]);
        //}

        [TestMethod]
        public void TestTimeScheduller()
        {

            //написать одноразовую процедуру переноса времени из PriceSendConfig в PriceSendClient перед первым запуском боевым

            //var repo = ServiceFactory.GetPriceSchedulerRepository();

            //var items = repo.GetSchedulers();

            var scheduler = new PriceScheduler();

            var items = scheduler.GetItemsToProcess(DateTime.Now);
            scheduler.Start();
            //
            return;
        }
    }
}