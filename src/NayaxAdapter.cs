using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace NayaxAPI
{
    public class NayaxAdapter
    {
        private const string API_URL = "https://uiservices.ecom.nayax.com/hosted/";

        private const string STATUS_SUCCESS = "SUCCESS";
        private const string STATUS_ERROR = "ERROR";

        private readonly string HASH_CODE;
        private readonly string MERCHANT_ID;

        public NayaxAdapter(string merchantId, string hashCode)
        {
            MERCHANT_ID = merchantId;
            HASH_CODE = hashCode;
        }

        public string InitiatePayment(Dictionary<string, string> transactionDetails)
        {
            var transaction = new Dictionary<string, string>
            {
                {"merchantID", MERCHANT_ID},
                {"trans_amount", transactionDetails["amount"]},
                {"trans_currency", transactionDetails["currency"]},
                {"trans_type", "0"}, // debit
                {"trans_installments", "1"},
                {"trans_refNum", transactionDetails["orderId"]}, // internal order id of merchant
                {"disp_paymentType", "CC"}, // credit card
                {"url_redirect", transactionDetails["redirectUrl"]},
                {"notification_url", transactionDetails["notificationUrl"]}
            };

            transaction["signature"] = CreateSignature(transaction);

            return GetRedirectUrl(transaction);
        }

        private string CreateSignature(IReadOnlyDictionary<string, string> transaction)
        {
            var signatureString = new StringBuilder();
            signatureString.Append(transaction["merchantID"]);
            signatureString.Append(transaction["trans_refNum"]);
            signatureString.Append(transaction["trans_installments"]);
            signatureString.Append(transaction["trans_amount"]);
            signatureString.Append(transaction["trans_currency"]);
            signatureString.Append(transaction["trans_type"]);
            signatureString.Append(transaction["disp_paymentType"]);
            signatureString.Append(transaction["notification_url"]);
            signatureString.Append(transaction["url_redirect"]);
            signatureString.Append(HASH_CODE);

            return GenerateSha256(signatureString.ToString());
        }

        private string GetRedirectUrl(IReadOnlyDictionary<string, string> transaction)
        {
            var redirectUrl = new StringBuilder();

            redirectUrl.Append(API_URL);
            redirectUrl.Append("?merchantID=" + MERCHANT_ID);
            redirectUrl.Append("&trans_refNum=" + transaction["trans_refNum"]);
            redirectUrl.Append("&trans_installments=" + transaction["trans_installments"]);
            redirectUrl.Append("&trans_amount=" + transaction["trans_amount"]);
            redirectUrl.Append("&trans_currency=" + transaction["trans_currency"]);
            redirectUrl.Append("&trans_type=" + transaction["trans_type"]);
            redirectUrl.Append("&disp_paymentType=" + transaction["disp_paymentType"]);
            redirectUrl.Append("&notification_url=" + WebUtility.UrlEncode(transaction["notification_url"]));
            redirectUrl.Append("&url_redirect=" + WebUtility.UrlEncode(transaction["url_redirect"]));
            redirectUrl.Append("&signature=" + WebUtility.UrlEncode(transaction["signature"]));


            return redirectUrl.ToString();
        }

        private static string GenerateSha256(string value)
        {
            var sh = SHA256.Create();
            var hashValue = sh.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(hashValue);
        }

        public Dictionary<string, string> handleNotification(IReadOnlyDictionary<string, string> notification)
        {
            var description = "";
            var code = "";
            var originalTransactionId = "";

            if (notification.ContainsKey("replyDesc"))
                description = notification["replyDesc"];
            else
                description = notification["ReplyDesc"];

            if (notification.ContainsKey("replyCode"))
                code = notification["replyCode"];
            else
                code = notification["Reply"];

            if (notification.ContainsKey("trans_refNum"))
                originalTransactionId = notification["trans_refNum"];
            else
                originalTransactionId = notification["Order"];

            var innerTransactionId = notification["trans_id"];
            var transactionAmount = notification["trans_amount"];
            var transactionCurrency = notification["trans_currency"];

            var notificationDetails = new Dictionary<string, string>
            {
                {"description", description},
                {"originalOrderId", originalTransactionId},
                {"internalTransactionId", innerTransactionId},
                {"amount", transactionAmount},
                {"currency", transactionCurrency},
                {"notification", notification.ToString()}
            };

            if (code == "000" || code == "000.000.000")
                notificationDetails["status"] = STATUS_SUCCESS;
            else
                notificationDetails["status"] = STATUS_ERROR;

            return notificationDetails;
        }
    }
}