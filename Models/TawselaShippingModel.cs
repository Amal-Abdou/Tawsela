using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Shipping.Tawsela.Models
{
    public class TawselaShippingModel : BaseNopModel
    {

        #region Properties

        [NopResourceDisplayName("Plugins.Shipping.Tawsela.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.Tawsela.Fields.AppId")]
        public string AppId { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.Tawsela.Fields.ServerKey")]
        public string ServerKey { get; set; }

        #endregion
    }
}