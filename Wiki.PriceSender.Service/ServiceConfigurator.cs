using System;
using System.Web.Http;
using Owin;
using Wiki.Service.Configuration;

namespace Wiki.PriceSender.Service
{
    public class ServiceConfigurator : IServiceConfig
    {
        internal readonly PriceScheduler _priceScheduler;
        internal readonly ConfigScheduler _configScheduler;
        internal bool _config;


        private static ServiceConfigurator _service = new ServiceConfigurator();

        public static ServiceConfigurator GetService
        {
            get { return _service;}
        }

        private ServiceConfigurator()
        {

            SenderSrv.Logger.WriteEvent("Init service");

            try
            {
                this._priceScheduler= new PriceScheduler();
                this._configScheduler = new ConfigScheduler();
                this._config = ServiceFactory.PriceSchedulerConfig();
            }
            catch (Exception e)
            {
                SenderSrv.Logger.WriteError("Error init ServiceConfigurator.",e);
                throw;
            }
        }

        public void Init(IAppBuilder app, HttpConfiguration config) 
        {
            
        }

        public void Start()
        {
            //#if !DEBUG
            if (_config)
            {
                this._priceScheduler.Run();
            }
            this._configScheduler.Start();
            SenderSrv.Logger.WriteEvent("********** ServiceConfigurator Service started *********");
            //#endif
        }

        public void Stop()
        {
            this._priceScheduler.Finish();
            this._configScheduler.Stop();
            SenderSrv.Logger.WriteEvent("********** Service stoped *********");

        }
    }
}