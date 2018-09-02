using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Wiki.PriceSender.Dto;
using Wiki.PriceSender.Dto.PriceList;
using Wiki.PriceSender.Service.Helpers;
using Wiki.PriceSender.Service.PriceSender;
using Wiki.PriceSender.Service.Repository;
using Wiki.PriceSender.Dto.PriceList.PriceHistory;
using Wiki.PriceSender.Dto.PriceList.Articles;
using Wiki.PriceSender.Dto.PriceList.MainPrice;


namespace Wiki.PriceSender.Service.Controllers
{
    [RoutePrefix("api/priceLists")]
    public class PriceListsController : ApiController
    {
        private readonly PriceListRepository _priceRepository = ServiceFactory.GetPriceListRepository();

        private readonly SchedulerRepository _schedulerRepository = ServiceFactory.GetPriceSchedulerRepository();

        [HttpGet]
        [Route("getGroups")]
        public async Task<List<PriceGroupListDto>> GetGroups()
        {
            var result = new List<PriceGroupListDto>();
            var res = await Task.Run(() => this._priceRepository.GetGroups().Select(DtoConverter.ConvertPriceGroupListDto).ToList());
            return res.OrderBy(o => o.Id).ToList();
        }

        [HttpGet]
        [Route("priceGroupSave")]
        public async Task<int> PriceGroupSave([FromUri]string id, [FromUri]string name)
        {
            return await Task.Run(() => this._priceRepository.PriceGroupSave(Int32.Parse(id), name));
        }

        [HttpGet]
        [Route("updatePriceGroup")]
        public async Task<bool> UpdatePriceGroup([FromUri] int groupId)
        {
            return await Task.Run(() => this._priceRepository.UpdatePriceGroupData(groupId));
        }

        [HttpGet]
        [Route("article/getArticles")]
        public async Task<List<ArticleDto>> GetArticles([FromUri] int groupId)
        {
            var res = await Task.Run(() => this._priceRepository.GetArticles(groupId).Select(DtoConverter.ConvertArticleDto).ToList());
            res.Reverse();
            return res;
        }

        [HttpGet]
        [Route("article/getArticleCatalog")]
        public async Task<List<ArticleCatalogsDto>> GetArticleCatalog([FromUri]string key)
        {
            var res = await Task.Run(() => this._priceRepository.GetArticleCatalog(key).Select(DtoConverter.ConvertArticleCatalogsDto).ToList());
            return res;
        }

        [HttpGet]
        [Route("article/setArticleGroup")]
        public async Task<int> SetArticleGroup([FromUri]string article, [FromUri]int catalog, [FromUri]int groupId, [FromUri]string userId)
        {
            return await Task.Run(() => this._priceRepository.SetArticleGroup(article, catalog, groupId, userId));
        }

        [HttpGet]
        [Route("article/deleteArticle")]
        public async Task<int> DeleteArticle([FromUri] int id, [FromUri] int groupId, [FromUri]string userId)
        {
            return await Task.Run(() => this._priceRepository.DeleteArticle(id, groupId, userId));
        }

        [HttpGet]
        [Route("catalog/getAllCatalogs")]
        public async Task<List<CatalogDto>> GetAllCatalogs([FromUri]int groupId)
        {
            var res = await Task.Run(() => this._priceRepository.GetAllCatalogs(groupId).Select(DtoConverter.ConvertCatalogDto).ToList());
            return res;
        }

        [HttpGet]
        [Route("catalog/getCatalogs")]
        public async Task<List<CatalogDto>> GetCatalogs([FromUri]int groupId)
        {
            var res = await Task.Run(() => this._priceRepository.GetCatalogs(groupId).Select(DtoConverter.ConvertCatalogDto).ToList());
            return res;

        }
        [HttpGet]
        [Route("catalog/deleteCatalog")]
        public async Task<string> DeleteCatalog([FromUri]int groupId, [FromUri]int catalogId, [FromUri]string userId)
        {
            return await Task.Run(() => this._priceRepository.DeleteCatalog(groupId, catalogId, userId));
        }
        [HttpGet]
        [Route("catalog/insertCatalog")]
        public async Task<string> InsertCatalog([FromUri]int groupId, [FromUri]int catalogId, [FromUri]string userId)
        {
            return await Task.Run(() => this._priceRepository.InsertCatalog(groupId, catalogId, userId));
        }

