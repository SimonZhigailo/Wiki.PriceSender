using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using CarParts.Data.Componets;
using System.Configuration;
using System.Globalization;
using Wiki.PriceSender.Dto;
using Wiki.PriceSender.Service.Models;
using Wiki.PriceSender.Service.Models.PriceList;
using Wiki.PriceSender.Service.Models.PriceList.MainPrice;
using Wiki.PriceSender.Service.Models.PriceList.PriceHistory;

namespace Wiki.PriceSender.Service.Repository
{
    public class PriceListRepository : IPriceListRepository
    {

        protected readonly string Connection;

        public PriceListRepository(string connection)
        {
            this.Connection = connection;
        }

        protected LafSqlCommand GetCommand(string sql, bool isProcedure = false)
        {
            var cmd = new LafSqlCommand(sql, new SqlConnection(this.Connection));
            if (isProcedure)
                cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }



        public List<PriceGroupList> GetGroups()
        {
            var list = new List<PriceGroupList>();

            using (var cmd = this.GetCommand("PriceGroupList", true))
            {

                cmd.ExecuteReader(x => list.Add(new PriceGroupList()
                {
                    Id = x.GetValue<int>("Id"),
                    Name = x.GetValue<string>("Name"),
                    CountRows = x.GetValue<int>("CountRows"),
                    CountClients = x.GetValue<int>("CountClients")
                }));

            }
            return list;
        }

        public int PriceGroupSave(int id, string name)
        {
            var query =
                "UPDATE pg set pg.Name = "+name+" from [dbo].[PriceGroup] pg where pg.Id = "+id;
            using (var cmd = this.GetCommand(query))
            {
                cmd.Execute();
            }
            return id;
        }

        //public PriceGroupSettings PriceListSettings(int groupId)
        //{
        //    var result = new PriceGroupSettings();

        //    var prices = new List<PriceGroupPriceList>();
        //    var catalogItems = new List<Catalog>();
        //    var treeList = new List<Tree>();
        //    var articleList = new List<Article>();

        //    using (var cmd = this.GetCommand("PriceGroupSettings", true))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("GroupId", groupId);
        //        cmd.ExecuteReader((r, i) =>
        //        {
        //            if (i == 1)
        //            {
        //                var price = new PriceGroupPriceList();
        //                price.Id = r.GetValue<int>("PriceListId");
        //                price.Name = r.GetValue<string>("PriceListName");
        //                price.ListOrder = r.GetValue<int>("ListOrder");
        //                prices.Add(price);
        //            }
        //            if (i == 2)
        //            {
        //                result.Id = groupId;
        //                result.ActionCatalog = r.GetValue<bool>("ActionCatalog");
        //                result.ActionGroupTree = r.GetValue<bool>("ActionGroupTree");
        //                result.ActionArticles = r.GetValue<bool>("ActionArticles");
        //            }
        //            if (i == 3)
        //            {
        //                var catalogItem = new Catalog();
        //                catalogItem.Id = r.GetValue<int>("CatalogId");
        //                catalogItem.Name = r.GetValue<string>("CatalogName");
        //                catalogItems.Add(catalogItem);
        //            }
        //            if (i == 4)
        //            {
        //                var treeItem = new Tree();
        //                treeItem.Id = r.GetValue<int>("TreeId");
        //                treeItem.Name = r.GetValue<string>("TreeName");
        //                treeList.Add(treeItem);
        //            }
        //            if (i == 5)
        //            {
        //                var articleItem = new Article();
        //                articleItem.TreeId = r.GetValue<int>("TreeId");
        //                articleItem.CatalogName = r.GetValue<string>("CatalogName");
        //                articleItem.CatalogNumber = r.GetValue<string>("CatalogNumber");
        //                articleItem.Name = r.GetValue<string>("Name");
        //                articleList.Add(articleItem);
        //            }
        //        });
        //    }
        //    result.prices = prices;
        //    result.catalogs = catalogItems;
        //    result.trees = treeList;
        //    result.articles = articleList;

        //    return result;

        //}



        #region CatalogCRUD
        public List<Catalog> GetCatalogs(int groupId)
        {
            var catalogItems = new List<Catalog>();
            var query =
                "select c.Id as [CatalogId],c.Name as [CatalogName] from PriceGroupCatalogs pc join [Catalog] c on c.Id=pc.CatalogId where pc.GroupId=@groupId";
            using (var cmd = this.GetCommand(query))
            {
                cmd.AddParameter("groupId", groupId);

                cmd.ExecuteReader(r =>
                {
                    var item = new Catalog
                    {
                        Id = r.GetValue<int>("CatalogId"),
                        Name = r.GetValue<string>("CatalogName")
                    };
                    catalogItems.Add(item);
                });
            }

            return catalogItems;
        }

        public string DeleteCatalog(int groupId, int catalogId, string userId)
        {
            var message = "";
            var CatalogName = "";
            var GroupName = "";

            using (var cmd = this.GetCommand("PriceGroup_DeleteCatalog", true))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("groupId", groupId);
                cmd.AddParameter("catalogId", catalogId);
                try
                {
                    cmd.ExecuteReader(r =>
                    {
                        CatalogName = r.GetValue<string>("CatalogName");
                        GroupName = r.GetValue<string>("GroupName");
                    });
                }
                catch (Exception e)
                {
                    return e.InnerException.Message;
                }
            }

            new EventDbLogger(this.Connection).SaveLog(
            (ServiceFactory.GetPriceChangeEvent(groupId, GetManagerName(userId),
                new KeyValuePair<string, string>("catalog", CatalogName),
                "Удалён каталог " + CatalogName + " в выборке прайс группы: " + GroupName, "delete")));

            return message;

        }

        public string InsertCatalog(int groupId, int catalogId, string userId)
        {
            var message = "";
            var CatalogName = "";
            var GroupName = "";

            using (var cmd = this.GetCommand("PriceGroup_InsertCatalog", true))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("groupId", groupId);
                cmd.AddParameter("catalogId", catalogId);
                try
                {
                    cmd.ExecuteReader(r =>
                    {
                        CatalogName = r.GetValue<string>("CatalogName");
                        GroupName = r.GetValue<string>("GroupName");

                    });
                }
                catch (Exception e)
                {
                    return e.InnerException.Message;
                }
            }

            new EventDbLogger(this.Connection).SaveLog(
            (ServiceFactory.GetPriceChangeEvent(groupId, GetManagerName(userId),
                new KeyValuePair<string, string>("catalog", CatalogName),
                "Добавлен каталог " + CatalogName + " в выборке прайс группы: " + GroupName, "insert")));

