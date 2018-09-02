using System.Configuration;
using Wiki.PriceSender.Service.PriceSender;

namespace MainTest
{
    internal class TestFactory
    {
        private static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["db"].ConnectionString; }
        }

        public static SchedulerRepository GerSchedulerRepository()
        {
            return new SchedulerRepository(ConnectionString);
        }

    }
}