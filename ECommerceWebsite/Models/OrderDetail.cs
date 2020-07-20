using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceWebsite.Models
{
    public class OrderDetail
    {
        [Key]

        public int OrderId { get; set; }

        public string Category { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public string Unit { get; set; }

        public int Price { get; set; }

        public int TotalDiscount { get; set; }

        public int TotalAmount { get; set; }
    }
}
