using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Dto.PriceList

{
    /// <summary>
    /// Модель группы прайсов
    /// </summary>
    public class PriceGroupListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountRows { get; set; }
        public int CountClients { get; set; }
    }
}
