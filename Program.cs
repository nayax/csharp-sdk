using System.Collections.Generic;

namespace NayaxAPI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string merchantId = "";
            const string hashCode = "";

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

            var notification = new Dictionary<string, string>
            {
                // {"replyDesc", "desc"}, // for example because of legacy api
                {"ReplyDesc", "Desc"},
                {"Reply", "000"},
                {"replyCode", "000"},
                {"trans_refNum", "1234"},
                {"Order", "1235"},
                {"trans_id", "1233"},
                {"trans_amount", "1"},
                {"trans_currency", "USD"}
            };

            var notificationResponse = nayaxAdapter.handleNotification(notification);
        }
    }
}