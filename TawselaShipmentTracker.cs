using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nop.Plugin.Shipping.Tawsela.Services;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.Tawsela
{
    public class TawselaShipmentTracker : IShipmentTracker
    {
        #region Fields

        private readonly TawselaService _tawselaService;

        #endregion

        #region Ctor

        public TawselaShipmentTracker(TawselaService tawselaService)
        {
            _tawselaService = tawselaService;
        }

        #endregion

        #region Methods

        public virtual bool IsMatch(string trackingNumber)
        {
            if (string.IsNullOrEmpty(trackingNumber))
                return false;
            
            return false;
        }

        public virtual string GetUrl(string trackingNumber)
        {
            return $"https://www.tawsela.com/track?&tracknum={trackingNumber}";
        }

        public virtual IList<ShipmentStatusEvent> GetShipmentEvents(string trackingNumber)
        {
            var result = new List<ShipmentStatusEvent>();

            if (string.IsNullOrEmpty(trackingNumber))
                return result;

            return result;
        }

        #endregion
    }
}