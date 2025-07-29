using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApp.MVVM.Models
{
    internal class ExcelRecord
    {
        public int RowNumber { get; set; }
        public decimal Quantity { get; set; }
        public string? Measurement { get; set; }
        public string? Item { get; set; }
        public string? ShipName { get; set; }


        public override string ToString()
        {
            return $"Row: {RowNumber}, Qty: {Quantity}, Meas: {Measurement}, Item: {Item}, Ship: {ShipName}";
        }

    }
}
