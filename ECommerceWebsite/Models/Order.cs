using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceWebsite.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public int OrderNumber { get; set; }

        public DateTime Orderdate { get; set; }

        public int CustomerId { get; set; }

        public int TotalDiscount { get; set; }

        public int TotalAmount { get; set; }

        public string OrderStatus { get; set; }
    }
}
