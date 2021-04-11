using System;
using System.Collections.Generic;

namespace NayaxAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            const string merchantId = "5787743";
            const string hashCode = "YWVGU8GM4L";
            
            var nayaxAdapter = new NayaxAdapter(merchantId, hashCode);

            var transactionDetails = new Dictionary<string, string>
            {
                {"amount", "10"},
                {"currency", "ACP"},
                {"orderId", "123444"},
                {"methodCode", "visa"},
                {"redirectUrl", "https://www.google.com/"},
                {"notificationUrl", "https://www.google.com/"}
            };

            var redirectUrl = nayaxAdapter.InitiatePayment(transactionDetails);
            Console.WriteLine(redirectUrl);
        }
    }
}