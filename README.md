# Nayax API C# SDK

## install like a local package or just copy files

## example form is given

## example in folders

```c#
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
```
