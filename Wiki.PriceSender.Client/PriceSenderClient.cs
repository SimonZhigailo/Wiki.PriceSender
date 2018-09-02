using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wiki.Core.Exceptions;
using Wiki.PriceSender.Dto.PriceList;
using Wiki.PriceSender.Dto.PriceList.Articles;
using Wiki.PriceSender.Dto.PriceList.MainPrice;
using Wiki.PriceSender.Dto.PriceList.PriceHistory;
using Wiki.Service.Common.Clients;

namespace Wiki.PriceSender.Client
{
    public class PriceSenderClient: ServiceClientBase
    {
        public PriceSenderClient() : this(
            ConfigurationManager.AppSettings["DiscoveryUrl"],
            ConfigurationManager.AppSettings["ClientId"],
            ConfigurationManager.AppSettings["ClientSecret"]
            ) { }

        public PriceSenderClient(string discoveryUrl, string clientId, string clientSecret)
            : base(discoveryUrl, clientId, clientSecret)
        {
        }

        public static PriceSenderClient CreateClient()
        {
#if DEBUG
            return new PriceSenderClient("http://127.0.0.1:9100", "service", "servicepasswd");
#else
            return new PriceSenderClient();
#endif
        }

        public override string ServiceId
        {
            get { return "Wiki.PriceSender.Service"; }
        }

        public override string FullUrl
        {
            get { return "api/priceLists/"; }
        }

        public async Task<List<PriceGroupListDto>> GetGroups()
        {
            return await this.GetData<List<PriceGroupListDto>>("getGroups");
        }

        public async Task<int> PriceGroupSave(int id, string name)
        {
            return await this.GetData<int>(string.Format("priceGroupSave?id={0}&name={1}", id, name));
        }

        public async Task<bool> UpdatePriceGroup(int groupId)
        {
            return await this.GetData<bool>(string.Format("updatePriceGroup?groupId={0}", groupId));
        }

        public async Task<PriceGroupSettingsDto> PriceListSettings(int groupId)
        {
            return await this.GetData<PriceGroupSettingsDto>(string.Format("priceListSettings?groupId={0}", groupId));

        }

        public async Task<List<CatalogDto>> GetAllCatalogs(int groupId)
        {
            return await this.GetData<List<CatalogDto>>(string.Format("catalog/getAllCatalogs?groupId={0}", groupId));
        }

        public async Task<List<CatalogDto>> GetCatalogs(int groupId)
        {
            return await this.GetData<List<CatalogDto>>(string.Format("catalog/getCatalogs?groupId={0}", groupId));
        }

        public async Task<string> DeleteCatalog(int groupId, int catalogId, string userId)
        {
            return await this.GetData<string>(string.Format("catalog/deleteCatalog?groupId={0}&catalogId={1}&userId={2}", groupId, catalogId, userId));
        }

        public async Task<string> InsertCatalog(int groupId, int catalogId, string userId)
        {

            return await this.GetData<string>(string.Format("catalog/insertCatalog?groupId={0}&catalogId={1}&userId={2}", groupId, catalogId, userId));

        }

        public async Task<List<ArticleDto>> GetArticles(int groupId)
        {
            return await this.GetData<List<ArticleDto>>(string.Format("article/getArticles?groupId={0}", groupId));
        }

        public async Task<List<ArticleCatalogsDto>> GetArticleCatalog(string key)
        {
            return
                await this.GetData<List<ArticleCatalogsDto>>(string.Format("article/getArticleCatalog?key={0}", key));
        }

