using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApp.MVVM.Models
{
    internal class Measurement
    {
        public int MeasurementId { get; set; }
        public string OriginalMeasurementName { get; set; }
        public string CleanMeasurementName { get; set; }

        public override string ToString()
        {
            return $"Measurement {{ Id: {MeasurementId}, Original: \"{OriginalMeasurementName}\", Clean: \"{CleanMeasurementName}\" }}";
        }
    }
}
