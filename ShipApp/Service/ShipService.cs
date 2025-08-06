using Microsoft.Maui.Controls;
using Npgsql;
using NpgsqlTypes;
using ShipApp.Data;
using ShipApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApp.Service
{
    internal class ShipService
    {

        public Ship GetShipByShipName(string shipName)
        {
            try
            {
                Ship ship = null;
                using var conn = DbConnectionFactory.CreateConnection();
                conn.Open();

                string sql = @"SELECT * FROM ship WHERE ship_name = @name";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("name", shipName);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    ship = MapShipToReader(reader);
                }
                return ship;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ShipService Error: {ex.Message}");
                throw;
            }
        }

        public int DeleteShipById (Ship ship)
        {
            try
            {
                using var conn = DbConnectionFactory.CreateConnection();
                conn.Open();

                string sql = @"DELETE FROM ship WHERE order_ship_id = @ship";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("ship", ship.ShipId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ShipService Error: {ex.Message}");
                throw;
            }
        }

        public Ship AddNewShip(Ship ship)
        {
            using var conn = DbConnectionFactory.CreateConnection();
            conn.Open();

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO ship (ship_name, complete, is_needed, date_completed, checked_stickers, is_printed)
                VALUES (@name, @complete, @needed, @date, @stickers, @printed)
                RETURNING ship_id;
                ";
                cmd.Parameters.AddWithValue("name", ship.ShipName);
                cmd.Parameters.AddWithValue("complete", ship.Complete);
                cmd.Parameters.AddWithValue("needed", ship.IsNeeded); // testing default
                cmd.Parameters.Add("date", NpgsqlDbType.Date).Value = ship.DateCompleted ?? (object)DBNull.Value;
                cmd.Parameters.AddWithValue("stickers", ship.CheckedStickers);
                cmd.Parameters.AddWithValue("printed", ship.IsPrinted);

                ship.ShipId = Convert.ToInt64(cmd.ExecuteScalar());

                Debug.WriteLine($"{ship.ShipName} in database!");
                return ship;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Insert failed: {ex.Message}");
                throw;
            }
        }

        public void UpdateShipIsNeededById(long shipId, bool isNeeded)
        {
            try
            {
                using var conn = DbConnectionFactory.CreateConnection();
                conn.Open();

                string sql = @"UPDATE ship SET is_needed = @isNeeded WHERE ship_id = @shipId";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("isNeeded", isNeeded);
                cmd.Parameters.AddWithValue("shipId", shipId);

                int rowsAffected = cmd.ExecuteNonQuery();
                Debug.WriteLine($"✅ Updated ship_id={shipId} to is_needed={isNeeded} ({rowsAffected} row(s) affected)");
            }
            catch (PostgresException ex)
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

        private Ship MapShipToReader(NpgsqlDataReader reader)
        {
            return new Ship
            {
                ShipId = reader.GetInt64(reader.GetOrdinal("ship_id")),
                ShipName = reader.GetString(reader.GetOrdinal("ship_name")),
                Complete = reader.GetBoolean(reader.GetOrdinal("complete")),
                IsNeeded = reader.GetBoolean(reader.GetOrdinal("is_needed")),
                DateCompleted = reader.IsDBNull(reader.GetOrdinal("date_completed"))
                                    ? (DateTime?)null
                                    : reader.GetDateTime(reader.GetOrdinal("date_completed")),
                CheckedStickers = reader.IsDBNull(reader.GetOrdinal("checked_stickers"))
                                    ? string.Empty
                                    : reader.GetString(reader.GetOrdinal("checked_stickers")),
                IsPrinted = reader.GetBoolean(reader.GetOrdinal("is_printed"))
            };
        }

    }
}
