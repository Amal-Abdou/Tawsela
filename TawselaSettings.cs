using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.Tawsela
{
    public class TawselaSettings : ISettings
    {

        public string AppId { get; set; }
        public string ServerKey { get; set; }
        public bool UseSandbox { get; set; }

    }
}