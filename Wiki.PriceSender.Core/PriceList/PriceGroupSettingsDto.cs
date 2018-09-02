using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Dto.PriceList

{
    /// <summary>
    /// Общая модель прайс группы без артикулов(в данный момент используется только Catalog)
    /// </summary>
    public class PriceGroupSettingsDto
    {
        public int Id;
        public bool ActionCatalog;
        public bool ActionGroupTree;
        public bool ActionArticles;
        public List<PriceGroupPriceListDto> prices;
        public List<CatalogDto> catalogs;
        public List<TreeDto> trees;
    }
    /// <summary>
    /// Модель каталога
    /// </summary>
    public class CatalogDto
    {
        public int Id;
        public string Name;
        public bool IsEnabled;
        public bool IsOriginal;
        public string KeyUrl = "";
    }

    public class TreeDto
    {
        public int Id;
        public string Name;
    }


}
