using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipApp.MVVM.Models;
using ShipApp.Service;

namespace ShipApp.Data
{
    public class AppDataStore
    {
        private static AppDataStore _instance;
        public static AppDataStore Instance => _instance ??= new AppDataStore();

        public List<InventoryItem> InventoryItems { get; private set; } = new();
        public List<QbItem> QbItems { get; private set; } = new();

        private AppDataStore() { }

        public void LoadData()
        {
            var inventoryService = new InventoryService();
            var qbItemService = new QbItemService();

            InventoryItems = inventoryService.GetAllInventoryItems();
            QbItems = qbItemService.GetAllQbItems();
        }

        public InventoryItem? FindInventoryById(int id) =>
            InventoryItems.FirstOrDefault(i => i.Id == id);

        public QbItem? FindQBItemById(int id) =>
            QbItems.FirstOrDefault(i => i.Id == id);
    }

}
