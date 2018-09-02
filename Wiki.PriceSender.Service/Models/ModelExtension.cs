using System;
using Wiki.Core.Scheduler;
using Wiki.PriceSender.Dto;

namespace Wiki.PriceSender.Service.Models
{
    public static class ModelExtension
    {
        public static DateTime? GetNextTime(this SchedulerItem item,DateTime time)
        {
            return new SchedulerTime(item.DaysSend, item.TimesSend).GetNextTime(time);
        }

        public static SchedulerTime GetTime(this SchedulerItem item)
        {
            return new SchedulerTime(item.DaysSend, item.TimesSend);
        }
    }
}