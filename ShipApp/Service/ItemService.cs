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
