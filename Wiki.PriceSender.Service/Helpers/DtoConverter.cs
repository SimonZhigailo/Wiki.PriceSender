using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wiki.PriceSender.Service.Models.PriceList;
using Wiki.PriceSender.Dto.PriceList;
using Wiki.PriceSender.Dto.PriceList.Articles;
using Wiki.PriceSender.Dto.PriceList.MainPrice;
using Wiki.PriceSender.Dto.PriceList.PriceHistory;
using Wiki.PriceSender.Service.Models;
using Wiki.PriceSender.Service.Models.PriceList.MainPrice;
using Wiki.PriceSender.Service.Models.PriceList.PriceHistory;

namespace Wiki.PriceSender.Service.Helpers
{
    public static class DtoConverter
    {
        public static ArticleDto ConvertArticleDto(Article article)
        {
            return new ArticleDto() {ArticleNumber = article.ArticleNumber, Catalog = article.Catalog, Id = article.Id, Name = article.Name}; 
        }

        public static ArticleCatalogsDto ConvertArticleCatalogsDto(ArticleCatalogs articleCatalogs)
        {
            return new ArticleCatalogsDto() {Article = articleCatalogs.Article, CatalogId = articleCatalogs.CatalogId, IsOriginal = articleCatalogs.IsOriginal, IsOurBrand = articleCatalogs.IsOurBrand, KeyUrl = articleCatalogs.KeyUrl, Name = articleCatalogs.Name};
        }

        public static PriceGroupDataResponseDto ConvertDataResponseDto(PriceGroupDataResponse response)
        {
            return new PriceGroupDataResponseDto() {Analog = response.Analog, ErpId = response.ErpId, GroupId = response.GroupId, MinOrder = response.MinOrder, Name = response.Name, PriceListId = response.PriceListId, Qty = response.Qty, SellerCatalog = response.SellerCatalog, SellerNumber = response.SellerNumber, total = response.total};
        }

        public static PriceGroupDataRequest ConvertDataRequest(PriceGroupDataRequestDto request)
        {
            if (request.Sort != null)
            {
                var items = new List<SortItem>();

                foreach (var dto in request.Sort)
                {
                    items.Add(ConvertSortItem(dto));
                }
                return new PriceGroupDataRequest() { CatalogFilter = request.CatalogFilter, GroupId = request.GroupId, NameFilter = request.NameFilter, NumberFilter = request.NumberFilter, Page = request.Page, PageSize = request.PageSize, Skip = request.Skip, Take = request.Take, Sort = items };

            }
            return new PriceGroupDataRequest() { CatalogFilter = request.CatalogFilter, GroupId = request.GroupId, NameFilter = request.NameFilter, NumberFilter = request.NumberFilter, Page = request.Page, PageSize = request.PageSize, Skip = request.Skip, Take = request.Take, Sort = null };

        }

        public static SortItem ConvertSortItem(SortItemDto item)
        {
            return new SortItem() {Dir = item.Dir, Field = item.Field};
        }

        public static PriceChangeModelResponseDto ConvertPriceChangeModelResponseDto(PriceChangeModelResponse response)
        {
            return new PriceChangeModelResponseDto() {GroupId = response.GroupId, ManagerName = response.ManagerName, Message = response.Message, Time = response.Time, total = response.total, Value = new KeyValuePair<string, string>(response.Value.Key, response.Value.Value), Type = response.Type};
        }

        public static PriceChangeModelRequest ConvertPriceChangeModelRequest(PriceChangeModelRequestDto request)
        {
            if (request.Sort != null)
            {
                var items = new List<SortItem>();

                foreach (var dto in request.Sort)
                {
                    items.Add(ConvertSortItem(dto));
                }
                return new PriceChangeModelRequest() { Page = request.Page, PageSize = request.PageSize, Skip = request.Skip, Sort = items, Take = request.Take };

            }
            return new PriceChangeModelRequest() { Page = request.Page, PageSize = request.PageSize, Skip = request.Skip, Sort = null, Take = request.Take };

        }

        public static ClientChangeModelResponseDto ConvertClientChangeModelResponseDto(
            ClientChangeModelResponse response)
        {
            return new ClientChangeModelResponseDto() {DaysSend = response.DaysSend, FileConfig = response.FileConfig};
        }

        public static ClientChangeModelRequest ConvertClientChangeModelRequest(ClientChangeModelRequestDto request)
        {
            if (request.Sort != null)
            {
                var items = new List<SortItem>();

                foreach (var dto in request.Sort)
                {
                    items.Add(ConvertSortItem(dto));
                }
                return new ClientChangeModelRequest() { PageSize = request.PageSize, Page = request.Page, Skip = request.Skip, Sort = items, Take = request.Take };

            }
            return new ClientChangeModelRequest() { PageSize = request.PageSize, Page = request.Page, Skip = request.Skip, Sort = null, Take = request.Take };

        }

        public static ClientDto ConvertClientDto(Client client)
        {
            return new ClientDto() {ClientCode = client.ClientCode, Company = client.Company, Email = client.Email, Id = client.Id, Name = client.Name};
        }

