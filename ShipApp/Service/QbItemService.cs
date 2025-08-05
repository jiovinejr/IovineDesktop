using Npgsql;
using ShipApp.Data;
using ShipApp.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ShipApp.Service
{
    public class QbItemService
    {
        public List<QbItem> GetAllQbItems ()
        { 
            List<QbItem> qbItems = new List<QbItem> ();
            
            try
            {
                using var conn = DbConnectionFactory.CreateConnection ();
                conn.Open ();

                string sql = @"SELECT id, qb_item_name, qb_item_description FROM qb_item order by qb_item_name ASC";
                using var cmd = new NpgsqlCommand (sql, conn);
                using var reader = cmd.ExecuteReader ();

                while (reader.Read ())
                {
                    int descOrdinal = reader.GetOrdinal("qb_item_description");
                    qbItems.Add(new QbItem
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        QbItemName = reader.GetString(reader.GetOrdinal("qb_item_name")),
                        QbItemDescription = reader.IsDBNull(descOrdinal) ? null : reader.GetString(descOrdinal),
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ InventoryService Error: {ex.Message}");
                throw;
            }
            return qbItems;
        }
    }
}