        public async Task<HttpResponseMessage> SetArticleGroup(string article, int catalog, int groupId, string userId)
        {
            var result =
                await
                    this.GetData<int>(string.Format("article/setArticleGroup?article={0}&catalog={1}&groupId={2}&userId={3}",
                        article, catalog, groupId, userId));
            if (result > 0)
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotModified);
            }
        }

        public async Task<HttpResponseMessage> DeleteArticle(int id, int groupId, string userId)
        {
            var result = await this.GetData<int>(string.Format("article/deleteArticle?id={0}&groupId={1}&userId={2}",
                id, groupId, userId));
            if (result > 0)
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotModified);
            }
        }

        public async Task<List<PriceGroupPriceListDto>> GetAllPrices(int groupId)
        {
            return await this.GetData<List<PriceGroupPriceListDto>>(string.Format("prices/getAllPrices?groupId={0}", groupId));
        }
        public async Task<List<PriceGroupPriceListDto>> PriceList(int groupId)
        {
            return await this.GetData<List<PriceGroupPriceListDto>>(string.Format("prices/priceList?groupId={0}", groupId));
        }


        public async Task<string> EditListOrder(int groupId, int priceListId, int listOrder)
        {
            return await this.GetData<string>(string.Format("prices/editListOrder?groupId={0}&priceListId={1}&listOrder={2}", groupId, priceListId, listOrder));
        }

        public async Task<string> PriceListSave(int groupId, int priceListId, int listOrder, string userId)
        {
            return await this.GetData<string>(string.Format("prices/priceListSave?groupId={0}&priceListId={1}&listOrder={2}&userId={3}", groupId, priceListId, listOrder, userId));
        }

        public async Task<string> DeletePriceList(int groupId, int priceListId, string userId)
        {
            return await this.GetData<string>(string.Format("prices/deletePriceList?groupId={0}&priceListId={1}&userId={2}", groupId, priceListId, userId));
        }

        public async Task<List<int>> GetSelectedNodesTree(int groupId)
        {
            return await this.GetData<List<int>>(string.Format("tree/getSelectedNodesTree?groupId={0}", groupId));
        }

        public async Task<string> InsertTree(int groupId, int treeId, string userId)
        {
            return await this.GetData<string>(string.Format("tree/insertTree?groupId={0}&treeId={1}&userId={2}", groupId, treeId, userId));
        }

        public async Task<string> DeleteNodeTree(int groupId, int treeId, string userId)
        {
            return await this.GetData<string>(string.Format("tree/deleteNodeTree?groupId={0}&treeId={1}&userId={2}", groupId, treeId, userId));
        }

        public async Task<RadioButtonsDto> GetRadioButtonsValues(int groupId)
        {
            return await this.GetData<RadioButtonsDto>(string.Format("radio/getValues?groupId={0}", groupId));
        }

        public async Task<string> SetActionArticles(int groupId, bool value, string userId)
        {
            return await this.GetData<string>(string.Format("radio/setActionArticles?groupId={0}&value={1}&userId={2}", groupId, value, userId));
        }


        public async Task<string> SetActionCatalog(int groupId, bool value, string userId)
        {
            return await this.GetData<string>(string.Format("radio/setActionCatalog?groupId={0}&value={1}&userId={2}", groupId, value, userId));

        }
        public async Task<string> SetActionGroupTree(int groupId, bool value, string userId)
        {
            return await this.GetData<string>(string.Format("radio/setActionGroupTree?groupId={0}&value={1}&userId={2}", groupId, value, userId));
        }

        public async Task<List<PriceGroupToClientDto>> GetClientsInfo(int groupId)
        {
            return await this.GetData<List<PriceGroupToClientDto>>(string.Format("clients/getClientInfo?groupId={0}", groupId));
        }

        public async Task<PriceGroupToClientDto> SaveClientInfo(PriceSendModelDto model, string userId)
        {
             return await this.PostData<PriceGroupToClientDto>("clients/saveClientInfo?userId="+userId.ToString(), model);
        }

        public async Task<PriceGroupToClientDto> CreateClientInfo(PriceSendModelDto model, string userId)
        {
            return await this.PostData<PriceGroupToClientDto>("clients/createClientInfo?userId=" + userId.ToString(), model);
        }

        public async Task<string> DeleteClientInfo(int id, string userId)
        {
            return await this.GetData<string>(string.Format("clients/deleteClientInfo?id={0}&userId={1}", id, userId));
        }

        public async Task<List<ClientDto>> GetClientIdFilter(int nums)
        {
            return await this.GetData<List<ClientDto>>("clients/getClientIdFilter?nums=" + nums);
        }

        public async Task<List<PriceChangeModelResponseDto>> GetPricesHistory(PriceChangeModelRequestDto request)
        {
            return await this.PostData<List<PriceChangeModelResponseDto>>("filters/getPriceChange", request);
        }

        public async Task<List<ClientChangeModelResponseDto>> GetClientsUpdateHistory(ClientChangeModelRequestDto request)
        {
            return await this.PostData<List<ClientChangeModelResponseDto>>("filters/getClientChange", request);
        }

        public async Task<List<ClientChangeModelResponseDto>> GetClientsCreateHistory(ClientChangeModelRequestDto request)
        {
            return await this.PostData<List<ClientChangeModelResponseDto>>("filters/getClientCreate", request);
        }

        public async Task<List<ClientChangeModelResponseDto>> GetClientsDeleteHistory(ClientChangeModelRequestDto request)
        {
            return await this.PostData<List<ClientChangeModelResponseDto>>("filters/getClientDelete", request);
        }

        public async Task<List<PriceGroupDataResponseDto>> GetPriceGroupData(PriceGroupDataRequestDto request)
        {
            return await this.PostData<List<PriceGroupDataResponseDto>>("filters/getPriceGroupData", request);
        }

        public async Task<List<PriceGroupDataResponseDto>> GetExcelPriceGroup(int groupId)
        {
            return await this.GetData<List<PriceGroupDataResponseDto>>("excel/getExcelPriceGroup?groupId=" + groupId);
        }

        public async Task<List<PriceSendModelResponseDto>> GetPriceSendHistory(PriceSendModelRequestDto model)
        {
            return await this.PostData<List<PriceSendModelResponseDto>>("filters/getPriceSendHistory", model);
        }

        public async Task<List<PriceSendErrorModelResponseDto>> GetPriceSendErrorHistory(
            PriceSendErrorModelRequestDto model)
        {
            return await this.PostData<List<PriceSendErrorModelResponseDto>>("filters/getPriceErrorSendHistory", model);
        }

        public async Task<bool> SendPrice(int configId)
        {
            return await this.GetData<bool>(string.Format("scheduler/send?configId="+ configId));
        }

        public async Task<bool> SendPricesByGroupId(int groupId)
        {
            return await this.GetData<bool>(string.Format("scheduler/sendAll?groupId="+ groupId));
        }

        public async Task<bool> SchedulerPower(bool value)
        {
            return await this.GetData<bool>(string.Format("scheduler/power?value=" + value));
        }

        public async Task<bool> GetShedulerPower()
        {
            return await this.GetData<bool>("scheduler/getPower");
        }
    }
}
