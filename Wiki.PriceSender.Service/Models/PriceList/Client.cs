using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki.PriceSender.Service.Models.PriceList
{/// <summary>
/// Клиент
/// </summary>
    public class Client
    {
        public int Id { get; set; }
        public int ClientCode { get; set; }
        public int Company { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