        public static PriceGroupListDto ConvertPriceGroupListDto(PriceGroupList pgl)
        {
            return new PriceGroupListDto() {Name = pgl.Name, Id = pgl.Id, CountClients = pgl.CountClients, CountRows = pgl.CountRows};
        }

        public static PriceGroupPriceListDto ConvertGroupPriceListDto(PriceGroupPriceList price)
        {
            return new PriceGroupPriceListDto() {Id = price.Id, ListOrder = price.ListOrder, Name = price.Name};
        }

        public static CatalogDto ConvertCatalogDto(Catalog cat)
        {
            return new CatalogDto() {Id = cat.Id, IsEnabled = cat.IsEnabled, IsOriginal = cat.IsOriginal, Name = cat.Name, KeyUrl = cat.KeyUrl};
        }

        public static PriceGroupToClientDto ConvertPriceGroupToClientDto(PriceGroupToClient pg)
        {
            return new PriceGroupToClientDto() {GroupId = pg.GroupId, ClientCode = pg.ClientCode, Id = pg.Id, ClientId = pg.ClientId, ClientName = pg.ClientName, DaysSend = pg.DaysSend, FileConfig = pg.FileConfig, ManagerName = pg.ManagerName, MailBody = pg.MailBody, FileName = pg.FileName, MailSubject = pg.MailSubject, ToEmail = pg.ToEmail, IsEnabled = pg.IsEnabled, TimeSend = pg.TimeSend, ProfileId = pg.ProfileId, FileType = pg.FileType, FromEmail = pg.FromEmail};
        }

        public static PriceSendModelDto ConvertPriceSendModelDto(PriceSendModel psm)
        {
            return new PriceSendModelDto() {GroupId = psm.GroupId, Body = psm.Body, DaysSend = psm.DaysSend, FileConfig = psm.FileConfig, FileName = psm.FileName, Subject = psm.Subject, TimesSend = psm.TimesSend, IsEnabled = psm.IsEnabled, Email = psm.Email, ClientId = psm.ClientId, FileType = psm.FileType, ProfileId = psm.ProfileId, EmailSetting = psm.EmailSetting};
        }

        public static PriceSendModel ConvertPriceSendModel(PriceSendModelDto psmDto)
        {
            return new PriceSendModel() {GroupId = psmDto.GroupId, Body = psmDto.Body, DaysSend = psmDto.DaysSend, FileConfig = psmDto.FileConfig, FileName = psmDto.FileName, Subject = psmDto.Subject, TimesSend = psmDto.TimesSend, IsEnabled = psmDto.IsEnabled, Email = psmDto.Email, ClientId = psmDto.ClientId, FileType = psmDto.FileType, ProfileId = psmDto.ProfileId, EmailSetting = psmDto.EmailSetting};
        }

        public static RadioButtonsDto ConvertRadioButtonsDto(RadioButtons but)
        {
            return new RadioButtonsDto() {ActionArticles = but.ActionArticles, ActionCatalog = but.ActionCatalog, ActionGroupTree = but.ActionGroupTree};
        }

        public static PriceSendModelResponseDto ConvertPriceSendHistoryDto(PriceSendModelResponse response)
        {
            return new PriceSendModelResponseDto() {GroupId = response.GroupId, FileName = response.FileName, ClientId = response.ClientId, EmailFrom = response.EmailFrom, ProfileId = response.ProfileId, EmailTo = response.EmailTo, FilePath = response.FilePath, NextTime = response.NextTime, SendTime = response.SendTime, total = response.total};
        }

        public static PriceSendErrorModelResponseDto ConvertPriceSendHistoryErrorModelResponseDto(
            PriceSendErrorModelResponse response)
        {
            return new PriceSendErrorModelResponseDto() {GroupId = response.GroupId, ClientId = response.ClientId, EmailTo = response.EmailTo, Message = response.Message, ProfileId = response.ProfileId, total = response.total};
        }

        public static PriceSendModelRequest ConvertPriceSendModelRequest(PriceSendModelRequestDto request)
        {
            if (request.Sort != null)
            {
                var items = new List<SortItem>();

                foreach (var dto in request.Sort)
                {
                    items.Add(ConvertSortItem(dto));
                }
                return new PriceSendModelRequest() {Sort = items, PageSize = request.PageSize, Page = request.Page, Skip = request.Skip, Take = request.Take};
            }
            return new PriceSendModelRequest() { Sort = null, PageSize = request.PageSize, Page = request.Page, Skip = request.Skip, Take = request.Take };

        }

        public static PriceSendErrorModelRequest ConvertPriceSendErrorModelRequest(PriceSendErrorModelRequestDto request)
        {
            if (request.Sort != null)
            {
                var items = new List<SortItem>();

                foreach (var dto in request.Sort)
                {
                    items.Add(ConvertSortItem(dto));
                }
                return new PriceSendErrorModelRequest() { Sort = items, PageSize = request.PageSize, Page = request.Page, Skip = request.Skip, Take = request.Take };
            }
            return new PriceSendErrorModelRequest() { Sort = null, PageSize = request.PageSize, Page = request.Page, Skip = request.Skip, Take = request.Take };

        }
    }
}
