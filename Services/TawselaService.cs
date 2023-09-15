using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.Tawsela.Services
{
    public class TawselaService
    {

        #region Ctor

        public TawselaService()
        {

        }

        #endregion

        #region Methods
        public virtual int CreateNewShipment()
        {
            var shippingOrderId = 0;
                var baseUrl = "http://tawsela.4hoste.com/api/createNewOrder";

                HttpClient client = new HttpClient();
                var values = new Dictionary<string, string>
                            {
                                { "place_name" , "kfc" },
                                { "user_name" , "mohamed maree" },
                                { "user_phone" , "0512345678" },
                                { "package_type" , "food" },
                                { "start_address" , "الرياض التخصصي" },
                                { "start_lat" , "31.03223562034151" },
                                { "start_long" , "31.396506431736107" },
                                { "end_address" , "الدمام شارع ٣٠" },
                                { "end_lat" , "31.03223562034151" },
                                { "end_long" , "31.396506431736107" },
                                { "address1" , "التخصصي" },
                                { "address2" , "شارع ٣٠" },
                                { "payment_type" , "visa" },
                                { "notes" , "2 سندوتش برجر" },
                                { "start_country" , "1" },
                                { "start_state" , "1" },
                                { "start_city" , "1" },
                                { "end_country" , "1" },
                                { "end_state" , "1" },
                                { "end_city" , "1" }

                            };

                var content = new FormUrlEncodedContent(values);

                HttpRequestMessage request = new HttpRequestMessage();
                request.RequestUri = new Uri(baseUrl);
                request.Content = content;
                request.Method = HttpMethod.Post;
                request.Headers.Add("appId", "194214236577");
                request.Headers.Add("serverKey", "eyJpdi-I6IlJH-SU9jSU-VGQllj-TFVnOH-BveDBG");
                var response = client.SendAsync(request).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(responseString) && JObject.Parse(responseString).GetValue("key") != null && JObject.Parse(responseString).GetValue("key").ToString() == "success")
                {
                     var data = JObject.Parse(responseString).GetValue("data").ToString();
                     shippingOrderId = Convert.ToInt32(JObject.Parse(data).GetValue("order_id").ToString());
                }

            return shippingOrderId;

        }

        public virtual string GetShippingStatus()
        {
            var shippingStatus = "";
            var baseUrl = "http://tawsela.4hoste.com/api/getOrderDetails";

            HttpClient client = new HttpClient();
            var values = new Dictionary<string, string>
                            {
                                { "order_id" , "9098" }

                            };

            var content = new FormUrlEncodedContent(values);

            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(baseUrl);
            request.Content = content;
            request.Method = HttpMethod.Post;
            request.Headers.Add("appId", "194214236577");
            request.Headers.Add("serverKey", "eyJpdi-I6IlJH-SU9jSU-VGQllj-TFVnOH-BveDBG");
            var response = client.SendAsync(request).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            if (!string.IsNullOrEmpty(responseString) && JObject.Parse(responseString).GetValue("key") != null && JObject.Parse(responseString).GetValue("key").ToString() == "success")
            {
                var data = JObject.Parse(responseString).GetValue("data").ToString();
                shippingStatus = JObject.Parse(data).GetValue("captain_status").ToString();
            }

            return shippingStatus;

        }

        #endregion

    }
}