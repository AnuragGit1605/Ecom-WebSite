using Microsoft.Extensions.Configuration;
using PayPal.Api;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcomWeb1.Utility
{
    public class Paypal : IPaypal
    {
        private readonly APIContext _apiContext;
        private readonly IConfiguration _configuration;

        public Paypal(IConfiguration configuration)
        {
            _configuration = configuration;

            var clientId = _configuration["PayPalSettings:ClientId"];
            var clientSecret = _configuration["PayPalSettings:Secret"];

            var config = new Dictionary<string, string>
            {
                { "mode", "sandbox" },
                { "clientId", clientId },
                { "clientSecret", clientSecret }
            };

            var accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
            _apiContext = new APIContext(accessToken);
        }

        public async Task<Payment> CreateOrderAsync(double amount, string returnUrl, string cancelUrl)
        {
            var itemList = new ItemList
            {
                items = new List<Item>
                {
                    new Item
                    {
                        name = "Payment",
                        currency = "INR",
                        price = amount.ToString("0.00"),
                        quantity = "1",
                        sku = "payment"
                    }
                }
            };

            var transaction = new Transaction
            {
                amount = new Amount
                {
                    currency = "USD",
                    total = amount.ToString("0.00"),
                    details = new Details
                    {
                        subtotal = amount.ToString("0.00")
                    }
                },
                item_list = itemList,
                description = "Book Payment"
            };

            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                redirect_urls = new RedirectUrls
                {
                    return_url = returnUrl,
                    cancel_url = cancelUrl
                },
                transactions = new List<Transaction> { transaction }
            };

            var createdPayment = payment.Create(_apiContext);
            return createdPayment;
        }
    }
}
