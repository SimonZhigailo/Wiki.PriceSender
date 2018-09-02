using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Dto.PriceList

{
    /// <summary>
    /// Модель дерева каталогов
    /// </summary>
    public class GroupTreeDto
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public int Type { get; set; }

        public int Level { get; set; }

        public string Name { get; set; }

        public List<GroupTreeDto> Childs { get; set; }

        public int Cnt { get; set; }

        public int Qty { get; set; }

        public GroupTreeDto()
        {
            this.Childs = new List<GroupTreeDto>();
        }
    }
}