            return message;
        }

        public List<Catalog> GetAllCatalogs(int groupId)
        {
            var result = new List<Catalog>();
            var query = "SELECT * FROM Catalog";
            using (var cmd = this.GetCommand(query))
            {
                cmd.ExecuteReader(r =>
                {
                    var item = new Catalog
                    {
                        Id = r.GetValue<int>("Id"),
                        Name = r.GetValue<string>("Name"),
                        KeyUrl = r.GetValue<string>("KeyUrl"),
                        IsEnabled = r.GetValue<bool>("IsEnabled"),
                        IsOriginal = r.GetValue<bool>("IsOriginal")
                    };
                    result.Add(item);
                });
            }

            if (groupId != 0)
            {
                var catalogItems = new List<Catalog>();
                var nquery =
                    "select c.Id as [CatalogId],c.Name as [CatalogName] from PriceGroupCatalogs pc join [Catalog] c on c.Id=pc.CatalogId where pc.GroupId=@groupId";
                using (var cmd = this.GetCommand(nquery))
                {
                    cmd.AddParameter("groupId", groupId);
                    cmd.ExecuteReader(r =>
                    {
                        var item = new Catalog
                        {
                            Id = r.GetValue<int>("CatalogId"),
                            Name = r.GetValue<string>("CatalogName")
                        };
                        catalogItems.Add(item);
                    });
                }

                IEnumerable<int> ids = from item in catalogItems
                                       select item.Id;
                result.RemoveAll(x => ids.Contains(x.Id));
            }


            return result;
        }
        #endregion

        #region PriceCRUD
        public List<PriceGroupPriceList> PriceList(int groupId)
        {
            var list = new List<PriceGroupPriceList>();
            using (var cmd = this.GetCommand("PriceGroupPriceListGet", true))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("GroupId", groupId);
                cmd.ExecuteReader(x => list.Add(new PriceGroupPriceList()
                {
                    Id = x.GetValue<int>("PriceListId"),
                    Name = x.GetValue<string>("PriceListName"),
                    ListOrder = x.GetValue<int>("ListOrder")
                }));
            }
            return list;
        }

        public string EditListOrder(int groupId, int priceListId, int listOrder)
        {
            var message = "";
            using (var cmd = this.GetCommand("PriceGroupPriceListSaveOrderList", true))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("GroupId", groupId);
                cmd.AddParameter("PriceListId", priceListId);
                cmd.AddParameter("ListOrder", listOrder);
                try
                {
                    cmd.Execute();
                }
                catch (Exception e)
                {
                    message = e.InnerException.Message;
                }
            }
            return message;
        }

        public string PriceListSave(int groupId, int priceListId, int listOrder, string userId)
        {
            var message = "";
            var GroupName = "";
            var PriceName = "";
            using (var cmd = this.GetCommand("PriceGroupPriceListSave", true))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("GroupId", groupId);
                cmd.AddParameter("PriceListId", priceListId);
                cmd.AddParameter("ListOrder", listOrder);

                try
                {
                    cmd.ExecuteReader(r =>
                    {
                        PriceName = r.GetValue<string>("PriceName");
                        GroupName = r.GetValue<string>("GroupName");
                    });
                }
                catch (Exception e)
                {
                    message = e.InnerException.Message;
                }
            }
            //отдать эвент методу: groupId, PriceName, message что именно

            new EventDbLogger(this.Connection).SaveLog(
            (ServiceFactory.GetPriceChangeEvent(groupId, GetManagerName(userId),
                new KeyValuePair<string, string>("price", PriceName),
                "Добавлен прайс " + PriceName + " в выборке прайс группы: " + GroupName, "insert")));

            return message;
        }

        public string DeletePriceList(int groupId, int priceListId, string userId)
        {
            var message = "";
            var GroupName = "";
            var PriceName = "";
            using (var cmd = this.GetCommand("PriceGroupPriceListDelete", true))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("GroupId", groupId);
                cmd.AddParameter("PriceListId", priceListId);
                try
                {
                    cmd.ExecuteReader(r =>
                    {
                        PriceName = r.GetValue<string>("PriceName");
                        GroupName = r.GetValue<string>("GroupName");
                    });
                }
                catch (Exception e)
                {
                    message = e.InnerException.Message;
                }
            }
            //отдать эвент методу: groupId, PriceName, message что именно
            new EventDbLogger(this.Connection).SaveLog(
            (ServiceFactory.GetPriceChangeEvent(groupId, GetManagerName(userId),
                new KeyValuePair<string, string>("price", PriceName),
                "Удалён прайс " + PriceName + " в выборке прайс группы: " + GroupName, "delete")));
            return message;
        }

        public List<PriceGroupPriceList> GetAllPrices(int groupId)
        {
            var allResult = new List<PriceGroupPriceList>();
            var allQuery =
               "SELECT * FROM PriceList";
            using (var cmd = this.GetCommand(allQuery))
            {
                cmd.ExecuteReader(x => allResult.Add(new PriceGroupPriceList()
                {
                    Id = x.GetValue<int>("Id"),
                    Name = x.GetValue<string>("Name"),
                    ListOrder = 0
                }));
            }
            if (groupId != 0)
            {
                var list = new List<PriceGroupPriceList>();
                using (var cmd = this.GetCommand("PriceGroupPriceListGet", true))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("GroupId", groupId);
                    cmd.ExecuteReader(x => list.Add(new PriceGroupPriceList()
                    {
                        Id = x.GetValue<int>("PriceListId"),
                        Name = x.GetValue<string>("PriceListName"),
                        ListOrder = x.GetValue<int>("ListOrder")
                    }));
                }
                IEnumerable<int> ids = from priceList in list
                                       select priceList.Id;

                allResult.RemoveAll(x => ids.Contains(x.Id));
            }


            return allResult;

        }

        #endregion

        #region Articles

        public List<Article> GetArticles(int groupId)
        {
            var vals = new List<Article>();

            var query =
                "select pga.ArticleId as Id, cat.Name as Catalog, an.Name as Name, a.Article as Article " +
                "from wiki.dbo.PriceGroupArticles pga left join wiki.dbo.Article a on a.Id = pga.ArticleId " +
                "left join wiki.dbo.ArticleNames an on pga.ArticleId = an.ArticleId " +
                "left join wiki.dbo.Catalog cat on a.Catalog = cat.Id where pga.GroupId = "+groupId;

            using (var cmd = this.GetCommand(query))
            {
                cmd.AddParameter("@groupId", groupId);
                cmd.ExecuteReader(x => vals.Add(new Article
                {
                    Id = x.GetValue<int>("Id"),
                    Catalog = x.GetValue<string>("Catalog"),
                    Name = x.GetValue<string>("Name"),
                    ArticleNumber = x.GetValue<string>("Article")
                }));
            }
            return vals;
        }

        public List<ArticleCatalogs> GetArticleCatalog(string key)
        {
            var vals = new List<ArticleCatalogs>();

            var query = @"SELECT c.Id, c.Name, c.KeyUrl, a.Article, cd.IsOurBrand, c.IsOriginal
                            FROM [Wiki].[dbo].[Catalog] c
                            join CatalogDesc cd on cd.Id = c.Id
                            join Article a on a.[Catalog] = c.Id
                            where a.Article = dbo.GetShortNum(@Str)
                            order by c.Name";
            using (var cmd = this.GetCommand(query))
            {
                cmd.AddParameter("Str", key);
                cmd.ExecuteReader(x => vals.Add(new ArticleCatalogs
                {
                    CatalogId = x.GetValue<int>("Id"),
                    Name = x.GetValue<string>("Name"),
                    KeyUrl = x.GetValue<string>("KeyUrl"),
                    Article = x.GetValue<string>("Article"),
                    IsOurBrand = x.GetValue<bool>("IsOurBrand"),
                    IsOriginal = x.GetValue<bool>("IsOriginal")
                }));
            }
            return vals;
        }

        public int SetArticleGroup(string article, int catalog, int groupId, string userId)
        {

            string query = @"select a.Id from [Catalog] c
                                join [Article] a on a.[Catalog] = c.Id
                                where c.Id = "+catalog+" and a.Article=dbo.GetShortNum("+article+")";

            var articleId = 0;

            using (var cmd = this.GetCommand(query))
            {
                articleId = Int32.Parse(cmd.ExecuteValue().ToString());
            }

            query = "insert into wiki.dbo.PriceGroupArticles(GroupId, ArticleId) values ("+groupId+", "+articleId+")";

            new EventDbLogger(this.Connection).SaveLog(
(ServiceFactory.GetPriceChangeEvent(groupId, GetManagerName(userId),
    new KeyValuePair<string, string>("article", article),
    "Добавлен артикул " + article + " к выборке прайс группы: " + groupId, "insert")));

            using (var cmd = this.GetCommand(query))
            {
                return cmd.Execute();
            }

        }

        public int DeleteArticle(int id, int groupId, string userId)
        {
            var query = "delete from wiki.dbo.PriceGroupArticles where GroupId = @groupId and ArticleId = @id";

            new EventDbLogger(this.Connection).SaveLog(
(ServiceFactory.GetPriceChangeEvent(groupId, GetManagerName(userId),
new KeyValuePair<string, string>("article", id.ToString()),
"Удалён артикул " + id.ToString() + "в выборке прайс группы: " + groupId, "delete")));

            using (var cmd = this.GetCommand(query))
            {
                cmd.AddParameter("groupId", groupId);
                cmd.AddParameter("id", id);
                return cmd.Execute();
            }
        }
        #endregion

        #region TreeView

        public string InsertTree(int groupId, int treeId, string userId)
        {
            var message = "";
            var TreeName = "";
            var GroupName = "";
            using (var cmd = this.GetCommand("PriceGroup_InsertCategoryTree", true))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("groupId", groupId);
                cmd.AddParameter("treeId", treeId);
                try
                {
                    cmd.ExecuteReader(r =>
                    {
                        TreeName = r.GetValue<string>("TreeName");
                        GroupName = r.GetValue<string>("GroupName");
                    });
                }
                catch (Exception e)
                {
                    return e.InnerException.Message;
                }
            }

            new EventDbLogger(this.Connection).SaveLog(
            (ServiceFactory.GetPriceChangeEvent(groupId, GetManagerName(userId),
                new KeyValuePair<string, string>("tree", TreeName),
                "Добавлена категория " + TreeName + " в выборке прайс группы: " + GroupName, "insert")));

            return message;

        }

        public List<int> GetSelectedNodesTree(int groupId)
        {
            var list = new List<int>();
            var query = "SELECT * FROM PriceGroupTree WHERE GroupId = @groupId";

            using (var cmd = this.GetCommand(query))
            {
                cmd.AddParameter("groupId", groupId);
                cmd.ExecuteReader(r =>
                {
                    int a = r.GetValue<int>("TreeId");
                    list.Add(a);
                });
            }
            return list;
        }

        public string DeleteNodeTree(int groupId, int treeId, string userId)
        {
            var message = "";
            var TreeName = "";
            var GroupName = "";
            using (var cmd = this.GetCommand("PriceGroup_DeleteCategoryTree", true))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("groupId", groupId);
                cmd.AddParameter("treeId", treeId);
                try
                {
                    cmd.ExecuteReader(r =>
                    {
                        TreeName = r.GetValue<string>("TreeName");
                        GroupName = r.GetValue<string>("GroupName");
                    });
                }
                catch (Exception e)
                {
                    return e.InnerException.Message;
                }
            }
            new EventDbLogger(this.Connection).SaveLog(
                (ServiceFactory.GetPriceChangeEvent(groupId, GetManagerName(userId),
                    new KeyValuePair<string, string>("tree", TreeName),
                    "Удалена категория " + TreeName + " в выборке прайс группы: " + GroupName, "delete")));
            return message;

        }

        #endregion

        #region radioValues

        public RadioButtons GetRadioButtonsValues(int groupId)
        {
            var buttons = new RadioButtons();
            var command =
                "SELECT pg.ActionArticles, pg.ActionCatalog, pg.ActionGroupTree FROM Wiki.dbo.PriceGroup pg where pg.Id = @groupId";
            using (var cmd = this.GetCommand(command))
            {
                cmd.AddParameter("groupId", groupId);
                cmd.ExecuteReader(r =>
                {
                    buttons.ActionArticles = r.GetValue<bool>("ActionArticles");
                    buttons.ActionCatalog = r.GetValue<bool>("ActionCatalog");
                    buttons.ActionGroupTree = r.GetValue<bool>("ActionGroupTree");
                });

            }
            return buttons;
        }

        public string SetActionArticles(int groupId, bool value, string userId)
        {
            var message = "";
            var GroupName = GetPriceName(groupId);

            const string command = "UPDATE Wiki.dbo.PriceGroup SET ActionArticles=@value WHERE Id=@groupId";

            using (var cmd = this.GetCommand(command))
            {
                cmd.AddParameter("groupId", groupId);
                cmd.AddParameter("value", value ? 1 : 0);

                try
                {
                    cmd.Execute();
                }
                catch (Exception e)
                {
                    message = e.InnerException.Message;
                }
            }


            string Strvalue = value ? "Включены" : "Исключены";
            new EventDbLogger(this.Connection).SaveLog(
            (ServiceFactory.GetPriceChangeEvent(groupId, GetManagerName(userId),
                new KeyValuePair<string, string>("article", value.ToString()),
                Strvalue + " выбранные артикулы в выборке прайс группы: " + GroupName, value ? "insert" : "delete")));

            return message;
        }

        public string SetActionCatalog(int groupId, bool value, string userId)
        {
            var message = "";
            var GroupName = GetPriceName(groupId);

            var command = "UPDATE Wiki.dbo.PriceGroup SET ActionCatalog=@value WHERE Id=@groupId";

            using (var cmd = this.GetCommand(command))
            {
                cmd.AddParameter("groupId", groupId);
                cmd.AddParameter("value", value ? 1 : 0);

                try
                {
                    cmd.Execute();
                }
                catch (Exception e)
                {
                    message = e.InnerException.Message;
                }
            }
            //отдать эвент методу: groupId, PriceName, message что именно
            string Strvalue = value ? "Включены" : "Исключены";
            new EventDbLogger(this.Connection).SaveLog(
            (ServiceFactory.GetPriceChangeEvent(groupId, GetManagerName(userId),
                new KeyValuePair<string, string>("catalogs", value.ToString()),
                Strvalue + " выбранные каталоги в выборке прайс группы: " + GroupName, value ? "insert" : "delete")));

            return message;
        }

        public string SetActionGroupTree(int groupId, bool value, string userId)
        {
            var message = "";
            var GroupName = GetPriceName(groupId);

            var command = "UPDATE Wiki.dbo.PriceGroup SET ActionGroupTree=@value WHERE Id=@groupId";

            using (var cmd = this.GetCommand(command))
            {
                cmd.AddParameter("groupId", groupId);
                cmd.AddParameter("value", value ? 1 : 0);

                try
                {
                    cmd.Execute();
                }
                catch (Exception e)
                {
                    message = e.InnerException.Message;
                }
            }

            string Strvalue = value ? "Включены" : "Исключены";
            new EventDbLogger(this.Connection).SaveLog(
            (ServiceFactory.GetPriceChangeEvent(groupId, GetManagerName(userId),
                new KeyValuePair<string, string>("trees", value.ToString()),
                Strvalue + " выбранные категории в выборке прайс группы: " + GroupName, value ? "insert" : "delete")));

            return message;
            //отдать эвент методу: groupId, PriceName, message что именно

        }

        #endregion

        #region Clients

        public List<PriceGroupToClient> GetClientsInfo(int groupId)
        {
            var result = new List<PriceGroupToClient>();

            var query = "select [Id],[PriceGroupId],[ManagerName],[ClientId],[ClientCode],[ClientName],[FromEmail],[ToEmail],[DaysSend],[TimeSend],[FileName],[FileType],[IsEnabled],[MailSubject],[ProfileId],[MailBody],[FileConfig] from Wiki.dbo.PriceGroupToClientView WHERE PriceGroupId = @groupId ";


            using (var cmd = this.GetCommand(query))
            {
                cmd.AddParameter("groupId", groupId);

                cmd.ExecuteReader(x => result.Add(new PriceGroupToClient()
                {
                    Id = x.GetValue<int>("Id"),
                    GroupId = x.GetValue<int>("PriceGroupId"),
                    ManagerName = x.GetValue<string>("ManagerName"),
                    ClientId = x.GetValue<int>("ClientId"),
                    ClientCode = x.GetValue<int>("ClientCode"),
                    ClientName = x.GetValue<string>("ClientName").TrimEnd(),
                    FromEmail = x.GetValue<string>("FromEmail"),
                    ToEmail = x.GetValue<string>("ToEmail"),
                    IsEnabled = x.GetValue<bool>("IsEnabled"),
                    MailSubject = x.GetValue<string>("MailSubject"),
                    FileName = x.GetValue<string>("FileName"),
                    ProfileId = x.GetValue<int>("ProfileId"),
                    TimeSend = x.GetValue<string>("TimeSend").Split(';'),
                    DaysSend = x.GetValue<string>("DaysSend").ToCharArray(),
                    FileType = x.GetValue<string>("FileType"),
                    MailBody = x.GetValue<string>("MailBody"),
                    FileConfig = x.GetValue<string>("FileConfig")
                }));
            }

            return result;
        }
        public PriceGroupToClient SaveClientInfo(PriceSendModel model, string userId)
        {
            PriceGroupToClient val = new PriceGroupToClient();
            using (var cmd = this.GetCommand("PriceGroupToClientSave"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("clientId", model.ClientId);
                cmd.AddParameter("email", model.Email);
                cmd.AddParameter("daysSend", model.DaysSend);
                cmd.AddParameter("timeSend", model.TimesSend);
                cmd.AddParameter("fileName", model.FileName);
                cmd.AddParameter("fileConfig", model.FileConfig);
                cmd.AddParameter("fileType", model.FileType);
                cmd.AddParameter("isEnabled", model.IsEnabled);
                cmd.AddParameter("mailSubject", model.Subject);
                cmd.AddParameter("mailBody", model.Body);
                cmd.AddParameter("profileId", model.EmailSetting.Id);
                cmd.AddParameter("groupId", model.GroupId);
                cmd.ExecuteReader(x =>
                {
                    val.Id = x.GetValue<int>("Id");
                    val.GroupId = x.GetValue<int>("PriceGroupId");
                    val.ManagerName = x.GetValue<string>("ManagerName");
                    val.ClientId = x.GetValue<int>("ClientId");
                    val.ClientCode = x.GetValue<int>("ClientCode");
                    val.ClientName = x.GetValue<string>("ClientName").TrimEnd();
                    val.FromEmail = x.GetValue<string>("FromEmail");
                    val.ToEmail = x.GetValue<string>("ToEmail");
                    val.IsEnabled = x.GetValue<bool>("IsEnabled");
                    val.MailSubject = x.GetValue<string>("MailSubject");
                    val.FileName = x.GetValue<string>("FileName");
                    val.ProfileId = x.GetValue<int>("ProfileId");
                    val.TimeSend = x.GetValue<string>("TimeSend").Split(';');
                    val.DaysSend = x.GetValue<string>("DaysSend").ToCharArray();
                    val.FileType = x.GetValue<string>("FileType");
                    val.MailBody = x.GetValue<string>("MailBody");
                    val.FileConfig = x.GetValue<string>("FileConfig");
                });
            }

            new EventDbLogger(this.Connection).SaveLog((Helpers.Helper.CalculateClientUpdates(model, val, GetManagerName(userId))));

            return val;
        }

        public PriceGroupToClient CreateClientInfo(PriceSendModel model, string userId)
        {
            PriceGroupToClient val = new PriceGroupToClient();

            using (var cmd = this.GetCommand("PriceGroupToClientCreate"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("clientId", model.ClientId);
                cmd.AddParameter("email", model.Email);
                cmd.AddParameter("daysSend", model.DaysSend);
                cmd.AddParameter("timeSend", model.TimesSend);
                cmd.AddParameter("fileName", model.FileName);
                cmd.AddParameter("fileConfig", model.FileConfig);
                cmd.AddParameter("fileType", model.FileType);
                cmd.AddParameter("isEnabled", model.IsEnabled);
                cmd.AddParameter("mailSubject", model.Subject);
                cmd.AddParameter("mailBody", model.Body);
                cmd.AddParameter("profileId", model.EmailSetting);
                cmd.AddParameter("groupId", model.GroupId);

                cmd.ExecuteReader(x =>
                {
                    val.Id = x.GetValue<int>("Id");
                    val.GroupId = x.GetValue<int>("PriceGroupId");
                    val.ManagerName = x.GetValue<string>("ManagerName");
                    val.ClientId = x.GetValue<int>("ClientId");
                    val.ClientCode = x.GetValue<int>("ClientCode");
                    val.ClientName = x.GetValue<string>("ClientName").TrimEnd();
                    val.FromEmail = x.GetValue<string>("FromEmail");
                    val.ToEmail = x.GetValue<string>("ToEmail");
                    val.IsEnabled = x.GetValue<bool>("IsEnabled");
                    val.MailSubject = x.GetValue<string>("MailSubject");
                    val.FileName = x.GetValue<string>("FileName");
                    val.ProfileId = x.GetValue<int>("ProfileId");
                    val.TimeSend = x.GetValue<string>("TimeSend").Split(';');
                    val.DaysSend = x.GetValue<string>("DaysSend").ToCharArray();
                    val.FileType = x.GetValue<string>("FileType");
                    val.MailBody = x.GetValue<string>("MailBody");
                    val.FileConfig = x.GetValue<string>("FileConfig");
                });
            }
            new EventDbLogger(this.Connection).SaveLog((Helpers.Helper.CalculateClientCreate(val, GetManagerName(userId))));
            return val;

        }

        public string DeleteClientInfo(int id, string userId)
        {
            var message = "";
            var val = new PriceGroupToClient();
            using (var cmd = this.GetCommand("PriceGroupToClientDelete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("id", id);
                try
                {
                    cmd.ExecuteReader(x =>
                    {
                        val.Id = x.GetValue<int>("Id");
                        val.GroupId = x.GetValue<int>("PriceGroupId");
                        val.ManagerName = x.GetValue<string>("ManagerName");
                        val.ClientId = x.GetValue<int>("ClientId");
                        val.ClientCode = x.GetValue<int>("ClientCode");
                        val.ClientName = x.GetValue<string>("ClientName").TrimEnd();
                        val.FromEmail = x.GetValue<string>("FromEmail");
                        val.ToEmail = x.GetValue<string>("ToEmail");
                        val.IsEnabled = x.GetValue<bool>("IsEnabled");
                        val.MailSubject = x.GetValue<string>("MailSubject");
                        val.FileName = x.GetValue<string>("FileName");
                        val.ProfileId = x.GetValue<int>("ProfileId");
                        val.TimeSend = x.GetValue<string>("TimeSend").Split(';');
                        val.DaysSend = x.GetValue<string>("DaysSend").ToCharArray();
                        val.FileType = x.GetValue<string>("FileType");
                        val.MailBody = x.GetValue<string>("MailBody");
                        val.FileConfig = x.GetValue<string>("FileConfig");
                    });
                }
                catch (Exception e)
                {
                    return e.InnerException.Message;
                }
            }

            //эвент удаления
            var list = new List<KeyValuePair<string, string>>();
            var res = new KeyValuePair<string, string>("ClientCode", val.ClientCode.ToString());
            list.Add(res);
            new EventDbLogger(this.Connection).SaveLog((ServiceFactory.GetPriceDeleteClientEvent(val.GroupId, list, GetManagerName(userId),
                "Удален клиент: " + val.ClientCode, "delete")));
            return message;
        }

        public List<Client> GetClientIdFilter(int nums)
        {
            var result = new List<Client>();
            var query = "select top 30 * from Client c where cast(c.ClientCode as varchar(5)) like '" + nums + "%'";


            using (var cmd = this.GetCommand(query))
            {
                cmd.ExecuteReader(x => result.Add(new Client()
                {
                    Id = x.GetValue<int>("Id"),
                    Name = x.GetValue<string>("Name"),
                    ClientCode = x.GetValue<int>("ClientCode"),
                    Company = x.GetValue<int>("Company"),
                    Email = x.GetValue<string>("Email")
                }));
            }
            return result;
        }

        public List<Client> GetClientNameFilter(string str)
        {
            var result = new List<Client>();
            var query = "select top 30 * from Client c where c.Name like '" + str + "%'";

            using (var cmd = this.GetCommand(query))
            {
                cmd.ExecuteReader(x => result.Add(new Client()
                {
                    Name = x.GetValue<string>("Name"),
                    ClientCode = x.GetValue<int>("ClientCode"),
                    Company = x.GetValue<int>("Company"),
                    Email = x.GetValue<string>("Email")
                }));
            }
            return result;
        }
        #endregion

        #region History

        public List<ClientChangeModelResponse> GetClientsCreated(ClientChangeModelRequest request)
        {
            //9005
            var query = "";

            if (request.Sort == null)
            {
                query = "select e.id, e.Stop as time, " +
                    "max(case when ei.Name = 'managerName' then ei.Value end) as managerName, " +
                    "max(case when ei.Name = 'type' then ei.Value  end) as type, " +
                    "max(case when ei.Name = 'groupId' then ei.Value  end) as groupId, " +
                    "max(case when ei.Name = 'message' then ei.Value  end) as message, " +
                    "max(case when ei.Name = 'DaysSend' then ei.Value end) as daysSend, " +
                    "max(case when ei.Name = 'FileConfig' then ei.Value end) as fileConfig, " +
                    "max(case when ei.Name = 'TimeSend' then ei.Value end) as timeSend, " +
                    "max(case when ei.Name = 'MailBody' then ei.Value end) as mailBody, " +
                    "max(case when ei.Name = 'MailSubject' then ei.Value end) as mailSubject, " +
                    "max(case when ei.Name = 'FileName' then ei.Value end) as fileName, " +
                    "max(case when ei.Name = 'ToEmail' then ei.Value end) as toEmail " +
                    "FROM[Wiki].[dbo].[Events] e JOIN[Wiki].[dbo].[EventParams] ei on e.id = ei.EventId WHERE e.Code = 9005 " +
                    "group by e.id, e.Stop order by e.Stop asc OFFSET " + request.Skip + " ROWS FETCH NEXT 100 ROWS ONLY";
            }
            else
            {
                query = "select e.id, e.Stop as time, " +
                    "max(case when ei.Name = 'managerName' then ei.Value end) as managerName, " +
                    "max(case when ei.Name = 'type' then ei.Value  end) as type, " +
                    "max(case when ei.Name = 'groupId' then ei.Value  end) as groupId, " +
                    "max(case when ei.Name = 'message' then ei.Value  end) as message, " +
                    "max(case when ei.Name = 'DaysSend' then ei.Value end) as daysSend, " +
                    "max(case when ei.Name = 'FileConfig' then ei.Value end) as fileConfig, " +
                    "max(case when ei.Name = 'TimeSend' then ei.Value end) as timeSend, " +
                    "max(case when ei.Name = 'MailBody' then ei.Value end) as mailBody, " +
                    "max(case when ei.Name = 'MailSubject' then ei.Value end) as mailSubject, " +
                    "max(case when ei.Name = 'FileName' then ei.Value end) as fileName, " +
                    "max(case when ei.Name = 'ToEmail' then ei.Value end) as toEmail " +
                    "FROM[Wiki].[dbo].[Events] e JOIN[Wiki].[dbo].[EventParams] ei on e.id = ei.EventId WHERE e.Code = 9005 " +
                    "group by e.id, e.Stop order by " + request.Sort[0].Field + " " + request.Sort[0].Dir + " OFFSET " + request.Skip + " ROWS FETCH NEXT " + request.Take + " ROWS ONLY";
            }
            return GetHistoryClient(query, 9005);
        }

        public List<ClientChangeModelResponse> GetClientsDeleted(ClientChangeModelRequest request)
        {
            //9006
            var query = "";

            if (request.Sort == null)
            {
                query = "select e.id, e.Stop as time, " +
                    "max(case when ei.Name = 'managerName' then ei.Value end) as managerName, " +
                    "max(case when ei.Name = 'type' then ei.Value  end) as type, " +
                    "max(case when ei.Name = 'groupId' then ei.Value  end) as groupId, " +
                    "max(case when ei.Name = 'message' then ei.Value  end) as message, " +
                    "max(case when ei.Name = 'DaysSend' then ei.Value end) as daysSend, " +
                    "max(case when ei.Name = 'FileConfig' then ei.Value end) as fileConfig, " +
                    "max(case when ei.Name = 'TimeSend' then ei.Value end) as timeSend, " +
                    "max(case when ei.Name = 'MailBody' then ei.Value end) as mailBody, " +
                    "max(case when ei.Name = 'MailSubject' then ei.Value end) as mailSubject, " +
                    "max(case when ei.Name = 'FileName' then ei.Value end) as fileName, " +
                    "max(case when ei.Name = 'ToEmail' then ei.Value end) as toEmail " +
                    "FROM[Wiki].[dbo].[Events] e JOIN[Wiki].[dbo].[EventParams] ei on e.id = ei.EventId WHERE e.Code = 9006 " +
                    "group by e.id, e.Stop order by e.Stop asc OFFSET " + request.Skip + " ROWS FETCH NEXT 100 ROWS ONLY";
            }
            else
            {
                query = "select e.id, e.Stop as time, " +
                    "max(case when ei.Name = 'managerName' then ei.Value end) as managerName, " +
                    "max(case when ei.Name = 'type' then ei.Value  end) as type, " +
                    "max(case when ei.Name = 'groupId' then ei.Value  end) as groupId, " +
                    "max(case when ei.Name = 'message' then ei.Value  end) as message, " +
                    "max(case when ei.Name = 'DaysSend' then ei.Value end) as daysSend, " +
                    "max(case when ei.Name = 'FileConfig' then ei.Value end) as fileConfig, " +
                    "max(case when ei.Name = 'TimeSend' then ei.Value end) as timeSend, " +
                    "max(case when ei.Name = 'MailBody' then ei.Value end) as mailBody, " +
                    "max(case when ei.Name = 'MailSubject' then ei.Value end) as mailSubject, " +
                    "max(case when ei.Name = 'FileName' then ei.Value end) as fileName, " +
                    "max(case when ei.Name = 'ToEmail' then ei.Value end) as toEmail " +
                    "FROM[Wiki].[dbo].[Events] e JOIN[Wiki].[dbo].[EventParams] ei on e.id = ei.EventId WHERE e.Code = 9006 " +
                    "group by e.id, e.Stop order by " + request.Sort[0].Field + " " + request.Sort[0].Dir + " OFFSET " + request.Skip + " ROWS FETCH NEXT " + request.Take + " ROWS ONLY";
            }
            return GetHistoryClient(query, 9006);

        }

        public List<ClientChangeModelResponse> GetClientHistory(ClientChangeModelRequest request)
        {
            //9004
            var query = "";

            if (request.Sort == null)
            {
                query = "select e.id, e.Stop as time, " +
                    "max(case when ei.Name = 'managerName' then ei.Value end) as managerName, " +
                    "max(case when ei.Name = 'type' then ei.Value  end) as type, " +
                    "max(case when ei.Name = 'groupId' then ei.Value  end) as groupId, " +
                    "max(case when ei.Name = 'message' then ei.Value  end) as message, " +
                    "max(case when ei.Name = 'DaysSend' then ei.Value end) as daysSend, " +
                    "max(case when ei.Name = 'FileConfig' then ei.Value end) as fileConfig, " +
                    "max(case when ei.Name = 'TimeSend' then ei.Value end) as timeSend, " +
                    "max(case when ei.Name = 'MailBody' then ei.Value end) as mailBody, " +
                    "max(case when ei.Name = 'MailSubject' then ei.Value end) as mailSubject, " +
                    "max(case when ei.Name = 'FileName' then ei.Value end) as fileName, " +
                    "max(case when ei.Name = 'ToEmail' then ei.Value end) as toEmail " +
                    "FROM[Wiki].[dbo].[Events] e JOIN[Wiki].[dbo].[EventParams] ei on e.id = ei.EventId WHERE e.Code = 9004 " +
                    "group by e.id, e.Stop order by e.Stop asc OFFSET " + request.Skip + " ROWS FETCH NEXT 100 ROWS ONLY";
            }
            else
            {
                query = "select e.id, e.Stop as time, " +
                    "max(case when ei.Name = 'managerName' then ei.Value end) as managerName, " +
                    "max(case when ei.Name = 'type' then ei.Value  end) as type, " +
                    "max(case when ei.Name = 'groupId' then ei.Value  end) as groupId, " +
                    "max(case when ei.Name = 'message' then ei.Value  end) as message, " +
                    "max(case when ei.Name = 'DaysSend' then ei.Value end) as daysSend, " +
                    "max(case when ei.Name = 'FileConfig' then ei.Value end) as fileConfig, " +
                    "max(case when ei.Name = 'TimeSend' then ei.Value end) as timeSend, " +
                    "max(case when ei.Name = 'MailBody' then ei.Value end) as mailBody, " +
                    "max(case when ei.Name = 'MailSubject' then ei.Value end) as mailSubject, " +
                    "max(case when ei.Name = 'FileName' then ei.Value end) as fileName, " +
                    "max(case when ei.Name = 'ToEmail' then ei.Value end) as toEmail " +
                    "FROM[Wiki].[dbo].[Events] e JOIN[Wiki].[dbo].[EventParams] ei on e.id = ei.EventId WHERE e.Code = 9004 " +
                    "group by e.id, e.Stop order by " + request.Sort[0].Field + " " + request.Sort[0].Dir + " OFFSET " + request.Skip + " ROWS FETCH NEXT " + request.Take + " ROWS ONLY";
            }
            return GetHistoryClient(query, 9004);
        }

        public List<PriceChangeModelResponse> GetPriceHistory(PriceChangeModelRequest request)
        {

            var query = "";

            if (request.Sort == null)
            {
                query = "select e.id, e.Stop as time, " +
                    "max(case when ei.Name = 'managerName' then ei.Value end) as managerName, " +
                    "max(case when ei.Name = 'catalog' or ei.Name = 'price' or ei.Name = 'tree' then ei.Name end) as operation, " +
                    "max(case when ei.Name = 'catalog' or ei.Name = 'price' or ei.Name = 'tree' then ei.Value end) as value, " +
                    "max(case when ei.Name = 'type' then ei.Value  end) as type, " +
                    "max(case when ei.Name = 'groupId' then ei.Value  end) as groupId, " +
                    "max(case when ei.Name = 'message' then ei.Value  end) as message " +
                    "FROM [Wiki].[dbo].[Events] e JOIN[Wiki].[dbo].[EventParams] ei on e.id = ei.EventId " +
                    "WHERE e.Code = 9003 group by e.id, e.Stop order by time asc OFFSET " + request.Skip + " ROWS FETCH NEXT 100 ROWS ONLY";
            }

            else
            {
                query = "select e.id, e.Stop as time, " +
                        "max(case when ei.Name = 'managerName' then ei.Value end) as managerName, " +
                        "max(case when ei.Name = 'catalog' or ei.Name = 'price' or ei.Name = 'tree' then ei.Name end) as operation, " +
                        "max(case when ei.Name = 'catalog' or ei.Name = 'price' or ei.Name = 'tree' then ei.Value end) as value, " +
                        "max(case when ei.Name = 'type' then ei.Value  end) as type, " +
                        "max(case when ei.Name = 'groupId' then ei.Value  end) as groupId, " +
                        "max(case when ei.Name = 'message' then ei.Value  end) as message " +
                        "FROM [Wiki].[dbo].[Events] e JOIN[Wiki].[dbo].[EventParams] ei on e.id = ei.EventId " +
                        "WHERE e.Code = 9003 group by e.id, e.Stop order by " + request.Sort[0].Field + " " + request.Sort[0].Dir + " OFFSET " + request.Skip + " ROWS FETCH NEXT " + request.Take + " ROWS ONLY";
            }

            return GetHistoryPrice(query);

        }


        public List<PriceSendModelResponse> GetSendHistory(PriceSendModelRequest request)
        {
            var query = "";

            if (request.Sort == null)
            {
                query = "select e.id, " +
                    "max(case when ei.Name = 'sendTime' then ei.Value end) as sendTime, " +
                    "max(case when ei.Name = 'nextTime' then ei.Value  end) as nextTime, " +
                    "max(case when ei.Name = 'emailTo' then ei.Value  end) as emailTo, " +
                    "max(case when ei.Name = 'profileId' then ei.Value  end) as profileId, " +
                    "max(case when ei.Name = 'clientId' then ei.Value end) as clientId, " +
                    "max(case when ei.Name = 'groupId' then ei.Value end) as groupId, " +
                    "max(case when ei.Name = 'fileName' then ei.Value end) as fileName, " +
                    "max(case when ei.Name = 'filePath' then ei.Value end) as filePath, " +
                    "max(case when ei.Name = 'emailFrom' then ei.Value end) as emailFrom " +
                    "FROM[Wiki].[dbo].[Events] e JOIN[Wiki].[dbo].[EventParams] ei on e.id = ei.EventId WHERE e.Code = 8999 " +
                    "group by e.id, e.Stop order by e.Stop asc OFFSET " + request.Skip + " ROWS FETCH NEXT 100 ROWS ONLY";
            }
            else
            {
                query = "select e.id, " +
                    "max(case when ei.Name = 'sendTime' then ei.Value end) as sendTime, " +
                    "max(case when ei.Name = 'nextTime' then ei.Value  end) as nextTime, " +
                    "max(case when ei.Name = 'emailTo' then ei.Value  end) as emailTo, " +
                    "max(case when ei.Name = 'profileId' then ei.Value  end) as profileId, " +
                    "max(case when ei.Name = 'clientId' then ei.Value end) as clientId, " +
                    "max(case when ei.Name = 'groupId' then ei.Value end) as groupId, " +
                    "max(case when ei.Name = 'fileName' then ei.Value end) as fileName, " +
                    "max(case when ei.Name = 'filePath' then ei.Value end) as filePath, " +
                    "max(case when ei.Name = 'emailFrom' then ei.Value end) as emailFrom " +
                    "FROM[Wiki].[dbo].[Events] e JOIN[Wiki].[dbo].[EventParams] ei on e.id = ei.EventId WHERE e.Code = 8999 " +
                    "group by e.id, e.Stop order by " + request.Sort[0].Field + " " + request.Sort[0].Dir + " OFFSET " + request.Skip + " ROWS FETCH NEXT " + request.Take + " ROWS ONLY";
            }
            return GetHistorySend(query, 8999);
        }

        public List<PriceSendErrorModelResponse> GetSendErrorHistory(PriceSendErrorModelRequest request)
        {
            var query = "";

            if (request.Sort == null)
            {
                query = "select e.id, e.Stop as time, " +
                    "max(case when ei.Name = 'erorMessage' then ei.Value end) as erorMessage, " +
                    "max(case when ei.Name = 'profileId' then ei.Value  end) as profileId, " +
                    "max(case when ei.Name = 'emailTo' then ei.Value  end) as emailTo, " +
                    "max(case when ei.Name = 'clientId' then ei.Value  end) as clientId, " +
                    "max(case when ei.Name = 'groupId' then ei.Value end) as groupId " +
                    "FROM[Wiki].[dbo].[Events] e JOIN[Wiki].[dbo].[EventParams] ei on e.id = ei.EventId WHERE e.Code = 8998 " +
                    "group by e.id, e.Stop order by e.Stop asc OFFSET " + request.Skip + " ROWS FETCH NEXT 100 ROWS ONLY";
            }
            else
            {
                query = "select e.id, e.Stop as time, " +
                    "max(case when ei.Name = 'erorMessage' then ei.Value end) as erorMessage, " +
                    "max(case when ei.Name = 'profileId' then ei.Value  end) as profileId, " +
                    "max(case when ei.Name = 'emailTo' then ei.Value  end) as emailTo, " +
                    "max(case when ei.Name = 'clientId' then ei.Value  end) as clientId, " +
                    "max(case when ei.Name = 'groupId' then ei.Value end) as groupId " +
                    "FROM[Wiki].[dbo].[Events] e JOIN[Wiki].[dbo].[EventParams] ei on e.id = ei.EventId WHERE e.Code = 8998 " +
                    "group by e.id, e.Stop order by " + request.Sort[0].Field + " " + request.Sort[0].Dir + " OFFSET " + request.Skip + " ROWS FETCH NEXT " + request.Take + " ROWS ONLY";
            }
            return GetHistoryErrorSend(query, 8998);
        }

        private List<PriceChangeModelResponse> GetHistoryPrice(string query)
        {
            var result = new List<PriceChangeModelResponse>();

            using (var cmd = this.GetCommand(query))
            {
                cmd.ExecuteReader(r => result.Add(new PriceChangeModelResponse
                {
                    GroupId = Int32.Parse(r.GetValue<string>("groupId")),
                    ManagerName = r.GetValue<string>("managerName"),
                    Message = r.GetValue<string>("message"),
                    Time = r.GetValue<DateTime>("time").ToString(CultureInfo.InvariantCulture),
                    Type = r.GetValue<string>("type"),
                    Value = new KeyValuePair<string, string>(r.GetValue<string>("operation"), r.GetValue<string>("value"))
                }));
            }

            var countQuery = "select count(*) as count FROM [Wiki].[dbo].[Events] WHERE Code = 9003 ";

            var count = 0;
            using (var cmd = this.GetCommand(countQuery))
            {
                cmd.ExecuteReader(r =>
                {
                    count = r.GetValue<int>("count");
                });
            }

            foreach (var price in result)
            {
                price.total = count;
            }

            return result;
        }


        private List<ClientChangeModelResponse> GetHistoryClient(string query, int eventCode)
        {
            var result = new List<ClientChangeModelResponse>();

            using (var cmd = this.GetCommand(query))
            {
                cmd.ExecuteReader(r => result.Add(new ClientChangeModelResponse
                {
                    GroupId = Int32.Parse(r.GetValue<string>("groupId")),
                    ManagerName = r.GetValue<string>("managerName"),
                    Message = r.GetValue<string>("message"),
                    Time = r.GetValue<DateTime>("time").ToString(CultureInfo.InvariantCulture),
                    Type = r.GetValue<string>("type"),
                    MailBody = r.GetValue<string>("mailBody"),
                    MailSubject = r.GetValue<string>("mailSubject"),
                    DaysSend = r.GetValue<string>("daysSend"),
                    FileName = r.GetValue<string>("fileName"),
                    FileConfig = r.GetValue<string>("fileConfig"),
                    ToEmail = r.GetValue<string>("toEmail"),
                    TimeSend = r.GetValue<string>("timeSend")
                }));
            }

            var countQuery = "select count(*) as count FROM [Wiki].[dbo].[Events] WHERE Code = " + eventCode;

            var count = 0;
            using (var cmd = this.GetCommand(countQuery))
            {
                cmd.ExecuteReader(r =>
                {
                    count = r.GetValue<int>("count");
                });
            }

            foreach (var price in result)
            {
                price.total = count;
            }

            return result;
        }

        private List<PriceSendErrorModelResponse> GetHistoryErrorSend(string query, int eventCode)
        {
            var result = new List<PriceSendErrorModelResponse>();
            using (var cmd = this.GetCommand(query))
            {
                cmd.ExecuteReader(r => result.Add(new PriceSendErrorModelResponse
                {
                    Message = r.GetValue<string>("erorMessage"),
                    ClientId = r.GetValue<string>("clientId"),
                    GroupId = r.GetValue<string>("groupId"),
                    EmailTo = r.GetValue<string>("emailTo"),
                    ProfileId = r.GetValue<string>("profileId"),
                    Time = r.GetValue<DateTime>("time")
                }));
            }
            var countQuery = "select count(*) as count FROM [Wiki].[dbo].[Events] WHERE Code = " + eventCode;

            var count = 0;
            using (var cmd = this.GetCommand(countQuery))
            {
                cmd.ExecuteReader(r =>
                {
                    count = r.GetValue<int>("count");
                });
            }

            foreach (var price in result)
            {
                price.total = count;
            }

            return result;
        }

        private List<PriceSendModelResponse> GetHistorySend(string query, int eventCode)
        {
            var result = new List<PriceSendModelResponse>();

            using (var cmd = this.GetCommand(query))
            {
                cmd.ExecuteReader(r => result.Add(new PriceSendModelResponse
                {
                    GroupId = r.GetValue<string>("groupId"),
                    FileName = r.GetValue<string>("fileName"),
                    ClientId = r.GetValue<string>("clientId"),
                    EmailFrom = r.GetValue<string>("emailFrom"),
                    EmailTo = r.GetValue<string>("emailTo"),
                    ProfileId = r.GetValue<string>("profileId"),
                    FilePath = r.GetValue<string>("filePath"),
                    NextTime = r.GetValue<string>("nextTime"),
                    SendTime = r.GetValue<string>("sendTime")
                }));
            }


            var countQuery = "select count(*) as count FROM [Wiki].[dbo].[Events] WHERE Code = " + eventCode;

            var count = 0;
            using (var cmd = this.GetCommand(countQuery))
            {
                cmd.ExecuteReader(r =>
                {
                    count = r.GetValue<int>("count");
                });
            }

            foreach (var price in result)
            {
                price.total = count;
            }

            return result;
        }
        #endregion

        #region PricesTab

        public List<PriceGroupDataResponse> GetPriceGroupData(PriceGroupDataRequest request)
        {
            var field = (request.Sort == null) ? "PriceListId" : request.Sort[0].Field;
            var sort = (request.Sort == null) ? "asc" : request.Sort[0].Dir;

            var catalogFilter = (String.IsNullOrWhiteSpace(request.CatalogFilter))
                ? ""
                : " AND SellerCatalog like '" + request.CatalogFilter + "%'";

            var numberFilter = (String.IsNullOrWhiteSpace(request.NumberFilter))
                ? ""
                : " AND SellerNumber like '" + request.NumberFilter + "%'";

            var nameFilter = (String.IsNullOrWhiteSpace(request.NameFilter))
                ? ""
                : " AND Name like '" + request.NameFilter + "%'";

            var result = new List<PriceGroupDataResponse>();
            var query =
                "SELECT [GroupId],[PriceListId],[Analog],[MinOrder],[ERP_ID],[SellerCatalog],[SellerNumber],[Name],[Qty] FROM [Wiki].[dbo].[PriceGroupData] where GroupId = " + request.GroupId + catalogFilter + numberFilter + nameFilter +
                " order by " + field + " " + sort + " OFFSET " + request.Skip + " ROWS FETCH NEXT " + request.Take + " ROWS ONLY";

            var count = 0;
            var countQuery = "select count(*) as count FROM [Wiki].[dbo].[PriceGroupData] where GroupId = " + request.GroupId;

            using (var cmd = this.GetCommand(query))
            {
                cmd.CommandTimeout = 180;

                cmd.ExecuteReader(r => result.Add(new PriceGroupDataResponse
                {
                    GroupId = r.GetValue<int>("GroupId"),
                    PriceListId = r.GetValue<int>("PriceListId"),
                    Analog = r.GetValue<string>("Analog"),
                    MinOrder = r.GetValue<int>("MinOrder"),
                    ErpId = r.GetValue<int>("ERP_ID"),
                    SellerCatalog = r.GetValue<string>("SellerCatalog"),
                    SellerNumber = r.GetValue<string>("SellerNumber"),
                    Name = r.GetValue<string>("Name"),
                    Qty = r.GetValue<int>("Qty")
                }));
            }
            using (var cmd = this.GetCommand(countQuery))
            {
                cmd.ExecuteReader(r =>
                {
                    count = r.GetValue<int>("count");
                });
            }

            foreach (var price in result)
            {
                price.total = count;
            }

            return result;
        }

        public List<PriceGroupDataResponse> GetPriceGroupsForExcel(int groupId)
        {
            var query =
                "SELECT [GroupId],[PriceListId],[Analog],[MinOrder],[ERP_ID],[SellerCatalog],[SellerNumber],[Name],[Qty] FROM [Wiki].[dbo].[PriceGroupData] where GroupId = " +
                groupId;

            //var model = new AnalyticsReportItem();

            var result = new List<PriceGroupDataResponse>();

            using (var cmd = this.GetCommand(query))
            {
                cmd.ExecuteReader(r => result.Add(new PriceGroupDataResponse
                {
                    GroupId = r.GetValue<int>("GroupId"),
                    PriceListId = r.GetValue<int>("PriceListId"),
                    Analog = r.GetValue<string>("Analog"),
                    MinOrder = r.GetValue<int>("MinOrder"),
                    ErpId = r.GetValue<int>("ERP_ID"),
                    SellerCatalog = r.GetValue<string>("SellerCatalog"),
                    SellerNumber = r.GetValue<string>("SellerNumber"),
                    Name = r.GetValue<string>("Name"),
                    Qty = r.GetValue<int>("Qty")
                }));
            }
            //model.Columns.Add(new Header() { Column = "Код прайса" });
            //model.Columns.Add(new Header() { Column = "Аналог" });
            //model.Columns.Add(new Header() { Column = "ERP_ID" });
            //model.Columns.Add(new Header() { Column = "Бренд" });
            //model.Columns.Add(new Header() { Column = "Артикул" });
            //model.Columns.Add(new Header() { Column = "Наименование" });
            //model.Columns.Add(new Header() { Column = "Кол-во" });
            //model.Columns.Add(new Header() { Column = "Мин. заказ" });

            //foreach (var priceGroup in priceGroups)
            //{
            //    model.Rows.Add(new Row() { Cell = new List<Cells>() {
            //        new Cells() { Value = priceGroup.PriceListId.ToString(), Color = "rgba(255,255,255,0)" },
            //        new Cells() { Value = priceGroup.Analog, Color = "rgba(255,255,255,0)" },
            //        new Cells() { Value = priceGroup.ErpId.ToString(), Color = "rgba(255,255,255,0)" },
            //        new Cells() { Value = priceGroup.SellerCatalog, Color = "rgba(255,255,255,0)" },
            //        new Cells() { Value = priceGroup.SellerNumber, Color = "rgba(255,255,255,0)" },
            //        new Cells() { Value = priceGroup.Name, Color = "rgba(255,255,255,0)" },
            //        new Cells() { Value = priceGroup.Qty.ToString(), Color = "rgba(255,255,255,0)" },
            //        new Cells() { Value = priceGroup.MinOrder.ToString(), Color = "rgba(255,255,255,0)" }
            //        }
            //    });
            //}
            //var result = await _udsCl.PostReport(model);

            return result;
        }

        public bool UpdatePriceGroupData(int groupId)
        {
            int rows = 0;
            using (var cmd = this.GetCommand("PriceGroupDataUpdate", true))
            {
                cmd.AddParameter("GroupId", groupId);
                rows = cmd.Execute();
            }
            return rows > 0;
        }

        #endregion

        #region Events
        public string GetManagerName(string userId)
        {
            var result = "";
            var query = "select cl.Name from Client cl where cl.Id = @id";

            using (var cmd = this.GetCommand(query))
            {
                cmd.AddParameter("id", userId);
                cmd.ExecuteReader(dr =>
                {
                    result = dr.GetValue<string>("Name");
                });
            }

            return result;
        }

        private string GetPriceName(int groupId)
        {
            var command = "	SELECT TOP 1 pg.Name as GroupName FROM PriceGroup pg where pg.Id = @groupId";
            string res = "";
            using (var cmd = this.GetCommand(command))
            {
                cmd.AddParameter("groupId", groupId);
                try
                {
                    cmd.ExecuteReader(r =>
                    {
                        res = r.GetValue<string>("GroupName");
                    });
                }
                catch (Exception e)
                {
                    return e.InnerException.Message;
                }
            }
            return res;
        }



        #endregion


        public List<SchedulerItem> GetSchedulersByGroupId(int groupId)
        {
            var query = "select * from PriceGroupToClient where PriceGroupId = " + groupId;
            var result = new List<SchedulerItem>();

            using (var cmd = GetCommand(query))
            {
                cmd.ExecuteReader(r =>
                {
                    var model = CreatePriceSendModel(r);
                    //model.EmailSetting = emails.First(o => o.Id == model.ProfileId);
                    result.Add(model);
                });

            }
            return result;
        }

        //вернуть scheduler или лист всех, если id = 0
        public List<SchedulerItem> GetSchedulers(int id = 0)
        {
            //var emails = GetEmailSettingsForMailingPricesService();

            using (var cmd = GetCommand("PriceSender_GetSchedulers"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("id", id);
                var result = new List<SchedulerItem>();
                cmd.ExecuteReader(r =>
                {
                    var model = CreatePriceSendModel(r);
                    //model.EmailSetting = emails.First(o => o.Id == model.ProfileId);
                    result.Add(model);
                });
                return result;
            }

        }

        private static SchedulerItem CreatePriceSendModel(DbDataReader r)
        {
            var model = new SchedulerItem
            {
                Id = r.GetValue<int>("Id"),
                GroupId = r.GetValue<int>("PriceGroupId"),
                ClientId = r.GetValue<int>("ClientId"),
                Email = r.GetValue<string>("Email"),
                DaysSend = r.GetValue<string>("DaysSend"),
                TimesSend = r.GetValue<string>("TimeSend"),
                FileName = r.GetValue<string>("FileName"),
                FileConfig = r.GetValue<string>("FileConfig"),
                FileType = r.GetValue<string>("FileType"),
                IsEnabled = r.GetValue<bool>("IsEnabled"),
                Subject = r.GetValue<string>("MailSubject"),
                Body = r.GetValue<string>("MailBody"),
                ProfileId = r.GetValue<int>("ProfileId"),
                LastSend = r.GetValue<DateTime>("LastSend")
            };
            return model;
        }


    }
}
