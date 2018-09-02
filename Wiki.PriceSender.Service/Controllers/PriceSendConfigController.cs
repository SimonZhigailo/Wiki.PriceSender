using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Wiki.PriceSender.Dto;
using Wiki.PriceSender.Service.PriceSender;

namespace Wiki.PriceSender.Service.Controllers
{
    [RoutePrefix("api/pricesendconfig")]
    public class PriceSendConfigController : ApiController
    {
        private readonly SchedulerRepository _schedulerRepository = ServiceFactory.GetPriceSchedulerRepository();

        [HttpGet]
        [Route("{id}")]
        public SchedulerItem GetConfig(int id)
        {
            return this._schedulerRepository.GetScheduler(id);
        }


        //[HttpPost]
        //public SchedulerItem SaveConfig(SchedulerItem schedulerItem)
        //{
        //    this._schedulerRepository.SaveScheduler(schedulerItem);
        //    return schedulerItem;
        //}

        //составляет и возвращает прайс по Id шедулера в виде Byte[]
        [HttpGet]
        [Route("price/{configId}")]
        public HttpResponseMessage Price(int configId)
        {
            var priceSender = new PriceMailSender(configId, this._schedulerRepository);

            var file = priceSender.CreatePrice();

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(file)
            };
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = priceSender.Config.FileType
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }

        [HttpGet]
        [Route("arhive/{configId}")]
        public ArhiveFile[] GetAhive(int configId)
        {
            return PriceMailSender.GetArhives(configId);
        }

        [HttpGet]
        [Route("arhives")]
        public ArhiveFile[] GetAllAhives()
        
        {
            return PriceMailSender.GetAllArhives().OrderByDescending(o=>o.Date).ToArray();
        }

        //[HttpGet]
        //[Route("emailprofiles/{clientId}")]
        //public ArhiveFile[] GetEmailProfiles(int clientId)
        //{
        //    return ServiceFactory.GerMailOrderRepository().GetEmailSettingsForMailingPricesService().Where(o=>o.);
        //}

        [HttpGet]
        [Route("arhive/{configId}/{fileName}")]
        public HttpResponseMessage GetAhiveFile(int configId, string fileName)
        {
            var dir = PriceMailSender.GetSchedulerConfigDir(configId);
            var fullName = Path.Combine(dir, fileName + ".zip");
            if (!File.Exists(fullName))
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "File not found");

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(File.ReadAllBytes(fullName))
            };
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = "arhive.zip"
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }

    }
}