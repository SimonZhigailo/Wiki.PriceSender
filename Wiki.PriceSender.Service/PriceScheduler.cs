using System;
using System.Collections.Generic;
using EasyXLS.Util;
using Wiki.Core.Scheduler;
using System.Linq;
using System.Threading.Tasks;
using CarParts.Common.Log;
using Wiki.PriceSender.Dto;
using Wiki.PriceSender.Service.Models;
using Wiki.PriceSender.Service.PriceSender;

namespace Wiki.PriceSender.Service
{
    internal class PriceScheduler : SchedulerBase
    {
        private SchedulerRepository _schedulerRepository;

        private static TimeSpan _offset = new TimeSpan(0, 3, 0); //3 минуты
        private static TimeSpan _offsetLastSend = new TimeSpan(0, 4, 0);
        private bool _isActive;

        public bool IsActive
        {
            get { return this._isActive; }
            private set { _isActive = value; }
        }

        public PriceScheduler() : base(new FileLogger("PriceScheduler"))
        {
            this._schedulerRepository = ServiceFactory.GetPriceSchedulerRepository();
            this.IsActive = ServiceFactory.PriceSchedulerConfig();
        }

        public void Run()
        {
            this.Start();
            this.IsActive = true;
            Wiki.Service.Configuration.ConfigurationContainer.Configuration["power"] = "true";
            Wiki.Service.Configuration.ConfigurationContainer.Configuration.Save();
        }


        public void Finish()
        {
            this.Stop();
            IsActive = false;
            Wiki.Service.Configuration.ConfigurationContainer.Configuration["power"] = "false";
            Wiki.Service.Configuration.ConfigurationContainer.Configuration.Save();
        }



        protected override void MainProcess()
        {
            try
            {
                var time = DateTime.Now;
                var items = GetItemsToProcess(time);

                if (items.Count > 0)
                {
                    this._logger.WriteEvent("PriceScheduler tick:{0}, ids:{1}", time,
                        string.Join(",", items.Select(x => x.Id.ToString())));
                    items.ForEach(x =>
                    {
                        try
                        {
                            ProcessItem(x);
                        }
                        catch (Exception e)
                        {
                            this._logger.WriteError(
                                string.Format("Error send price. Scheduler id:{0}, time to send:{1} at Email:{2}", x.Id,
                                    time, x.Email), e);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                this._logger.WriteError("Error send price", e);
            }
        }


        public void ProcessItem(SchedulerItem item)
        {
            try
            {
                if (string.IsNullOrEmpty(item.Email))
                {
                    this._logger.WriteWarning(string.Format("Price not sent. Not set email. Id:{0}", item.Id));
                    return;
                }

                var priceSender = new PriceMailSender(item, this._schedulerRepository);

                if (priceSender.Send())
                    this._logger.WriteEvent(
                        String.Format("Begin send price.Id:{0}, email:{1},lastSend:{2}, nextTime:{3}", item.Id,
                            item.Email, item.LastSend, item.NextSend));
            }
            catch (Exception e)
            {
                this._logger.WriteError("Error send price. Id:" + item.Id, e);
            }
        }


        public List<SchedulerItem> GetItemsToProcess(DateTime time)
        {
            var items = this._schedulerRepository.GetSchedulers();
            //var times = this._schedulerRepository.GetLastSendDates();//<id,date>

            var result = new List<SchedulerItem>();

            foreach (var item in items)
            {
                if ((item.DaysSend.Equals("0000000") || item.TimesSend.Trim().Equals("")) ||
                    (item.DaysSend.Equals("0000000") && item.TimesSend.Trim().Equals("")))
                {
                    continue;
                }

                try
                {
                    item.NextSend = item.GetTime().NextTime.Value;

                    if (item.NextSend >= time && IsTimeCome(time, item))
                    {
                        result.Add(item);
                    }
                }
                catch (System.InvalidOperationException)
                {
                    this._logger.WriteWarning(
                        String.Format(
                            "Unhandled InvalidOperationException at price.Id:{0}, DaysSend:{1},TimesSend:{2}", item.Id,
                            item.DaysSend, item.TimesSend));
                }
            }

            return result.OrderBy(t => t.NextSend).ToList();
        }

        private static bool IsTimeCome(DateTime time, SchedulerItem item)
        {
            var isNextSend = (item.NextSend - time) <= _offset;
            var isLastSend = ((time + _offsetLastSend) >= item.LastSend);
            if (isLastSend && isNextSend)
            {
                return true;
            }
            return false;
        }

    }
}