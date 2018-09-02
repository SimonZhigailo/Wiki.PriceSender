using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Service.Models.PriceList

{
    /// <summary>
    /// Модель дерева каталогов
    /// </summary>
    public class GroupTree
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public int Type { get; set; }

        public int Level { get; set; }

        public string Name { get; set; }

        public List<GroupTree> Childs { get; set; }

        public int Cnt { get; set; }

        public int Qty { get; set; }

        public GroupTree()
        {
            this.Childs = new List<GroupTree>();
        }
    }
}
