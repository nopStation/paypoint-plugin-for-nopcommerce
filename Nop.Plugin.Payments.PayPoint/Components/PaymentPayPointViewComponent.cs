using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.PayPoint.Components
{
    [ViewComponent(Name = "PaymentPayPoint")]
    public class PaymentPayPointViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Payments.PayPoint/Views/PaymentInfo.cshtml");
        }
    }
}
