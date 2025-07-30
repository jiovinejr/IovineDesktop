using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApp.MVVM.Models
{
    internal class Item
    {
        public int ItemId { get; set; }
        public string OriginalItemName { get; set; }
        public string CleanItemName { get; set; }
        public decimal CaseWeight { get; set; }
        public int ItemInventoryId { get; set; }
        public int ItemQbId { get; set; }
    }
}
