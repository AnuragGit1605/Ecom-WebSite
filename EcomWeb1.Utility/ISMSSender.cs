using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomWeb1.Utility
{
    public interface ISMSSender
    {
        Task SenderSMSAsync(string toPhone, string message);
        public void SendSms(string toPhone, string message);

    }
}
