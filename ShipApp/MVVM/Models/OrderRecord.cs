using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApp.MVVM.Models
{
    internal class OrderRecord
    {
        public int OrderLineId { get; set; }
        public long OrderShipId { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityChecked { get; set; }
        public int OrderMeasurementId { get; set; }
        public int OrderItemId { get; set; }
        public bool Checked { get; set; }
        public string Notes { get; set; }
    }
}
