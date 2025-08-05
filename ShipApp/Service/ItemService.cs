using Npgsql;
using ShipApp.Data;
using ShipApp.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApp.Service
{
    internal class ItemService
    {
        public Item InsertNewItem(Item item)
        {
            try
            {
                using var conn = DbConnectionFactory.CreateConnection();
                conn.Open();

                string sql = @"
            INSERT INTO item (original_item_name, clean_item_name, case_weight, item_inventory_id, item_qb_id)
            VALUES (@original, @clean, @weight, @inventoryId, @qbId)
            RETURNING item_id";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("original", item.OriginalItemName);
                cmd.Parameters.AddWithValue("clean", item.CleanItemName);
                cmd.Parameters.AddWithValue("weight", item.CaseWeight);
                cmd.Parameters.AddWithValue("inventoryId", item.ItemInventoryId);
                cmd.Parameters.AddWithValue("qbId", item.ItemQbId);

                int insertedId = (int)cmd.ExecuteScalar();
                item.ItemId = insertedId;
                return item;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ InsertNewItem failed: {ex.Message}");
                throw;
            }
        }


        public Item GetItemByOriginalName(string originalName)
        {
            try
            {
                Item result = null;
                string sql = "SELECT * FROM item WHERE original_item_name = @originalName";

                using (var conn = DbConnectionFactory.CreateConnection())
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@originalName", originalName);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = MapReaderToItem(reader);
                        }
                    }
                }

                return result;
            }
            catch (Npgsql.PostgresException ex)
            {
                Debug.WriteLine($"❌ PostgresException: {ex.MessageText} | Code: {ex.SqlState} | Detail: {ex.Detail}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ General DB Error: {ex}");
                throw;
            }
        }


        private Item MapReaderToItem (NpgsqlDataReader reader)
        {
            return new Item
            {
                ItemId = reader.GetInt32(reader.GetOrdinal("item_id")),
                OriginalItemName = reader.GetString(reader.GetOrdinal("original_item_name")),
                CleanItemName = reader.GetString(reader.GetOrdinal("clean_item_name")),
                CaseWeight = reader.GetDecimal(reader.GetOrdinal("case_weight")),
                ItemInventoryId = reader.IsDBNull(reader.GetOrdinal("item_inventory_id")) 
                    ? 0 : reader.GetInt32(reader.GetOrdinal("item_inventory_id")),
                ItemQbId = reader.IsDBNull(reader.GetOrdinal("item_qb_id"))
                    ? 0 : reader.GetInt32(reader.GetOrdinal("item_qb_id"))
            };
        }
    }
}
