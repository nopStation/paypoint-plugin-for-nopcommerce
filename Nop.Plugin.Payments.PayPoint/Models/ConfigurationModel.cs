using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.PayPoint.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPoint.Fields.ApiUsername")]
        public string ApiUsername { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPoint.Fields.ApiPassword")]
        public string ApiPassword { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPoint.Fields.InstallationId")]
        public string InstallationId { get; set; }
        public bool InstallationId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPoint.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPoint.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPoint.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }
    }
}