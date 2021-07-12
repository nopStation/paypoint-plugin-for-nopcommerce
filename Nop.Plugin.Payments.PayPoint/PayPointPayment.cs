using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nop.Plugin.Payments.PayPoint
{
    /// <summary>
    /// Represents payment
    /// </summary>
    public class PayPointPayment
    {
        /// <summary>
        /// Gets or sets details of the transaction you want to create.
        /// </summary>
        [JsonProperty(PropertyName = "transaction")]
        public PayPointPaymentTransaction Transaction { get; set; }

        /// <summary>
        /// Gets or sets details of the customer.
        /// </summary>
        [JsonProperty(PropertyName = "customer")]
        public PayPointPaymentCustomer Customer { get; set; }

        /// <summary>
        /// Gets or sets the ISO-639 code for your Customer’s locale.
        /// </summary>
        [JsonProperty(PropertyName = "locale")]
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets details of the current session.
        /// </summary>
        [JsonProperty(PropertyName = "session")]
        public PayPointPaymentSession Session { get; set; }
    }

    /// <summary>
    /// Represents payment transaction
    /// </summary>
    public class PayPointPaymentTransaction
    {
        /// <summary>
        /// Gets or sets your reference for the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "merchantReference")]
        public string MerchantReference { get; set; }

        /// <summary>
        /// Gets or sets money information.
        /// </summary>
        [JsonProperty(PropertyName = "money")]
        public PayPointPaymentMoney Money { get; set; }

        /// <summary>
        /// Gets or sets the description of the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }

    /// <summary>
    /// Represents money information 
    /// </summary>
    public class PayPointPaymentMoney
    {
        /// <summary>
        /// Gets or sets the currency of your customer’s transaction. Use the 3 character ISO-4217 code.
        /// </summary>
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets amount specifications.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public PayPointPaymentAmount Amount { get; set; }
    }

    /// <summary>
    /// Represents amount
    /// </summary>
    public class PayPointPaymentAmount
    {
        /// <summary>
        /// Gets or sets a fixed amount. The customer can not change the amount.
        /// </summary>
        [JsonProperty(PropertyName = "fixed")]
        public decimal Fixed { get; set; }
    }

    /// <summary>
    /// Represents customer information
    /// </summary>
    public class PayPointPaymentCustomer
    {
        /// <summary>
        /// Gets or sets a value indicating whether you wish to create or use a registered customer. False if you do not wish to register your customer, otherwise set to true. 
        /// </summary>
        [JsonProperty(PropertyName = "registered")]
        public bool Registered { get; set; }
    }

    /// <summary>
    /// Represents session
    /// </summary>
    public class PayPointPaymentSession
    {
        /// <summary>
        /// Gets or sets details of the callback made before the transaction is sent for authorisation.
        /// </summary>
        [JsonProperty(PropertyName = "preAuthCallback")]
        public PayPointPaymentCallbackUrl PreAuthCallback { get; set; }

        /// <summary>
        /// Gets or sets details of the callback made after the transaction is sent for authorisation.
        /// </summary>
        [JsonProperty(PropertyName = "postAuthCallback")]
        public PayPointPaymentCallbackUrl PostAuthCallback { get; set; }

        /// <summary>
        /// Gets or sets details of the notification sent after transaction completion.
        /// </summary>
        [JsonProperty(PropertyName = "transactionNotification")]
        public PayPointPaymentCallbackUrl TransactionNotification { get; set; }

        /// <summary>
        /// Gets or sets the URL that will return your customer to after processing the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "returnUrl")]
        public PayPointPaymentUrl ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL that will return your customer to if they cancel the hosted session. If omitted the returnUrl is used if they cancel.
        /// </summary>
        [JsonProperty(PropertyName = "cancelUrl")]
        public PayPointPaymentUrl CancelUrl { get; set; }
    }

    /// <summary>
    /// Represents payment callback details
    /// </summary>
    public class PayPointPaymentCallbackUrl
    {
        /// <summary>
        /// Gets or sets the URL you want the callback or notification to be sent to. Where a blank URL field is specified, no callback or notification will be sent.
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the format of the callback content.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "format")]
        public PayPointPaymentFormat Format { get; set; }
    }

    /// <summary>
    /// Represents url
    /// </summary>
    public class PayPointPaymentUrl
    {
        /// <summary>
        /// Gets or sets the redirection URL.
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }

    /// <summary>
    /// Represents response format enumeration
    /// </summary>
    public enum PayPointPaymentFormat
    {
        /// <summary>
        /// Represents XML format
        /// </summary>
        REST_XML,

        /// <summary>
        /// Represents JSON format
        /// </summary>
        REST_JSON
    }

    /// <summary>
    /// Represents payment response details
    /// </summary>
    public class PayPointPaymentResponse
    {
        /// <summary>
        /// Gets or sets ID for the hosted session.
        /// </summary>
        [JsonProperty(PropertyName = "sessionId")]
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the URL you should direct your customer to to start the hosted session.
        /// </summary>
        [JsonProperty(PropertyName = "redirectUrl")]
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets or sets the status of the session creation.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "status")]
        public PayPointStatus Status { get; set; }

        /// <summary>
        /// Gets or sets further information about the status of the session creation.
        /// </summary>
        [JsonProperty(PropertyName = "reasonCode")]
        public string ReasonCode { get; set; }

        /// <summary>
        /// Gets or sets further information about the status of the session creation. This is where will provides detailed information about any errors.
        /// </summary>
        [JsonProperty(PropertyName = "reasonMessage")]
        public string ReasonMessage { get; set; }
    }

    /// <summary>
    /// Represents status enumeration
    /// </summary>
    public enum PayPointStatus
    {
        /// <summary>
        /// Represents success status
        /// </summary>
        SUCCESS,

        /// <summary>
        /// Represents failed status
        /// </summary>
        FAILED,

        /// <summary>
        /// Represents pending status
        /// </summary>
        PENDING,

        /// <summary>
        /// Represents expired status
        /// </summary>
        EXPIRED,

        /// <summary>
        /// Represents cancelled status
        /// </summary>
        CANCELLED,

        /// <summary>
        /// Represents voided status
        /// </summary>
        VOIDED
    }

    /// <summary>
    /// Represents callback details
    /// </summary>
    public class PayPointCallback
    {
        /// <summary>
        /// Gets or sets ID for the hosted session.
        /// </summary>
        [JsonProperty(PropertyName = "sessionId")]
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets details of the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "transaction")]
        public PayPointCallbackTransaction Transaction { get; set; }
    }

    /// <summary>
    /// Represents callback transaction
    /// </summary>
    public class PayPointCallbackTransaction
    {
        /// <summary>
        /// Gets or sets ID for the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "transactionId")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets reference for the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "merchantRef")]
        public string MerchantRef { get; set; }

        /// <summary>
        /// Gets or sets the current state of the transaction.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "status")]
        public PayPointStatus Status { get; set; }
    }
}