        [HttpGet]
        [Route("prices/getAllPrices")]
        public async Task<List<PriceGroupPriceListDto>> GetAllPrices([FromUri]int groupId)
        {
            var res = await Task.Run(() => this._priceRepository.GetAllPrices(groupId).Select(DtoConverter.ConvertGroupPriceListDto).ToList());
            return res;
        }

        [HttpGet]
        [Route("prices/priceList")]
        public async Task<List<PriceGroupPriceListDto>> PriceList([FromUri]int groupId)
        {
            var res = await Task.Run(() => this._priceRepository.PriceList(groupId).Select(DtoConverter.ConvertGroupPriceListDto).ToList());
            return res;
        }
        [HttpGet]
        [Route("prices/editListOrder")]
        public async Task<string> EditListOrder([FromUri]int groupId, [FromUri]int priceListId, [FromUri]int listOrder)
        {
            return await Task.Run(() => this._priceRepository.EditListOrder(groupId, priceListId, listOrder));
        }
        [HttpGet]
        [Route("prices/priceListSave")]
        public async Task<string> PriceListSave([FromUri]int groupId, [FromUri]int priceListId, [FromUri]int listOrder, [FromUri]string userId)
        {
            return await Task.Run(() => this._priceRepository.PriceListSave(groupId, priceListId, listOrder, userId));
        }
        [HttpGet]
        [Route("prices/deletePriceList")]
        public async Task<string> DeletePriceList([FromUri]int groupId, [FromUri]int priceListId, [FromUri]string userId)
        {
            return await Task.Run(() => this._priceRepository.DeletePriceList(groupId, priceListId, userId));
        }

        [HttpGet]
        [Route("tree/getSelectedNodesTree")]
        public async Task<List<int>> GetSelectedNodesTree([FromUri]int groupId)
        {
            return await Task.Run(() => this._priceRepository.GetSelectedNodesTree(groupId));
        }

        [HttpGet]
        [Route("tree/insertTree")]
        public async Task<string> InsertTree([FromUri]int groupId, [FromUri]int treeId, [FromUri]string userId)
        {
            return await Task.Run(() => this._priceRepository.InsertTree(groupId, treeId, userId));
        }

