using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Service.Models.PriceList

{
    /// <summary>
    /// модель radio buttons (включить/исключить)
    /// </summary>
    public class RadioButtons
    {
        public bool ActionArticles { get; set; }
        public bool ActionCatalog { get; set; }
        public bool ActionGroupTree { get; set; }

    }
}
