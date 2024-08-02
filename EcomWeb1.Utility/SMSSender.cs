using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace EcomWeb1.Utility
{
    public class SMSSender : ISMSSender
    {
        private readonly TwilioSettings _twilioSettings;
        public SMSSender(IOptions<TwilioSettings> twilioSettings)
        {
            _twilioSettings= twilioSettings.Value;
        }
        public async Task SenderSMSAsync(string toPhone, string message)
        {
            TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);
            await MessageResource.CreateAsync(
                to: toPhone,
                from: _twilioSettings.PhoneNumber,
                body: message
                );
        }
        public void SendSms(string toPhone, string message)
        {
            TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);

            MessageResource.Create(
                to: toPhone,
                from: _twilioSettings.PhoneNumber,
                body: message
            );
        }
    }
}