        [HttpGet]
        [Route("tree/deleteNodeTree")]
        public async Task<string> DeleteNodeTree([FromUri]int groupId, [FromUri]int treeId, [FromUri]string userId)
        {
            return await Task.Run(() => this._priceRepository.DeleteNodeTree(groupId, treeId, userId));
        }
        [HttpGet]
        [Route("radio/getValues")]
        public async Task<RadioButtonsDto> GetRadioButtonsValues([FromUri]int groupId)
        {
            return await Task.Run(() => DtoConverter.ConvertRadioButtonsDto(this._priceRepository.GetRadioButtonsValues(groupId)));
        }
        [HttpGet]
        [Route("radio/setActionArticles")]
        public async Task<string> SetActionArticles([FromUri]int groupId, [FromUri]bool value, [FromUri]string userId)
        {
            return await Task.Run(() => this._priceRepository.SetActionArticles(groupId, value, userId));
        }
        [HttpGet]
        [Route("radio/setActionCatalog")]
        public async Task<string> SetActionCatalog([FromUri]int groupId, [FromUri]bool value, [FromUri]string userId)
        {

            return await Task.Run(() => this._priceRepository.SetActionCatalog(groupId, value, userId));
        }
        [HttpGet]
        [Route("radio/setActionGroupTree")]
        public async Task<string> SetActionGroupTree([FromUri]int groupId, [FromUri]bool value, [FromUri]string userId)
        {
            return await Task.Run(() => this._priceRepository.SetActionGroupTree(groupId, value, userId));
        }
        [HttpGet]
        [Route("clients/getClientInfo")]
        public async Task<List<PriceGroupToClientDto>> GetClientsInfo(int groupId)
        {
            var res = await Task.Run(() => this._priceRepository.GetClientsInfo(groupId));
            var result = new List<PriceGroupToClientDto>();

            foreach (var priceList in res)
            {
                result.Add(DtoConverter.ConvertPriceGroupToClientDto(priceList));
            }

            return result;

        }
        [HttpPost]
        [Route("clients/saveClientInfo")]
        public async Task<PriceGroupToClientDto> SaveClientInfo(PriceSendModelDto model, [FromUri]string userId)
        {
            var request = DtoConverter.ConvertPriceSendModel(model);
            return await Task.Run(() => DtoConverter.ConvertPriceGroupToClientDto(this._priceRepository.SaveClientInfo(request, userId)));
        }
        [HttpPost]
        [Route("clients/createClientInfo")]
        public async Task<PriceGroupToClientDto> CreateClientInfo(PriceSendModelDto model, [FromUri]string userId)
        {
            var request = DtoConverter.ConvertPriceSendModel(model);
            return await Task.Run(() => DtoConverter.ConvertPriceGroupToClientDto(this._priceRepository.CreateClientInfo(request, userId)));
        }
        [HttpGet]
        [Route("clients/deleteClientInfo")]
        public async Task<string> DeleteClientInfo([FromUri]int id, [FromUri]string userId)
        {
            return await Task.Run(() => this._priceRepository.DeleteClientInfo(id, userId));
        }
        [HttpGet]
        [Route("clients/getClientIdFilter")]
        public async Task<List<ClientDto>> GetClientIdFilter([FromUri]int nums)
        {
            var res = await Task.Run(() => this._priceRepository.GetClientIdFilter(nums));
            var result = new List<ClientDto>();

            foreach (var re in res)
            {
                result.Add(DtoConverter.ConvertClientDto(re));
            }

            return result;
        }
        [HttpPost]
        [Route("filters/getPriceChange")]
        public async Task<List<PriceChangeModelResponseDto>> GetPriceFilteredHistory(PriceChangeModelRequestDto model)
        {
            var request = DtoConverter.ConvertPriceChangeModelRequest(model);
            var res = await Task.Run(() => this._priceRepository.GetPriceHistory(request));
            var result = new List<PriceChangeModelResponseDto>();

            foreach (var re in res)
            {
                result.Add(DtoConverter.ConvertPriceChangeModelResponseDto(re));
            }

            return result;
        }
        [HttpPost]
        [Route("filters/getClientChange")]
        public async Task<List<ClientChangeModelResponseDto>> GetClientChangeFilteredHistory(ClientChangeModelRequestDto model)
        {
            var request = DtoConverter.ConvertClientChangeModelRequest(model);
            var res = await Task.Run(() => this._priceRepository.GetClientHistory(request));
            var result = new List<ClientChangeModelResponseDto>();

            foreach (var re in res)
            {
                result.Add(DtoConverter.ConvertClientChangeModelResponseDto(re));
            }

            return result;
        }

        [HttpPost]
        [Route("filters/getClientCreate")]
        public async Task<List<ClientChangeModelResponseDto>> GetClientCreateFilteredHistory(
            ClientChangeModelRequestDto model)
        {
            var request = DtoConverter.ConvertClientChangeModelRequest(model);
            var res = await Task.Run(() => this._priceRepository.GetClientsCreated(request));
            var result = new List<ClientChangeModelResponseDto>();
            foreach (var re in res)
            {
                result.Add(DtoConverter.ConvertClientChangeModelResponseDto(re));
            }
            return result;
        }

