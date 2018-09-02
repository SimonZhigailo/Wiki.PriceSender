using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Service.Models.PriceList

{
    /// <summary>
    /// Общая модель прайс группы без артикулов(в данный момент используется только Catalog)
    /// </summary>
    public class PriceGroupSettings
    {
        public int Id;
        public bool ActionCatalog;
        public bool ActionGroupTree;
        public bool ActionArticles;
        public List<PriceGroupPriceList> prices;
        public List<Catalog> catalogs;
        public List<Tree> trees;
    }
    /// <summary>
    /// Модель каталога
    /// </summary>
    public class Catalog
    {
        public int Id;
        public string Name;
        public bool IsEnabled;
        public bool IsOriginal;
        public string KeyUrl = "";
    }

    public class Tree
    {
        public int Id;
        public string Name;
    }


}
