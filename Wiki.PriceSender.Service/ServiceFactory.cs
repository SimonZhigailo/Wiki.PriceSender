using System;
using System.Collections.Generic;
using CarParts.Data.Logging;
using Wiki.PriceSender.Service.PriceSender;
using Wiki.PriceSender.Service.Repository;

namespace Wiki.PriceSender.Service
{
    internal class ServiceFactory
    {

        //public static PriceRepository GerPriceRepository()
        //{
        //    return new PriceRepository(Wiki.Service.Configuration.ConfigurationContainer.Configuration["db"]);
        //}
        public static bool PriceSchedulerConfig()
        {
           Wiki.Service.Configuration.ConfigurationContainer.Configuration.Load();
           return Convert.ToBoolean(Wiki.Service.Configuration.ConfigurationContainer.Configuration["power"]);
        }
        public static SchedulerRepository GetPriceSchedulerRepository()
        {
            return new SchedulerRepository(Wiki.Service.Configuration.ConfigurationContainer.Configuration["db"]);
        }

        public static PriceListRepository GetPriceListRepository()
        {
            return new PriceListRepository(Wiki.Service.Configuration.ConfigurationContainer.Configuration["db"]);
        }

        public static PriceChangeEvent GetPriceChangeEvent(int groupId, string managerName, KeyValuePair<string, string> val, string message, string type)
        {
            return new PriceChangeEvent(groupId,managerName, val, message, type);
        }

        public static PriceChangeClientEvent GetPriceChangeClientEvent(int groupId,
            List<KeyValuePair<string, string>> vals, string managerName,string message, string type)
        {
            return new PriceChangeClientEvent(groupId, vals, managerName, message, type);
        }

        public static PriceCreateClientEvent GetPriceCreateClientEvent(int groupId,
            List<KeyValuePair<string, string>> vals, string managerName, string message, string type)
        {
            return new PriceCreateClientEvent(groupId, vals, managerName, message, type);
        }

        public static PriceDeleteClientEvent GetPriceDeleteClientEvent(int groupId,
    List<KeyValuePair<string, string>> vals, string managerName, string message, string type)
        {
            return new PriceDeleteClientEvent(groupId, vals, managerName, message, type);
        }

    }
}