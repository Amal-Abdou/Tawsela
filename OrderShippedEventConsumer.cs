using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Vendors;

namespace Nop.Plugin.SMS.Verizon
{
    public class OrderShippedEventConsumer : IConsumer<ShipmentSentEvent>
    {
        private readonly IShipmentService _shipmentService;
        private readonly IVendorService _vendorService;
        private readonly IAddressService _addressService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;


        public OrderShippedEventConsumer(
            IShipmentService shipmentService,
            IVendorService vendorService,
            IAddressService addressService,
            IProductService productService,
            IOrderService orderService,
            ICustomerService customerService)
        {
            this._shipmentService = shipmentService;
            this._vendorService = vendorService;
            this._addressService = addressService;
            this._productService = productService;
            this._orderService = orderService;
            this._customerService = customerService;
        }


        public void HandleEvent(ShipmentSentEvent eventMessage)
        { 

            var shipment = eventMessage.Shipment;
            var shippingOrderId = 0;
            var baseUrl = "http://tawsela.4hoste.com/api/createNewOrder";

            HttpClient client = new HttpClient();
            var Items = _shipmentService.GetShipmentItemsByShipmentId(shipment.Id).ToList();
            var Item = _orderService.GetOrderItemById(Items.FirstOrDefault().OrderItemId);
            var product = _productService.GetProductById(Item.ProductId);
            var vendor = _vendorService.GetVendorById(product.VendorId != 0 ? product.VendorId : 0);
            var vendorAddress = _addressService.GetAddressById(vendor != null ? vendor.AddressId : 0);
            var order = _orderService.GetOrderById(shipment.OrderId);
            var customerAddress = _addressService.GetAddressById(order.ShippingAddressId.GetValueOrDefault());
            var values = new Dictionary<string, string>
                            {
                                { "place_name" , vendor.Name },
                                { "user_name" , customerAddress.FirstName + " " + customerAddress.LastName },
                                { "user_phone" ,customerAddress.PhoneNumber },
                                { "package_type" , "food" },
                                { "start_address" ,vendorAddress != null ? vendorAddress.Address1 : "" },
                                { "start_lat" , "31.03223562034151" },
                                { "start_long" , "31.396506431736107" },
                                { "end_address" , customerAddress.Address1 },
                                { "end_lat" , "31.03223562034151" },
                                { "end_long" , "31.396506431736107" },
                                { "address1" , vendorAddress != null ? vendorAddress.Address1 : ""},
                                { "address2" , customerAddress.Address1  },
                                { "payment_type" , order.PaymentMethodSystemName },
                                { "notes" , "2 سندوتش برجر" },
                                { "start_country" , vendorAddress != null ? vendorAddress.CountryId.ToString() : ""},
                                { "start_state" , vendorAddress != null ? vendorAddress.StateProvinceId.ToString() : ""},
                                { "start_city" , vendorAddress != null ? vendorAddress.CityId.ToString() : ""},
                                { "end_country" , customerAddress.CountryId.ToString() },
                                { "end_state" , customerAddress.StateProvinceId.ToString() },
                                { "end_city" ,customerAddress.CityId.ToString() }

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

        }
    }
}