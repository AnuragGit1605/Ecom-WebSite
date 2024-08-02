using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomWeb1.Utility
{
    public interface IPaypal
    {
       Task<Payment> CreateOrderAsync(double amount, string returnUrl, string cancelUrl);
    }
}
