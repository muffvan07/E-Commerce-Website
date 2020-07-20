using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceWebsite.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string RetailerId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
        public int UnitPrice { get; set; }
        public int Discount { get; set; }
        public string Photo { get; set; }
        public bool Status { get; set; }
    }
}