        [HttpPost]
        [Route("filters/getClientDelete")]
        public async Task<List<ClientChangeModelResponseDto>> GetClientDeleteFilteredHistory(
            ClientChangeModelRequestDto model)
        {
            var request = DtoConverter.ConvertClientChangeModelRequest(model);

            var res = await Task.Run(() => this._priceRepository.GetClientsDeleted(request));

            var result = new List<ClientChangeModelResponseDto>();
            foreach (var re in res)
            {
                result.Add(DtoConverter.ConvertClientChangeModelResponseDto(re));
            }
            return result;
        }

        [HttpPost]
        [Route("filters/getPriceGroupData")]
        public async Task<List<PriceGroupDataResponseDto>> GetPriceGroupData(PriceGroupDataRequestDto model)
        {
            var request = DtoConverter.ConvertDataRequest(model);
            var res = await Task.Run(() => this._priceRepository.GetPriceGroupData(request));

            var result = new List<PriceGroupDataResponseDto>();
            foreach (var re in res)
            {
                result.Add(DtoConverter.ConvertDataResponseDto(re));
            }
            return result;
        }



        [HttpPost]
        [Route("filters/getPriceSendHistory")]
        public async Task<List<PriceSendModelResponseDto>> GetPriceSendHistory(PriceSendModelRequestDto model)
        {
            var request = DtoConverter.ConvertPriceSendModelRequest(model);
            var res = await Task.Run(() => this._priceRepository.GetSendHistory(request));

            var result = new List<PriceSendModelResponseDto>();
            foreach (var re in res)
            {
                result.Add(DtoConverter.ConvertPriceSendHistoryDto(re));
            }
            return result;
        }

        [HttpPost]
        [Route("filters/getPriceErrorSendHistory")]
        public async Task<List<PriceSendErrorModelResponseDto>> GetPriceSendErrorHistory(
            PriceSendErrorModelRequestDto model)
        {
            var request = DtoConverter.ConvertPriceSendErrorModelRequest(model);
            var res = await Task.Run(() => this._priceRepository.GetSendErrorHistory(request));

            var result = new List<PriceSendErrorModelResponseDto>();
            foreach (var re in res)
            {
                result.Add(DtoConverter.ConvertPriceSendHistoryErrorModelResponseDto(re));
            }
            return result;
        }

        [HttpGet]
        [Route("excel/getExcelPriceGroup")]
        public async Task<List<PriceGroupDataResponseDto>> GetExcelFileForPriceGroupId([FromUri]int groupId)
        {
            return await Task.Run(() => this._priceRepository.GetPriceGroupsForExcel(groupId).Select(DtoConverter.ConvertDataResponseDto).ToList());
        }
        #region Scheduler
        /// <summary>
        /// отправляет прайс по заданному id в PriceGroupToClient
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("scheduler/send")]
        public Task<bool> Send([FromUri] int configId)
        {
            return Task.Run(() =>
            {
                var scheduler = this._schedulerRepository.GetScheduler(configId);
                var priceSender = new PriceMailSender(scheduler, this._schedulerRepository);
                priceSender.Send();
                return true;
            });
        }
        /// <summary>
        /// отправляет прайсы для всей группы
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("scheduler/sendAll")]
        public Task<bool> SendAll([FromUri]int groupId)
        {
            return Task.Run(() =>
            {
                List<SchedulerItem> schedulers = this._schedulerRepository.GetSchedulersByGroupId(groupId);
                foreach (var scheduler in schedulers)
                {
                    var priceSender = new PriceMailSender(scheduler, this._schedulerRepository);
                    priceSender.Send();
                }
                return true;
            });
        }

        [HttpGet]
        [Route("scheduler/getPower")]
        public Task<bool> GetPower()
        {
            return Task.Run(() => ServiceFactory.PriceSchedulerConfig()); 
        }

        [HttpGet]
        [Route("scheduler/power")]
        public Task<bool> Power([FromUri] bool value)
        {
                return Task.Run(() =>
                {
                    Wiki.Service.Configuration.ConfigurationContainer.Configuration["power"] = value.ToString();
                    Wiki.Service.Configuration.ConfigurationContainer.Configuration.Save();
                    return !value;
                });
        }
        #endregion
    }
}
