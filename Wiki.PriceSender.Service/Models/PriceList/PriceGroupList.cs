using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Service.Models.PriceList

{
    /// <summary>
    /// Модель группы прайсов
    /// </summary>
    public class PriceGroupList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountRows { get; set; }
        public int CountClients { get; set; }
    }
}
