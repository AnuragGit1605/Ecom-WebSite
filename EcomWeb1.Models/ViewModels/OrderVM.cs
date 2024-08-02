using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomWeb1.Models.ViewModels
{
    public class OrderVM
    {
        public string Name { get; set; }
        public string OrderDate { get; set; }
        public double OrderTotal { get; set; }
        public int OrderId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public class OrderItem
        {
            public int SerialNumber { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public double Price { get; set; }
            public double Amount => Quantity * Price;
        }
    }
}
