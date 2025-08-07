using ShipApp.Data;
using System.Collections.Generic;
using System.Diagnostics;
using Npgsql;
using ShipApp.MVVM.Models;

namespace ShipApp.Service
{
    public class InventoryService
    {
        public List<InventoryItem> GetAllInventoryItems()
        {
            List<InventoryItem> items = new();

            try
            {
                using var conn = DbConnectionFactory.CreateConnection();
                conn.Open();

                string sql = @"SELECT id, category, inventory_name FROM inventory_item ORDER BY inventory_name ASC";
                using var cmd = new NpgsqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    items.Add(new InventoryItem
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Category = reader.GetString(reader.GetOrdinal("category")),
                        InventoryName = reader.GetString(reader.GetOrdinal("inventory_name"))
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ InventoryService Error: {ex.Message}");
                throw;
            }

            return items;
        }
    }
}
