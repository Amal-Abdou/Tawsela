using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Shipping.Tawsela.Models;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Shipping.Tawsela.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class TawselaShippingController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IMeasureService _measureService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly TawselaSettings _tawselaSettings;

        #endregion

        #region Ctor

        public TawselaShippingController(ILocalizationService localizationService,
            IMeasureService measureService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            TawselaSettings tawselaSettings)
        {
            _localizationService = localizationService;
            _measureService = measureService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _tawselaSettings = tawselaSettings;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new TawselaShippingModel
            {
                UseSandbox = _tawselaSettings.UseSandbox,
                AppId = _tawselaSettings.AppId,
                ServerKey = _tawselaSettings.ServerKey
            };

            return View("~/Plugins/Croxees.Shipping.Tawsela/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(TawselaShippingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
               return Configure();

            _tawselaSettings.UseSandbox = model.UseSandbox;
            _tawselaSettings.AppId = model.AppId;
            _tawselaSettings.ServerKey = model.ServerKey;

            _settingService.SaveSetting(_tawselaSettings);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}