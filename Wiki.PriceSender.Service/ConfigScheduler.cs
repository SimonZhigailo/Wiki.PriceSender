using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarParts.Common.Log;

namespace Wiki.PriceSender.Service
{
    internal class ConfigScheduler : SchedulerBase
    {
        private ServiceConfigurator _service;
        private bool _config;

        public ConfigScheduler() : base(new FileLogger("ConfigScheduler"))
        {
            this._interval = 2000;
            this._service = ServiceConfigurator.GetService;
            Wiki.Service.Configuration.ConfigurationContainer.Configuration.Load();
        }

        protected override void MainProcess()
        {
            var config = ServiceFactory.PriceSchedulerConfig();

            if (!config && _service._priceScheduler.IsActive)
            {
                try
                {
                    _service._priceScheduler.Finish();
                    this._logger.WriteEvent("ConfigScheduler time:{0}, PriceScheduler stoped", DateTime.Now);
                }
                catch (Exception e)
                {
                    this._logger.WriteError(string.Format("ConfigScheduler time:{0}, Error:{1}", DateTime.Now, e.Message), e);
                }

            }
            else if (config && !_service._priceScheduler.IsActive)
            {
                try
                {
                    _service._priceScheduler.Run();
                    this._logger.WriteEvent("ConfigScheduler time:{0}, PriceScheduler started", DateTime.Now);
                }
                catch (Exception e)
                {
                    this._logger.WriteError(string.Format("ConfigScheduler time:{0}, Error:{1}", DateTime.Now, e.Message), e);
                }
            }
            
            }
        }
    }

