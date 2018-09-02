using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Service.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string ArticleNumber { get; set; }
        public string Name { get; set; }
        public string Catalog { get; set; }
    }

    public class ArticleCatalogs
    {
        public int CatalogId { get; set; }
        public string Name { get; set; }
        public string KeyUrl { get; set; }
        public string Article { get; set; }
        public bool IsOurBrand { get; set; }
        public bool IsOriginal { get; set; }
    }
}
