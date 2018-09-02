using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wiki.PriceSender.Service.Models;
using Wiki.PriceSender.Service.Models.PriceList;


namespace Wiki.PriceSender.Service.Repository
{
    interface IPriceListRepository
    {
        List<PriceGroupList> GetGroups();

        int PriceGroupSave(int id, string name);

        List<Article> GetArticles(int groupId);

        List<ArticleCatalogs> GetArticleCatalog(string key);

        int SetArticleGroup(string article, int catalog, int groupId, string userId);

        int DeleteArticle(int id, int groupId, string userId);

        List<Catalog> GetCatalogs(int groupId);

        string DeleteCatalog(int groupId, int catalogId, string userId);

        string InsertCatalog(int groupId, int catalogId, string userId);

        List<Catalog> GetAllCatalogs(int groupId);

        List<PriceGroupPriceList> PriceList(int groupId);

        string EditListOrder(int groupId, int priceListId, int listOrder);

        string PriceListSave(int groupId, int priceListId, int listOrder, string userId);

        string DeletePriceList(int groupId, int priceListId, string userId);

        List<PriceGroupPriceList> GetAllPrices(int groupId);

        string InsertTree(int groupId, int treeId, string userId);

        List<int> GetSelectedNodesTree(int groupId);

        string DeleteNodeTree(int groupId, int treeId, string userId);

        RadioButtons GetRadioButtonsValues(int groupId);

        string SetActionArticles(int groupId, bool value, string userId);

        string SetActionCatalog(int groupId, bool value, string userId);

        string SetActionGroupTree(int groupId, bool value, string userId);

        List<PriceGroupToClient> GetClientsInfo(int groupId);

        PriceGroupToClient SaveClientInfo(PriceSendModel model, string userId);

        PriceGroupToClient CreateClientInfo(PriceSendModel model, string userId);

        string DeleteClientInfo(int id, string userId);

        List<Client> GetClientIdFilter(int nums);

        List<Client> GetClientNameFilter(string str);

    }
}
