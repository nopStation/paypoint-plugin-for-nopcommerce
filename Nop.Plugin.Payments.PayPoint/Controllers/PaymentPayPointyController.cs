using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Payments.PayPoint.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.PayPoint.Controllers
{
    
    public class PaymentPayPointController : BasePaymentController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor

        public PaymentPayPointController(ILocalizationService localizationService,
            ILogger logger,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            ISettingService settingService,
            IStoreContext storeContext,
            IPermissionService permissionService,
            INotificationService notificationService)
        {
            _localizationService = localizationService;
            _logger = logger;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _settingService = settingService;
            _storeContext = storeContext;
            _permissionService = permissionService;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var payPointPaymentSettings = await _settingService.LoadSettingAsync<PayPointPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                ApiUsername = payPointPaymentSettings.ApiUsername,
                ApiPassword = payPointPaymentSettings.ApiPassword,
                InstallationId = payPointPaymentSettings.InstallationId,
                UseSandbox = payPointPaymentSettings.UseSandbox,
                AdditionalFee = payPointPaymentSettings.AdditionalFee,
                AdditionalFeePercentage = payPointPaymentSettings.AdditionalFeePercentage,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.InstallationId_OverrideForStore = await _settingService.SettingExistsAsync(payPointPaymentSettings, x => x.InstallationId, storeScope);
                model.UseSandbox_OverrideForStore = await _settingService.SettingExistsAsync(payPointPaymentSettings, x => x.UseSandbox, storeScope);
                model.AdditionalFee_OverrideForStore = await _settingService.SettingExistsAsync(payPointPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = await _settingService.SettingExistsAsync(payPointPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
            }

            return View("~/Plugins/Payments.PayPoint/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var payPointPaymentSettings = await _settingService.LoadSettingAsync<PayPointPaymentSettings>(storeScope);

            //save settings
            payPointPaymentSettings.ApiUsername = model.ApiUsername;
            payPointPaymentSettings.ApiPassword = model.ApiPassword;
            payPointPaymentSettings.InstallationId = model.InstallationId;
            payPointPaymentSettings.UseSandbox = model.UseSandbox;
            payPointPaymentSettings.AdditionalFee = model.AdditionalFee;
            payPointPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingAsync(payPointPaymentSettings, x => x.ApiUsername, storeScope, false);
            await _settingService.SaveSettingAsync(payPointPaymentSettings, x => x.ApiPassword, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(payPointPaymentSettings, x => x.InstallationId, model.InstallationId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(payPointPaymentSettings, x => x.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(payPointPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(payPointPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return RedirectToAction("Configure");
        }

        public async Task<IActionResult> Callback(object obj)
        {
            PayPointCallback payPointPaymentCallback = null;
            try
            {
                using (var streamReader = new StreamReader(HttpContext.Request.Body))
                {
                    payPointPaymentCallback = JsonConvert.DeserializeObject<PayPointCallback>(streamReader.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("PayPoint callback error", ex);
                return new StatusCodeResult((int)HttpStatusCode.OK);
            }

            if (payPointPaymentCallback.Transaction.Status != PayPointStatus.SUCCESS)
            {
                await _logger.ErrorAsync($"PayPoint callback error. Transaction is {payPointPaymentCallback.Transaction.Status}");
                return new StatusCodeResult((int)HttpStatusCode.OK);
            }

            if (!Guid.TryParse(payPointPaymentCallback.Transaction.MerchantRef, out Guid orderGuid))
            {
                await _logger.ErrorAsync("PayPoint callback error. Data is not valid");
                return new StatusCodeResult((int)HttpStatusCode.OK);
            }

            var order = await _orderService.GetOrderByGuidAsync(orderGuid);
            if (order == null)
                return new StatusCodeResult((int)HttpStatusCode.OK);

            //paid order
            if (_orderProcessingService.CanMarkOrderAsPaid(order))
            {
                order.CaptureTransactionId = payPointPaymentCallback.Transaction.TransactionId;
                await _orderService.UpdateOrderAsync(order);
                await _orderProcessingService.MarkOrderAsPaidAsync(order);
            }

            return new StatusCodeResult((int)HttpStatusCode.OK);
        }

        #endregion
    }
}