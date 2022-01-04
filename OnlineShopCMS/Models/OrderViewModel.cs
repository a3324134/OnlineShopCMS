using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopCMS.Models
{
    public class OrderViewModel
    {
        public Order Order { get; set; }
        public List<OrderItemDetail> OrderItemDetails { get; set; }
    }
}
