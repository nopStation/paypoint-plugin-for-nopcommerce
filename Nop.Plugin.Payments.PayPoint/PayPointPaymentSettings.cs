using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.PayPoint
{
    public class PayPointPaymentSettings : ISettings
    {
        /// <summary>
        /// Gets or sets API username
        /// </summary>
        public string ApiUsername { get; set; }

        /// <summary>
        /// Gets or sets API password
        /// </summary>
        public string ApiPassword { get; set; }

        /// <summary>
        /// Gets or sets installation ID
        /// </summary>
        public string InstallationId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox (testing environment)
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets an additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }
    }
}
