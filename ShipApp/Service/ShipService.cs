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
        public Ship AddNewShipWithRollback(Ship ship)
        {
            using var conn = DbConnectionFactory.CreateConnection();
            conn.Open();

            using var transaction = conn.BeginTransaction(); // Start transaction

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = transaction;
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

                // Rollback instead of commit to avoid saving during test
                transaction.Rollback();
                Debug.WriteLine($"{ship.ShipName} in database!");
                return ship;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Debug.WriteLine($"❌ Insert failed: {ex.Message}");
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

    }
}
