using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Plugin.Shipping.Tawsela.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using Nop.Services.Vendors;

namespace Nop.Plugin.Shipping.Tawsela
{

    public class TawselaComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly TawselaService _tawselaService;

        #endregion

        #region Ctor

        public TawselaComputationMethod(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper,
            TawselaService tawselaService)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
            _tawselaService = tawselaService;
        }

        #endregion

        #region Methods

        public GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            var response = new GetShippingOptionResponse();

            if (getShippingOptionRequest == null)
                throw new ArgumentNullException(nameof(getShippingOptionRequest));

            if (!getShippingOptionRequest.Items?.Any() ?? true)
                return new GetShippingOptionResponse { Errors = new[] { "No shipment items" } };

            if (getShippingOptionRequest.ShippingAddress?.CountryId == null)
               return new GetShippingOptionResponse { Errors = new[] { "Shipping address is not set" } };


            //return _tawselaService.GetRates(getShippingOptionRequest);
            response.ShippingOptions.Add(new ShippingOption
            {
                Name = "Tawsela",
                Description = "TawselaDesc",
                Rate = GetFixedRate(getShippingOptionRequest).GetValueOrDefault(),
                TransitDays = 3
            });

            return response;

        }

        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            var rate = decimal.Zero;
            var baseUrl = "http://tawsela.4hoste.com/api/getDistancePriceTime";

            HttpClient client = new HttpClient();
            var vendorService = EngineContext.Current.Resolve<IVendorService>();
            var addressService = EngineContext.Current.Resolve<IAddressService>();
            var vendorId = getShippingOptionRequest.Items.FirstOrDefault().Product.VendorId;
            var vendor = vendorService.GetVendorById(vendorId != 0 ? vendorId : 0);
            var addressId = vendor != null ? vendor.AddressId : 0;
            var vendorAddress = addressService.GetAddressById(addressId);
            var values = new Dictionary<string, string>
                {
                                { "from_country", vendorAddress != null ? vendorAddress.CountryId.ToString() :""},
                                { "from_state", vendorAddress != null ? vendorAddress.StateProvinceId.ToString() :""},
                                { "from_city", vendorAddress != null ? vendorAddress.CityId.ToString() :""},
                                { "to_country", getShippingOptionRequest.ShippingAddress.CountryId.ToString() },
                                { "to_state", getShippingOptionRequest.ShippingAddress.StateProvinceId.ToString() },
                                { "to_city", getShippingOptionRequest.ShippingAddress.CityId.ToString() },

                };

            var content = new FormUrlEncodedContent(values);

            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(baseUrl);
            request.Content = content;
            request.Method = HttpMethod.Post;
            request.Headers.Add("appId", "194214236577");
            request.Headers.Add("serverKey", "eyJpdi-I6IlJH-SU9jSU-VGQllj-TFVnOH-BveDBG");
            var response= client.SendAsync(request).Result;
            var responseString =  response.Content.ReadAsStringAsync().Result;
            if (!string.IsNullOrEmpty(responseString) && JObject.Parse(responseString).GetValue("key") != null && JObject.Parse(responseString).GetValue("key").ToString() == "success")
            {
                var data = JObject.Parse(responseString).GetValue("data").ToString();
                var price = JObject.Parse(data).GetValue("price").ToString();
                rate = Convert.ToDecimal(price);
            }
     
            return rate;
        }


        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/TawselaShipping/Configure";
        }

        public override void Install()
        {
            _settingService.SaveSetting(new TawselaSettings
            {
                UseSandbox = true,
            });

            _localizationService.AddPluginLocaleResource(new Dictionary<string, string>
            {
                ["Plugins.Shipping.Tawsela.Fields.AppId"] = "App Id",
                ["Plugins.Shipping.Tawsela.Fields.AppId.Hint"] = "Specify Tawsela App Id.",
                ["Plugins.Shipping.Tawsela.Fields.UseSandbox"] = "Use sandbox",
                ["Plugins.Shipping.Tawsela.Fields.UseSandbox.Hint"] = "Check to use sandbox (testing environment).",
                ["Plugins.Shipping.Tawsela.Fields.ServerKey"] = "Server Key",
                ["Plugins.Shipping.Tawsela.Fields.ServerKey.Hint"] = "Specify Tawsela Server Key.",
            });

            base.Install();
        }


        public override void Uninstall()
        {
            _settingService.DeleteSetting<TawselaSettings>();

            _localizationService.DeletePluginLocaleResources("Plugins.Shipping.Tawsela");

            base.Uninstall();
        }

        #endregion

        #region Properties

        public ShippingRateComputationMethodType ShippingRateComputationMethodType => ShippingRateComputationMethodType.Realtime;

        public IShipmentTracker ShipmentTracker => new TawselaShipmentTracker(_tawselaService);

        #endregion
    }
}