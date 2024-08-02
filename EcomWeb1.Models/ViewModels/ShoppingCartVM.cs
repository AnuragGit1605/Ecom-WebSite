using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomWeb1.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingKart> ListCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public List<string> UniqueAddresses { get; set; }
    }
}
