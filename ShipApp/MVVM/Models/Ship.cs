using System;
using Xamarin.Google.Crypto.Tink.Signature;

namespace ShipApp.Models
{
    public class Ship
    {
        public long ShipId { get; set; }
        public string ShipName { get; set; }
        public bool Complete { get; set; }
        public bool IsNeeded { get; set; }
        public DateTime? DateCompleted { get; set; }
        public string CheckedStickers { get; set; }
        public bool IsPrinted { get; set; }

        public Ship(string shipName)
        {
            ShipName = shipName;
            Complete = false;
            IsNeeded = false;
            DateCompleted = null;
            CheckedStickers = string.Empty;
            IsPrinted = false;
        }
    }

}
