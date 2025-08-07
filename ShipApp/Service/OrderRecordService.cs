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
    internal class OrderRecordService
    {
        public void InsertNewOrderRecord(ExcelRecord excelRecord, Ship ship, Item item, Measurement measurement)
        {
            OrderRecord rec = new OrderRecord
            {
                OrderLineId = excelRecord.RowNumber,
                OrderShipId = ship.ShipId,
                Quantity = excelRecord.Quantity,
                QuantityChecked = 0,
                OrderMeasurementId = measurement.MeasurementId,
                OrderItemId = item.ItemId,
                Checked = false,
                Notes = string.Empty,
            };

            try
            {
                using var conn = DbConnectionFactory.CreateConnection();
                conn.Open();

                string sql = @"INSERT INTO public.order_record(
	                            order_line_id, order_ship_id, quantity, quantity_checked, 
                                order_measurement_id, order_item_id, checked, notes)
                                VALUES(@line, @ship, @qty, @qtychx, @meas, @item, @ischx, @notes)";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("line", rec.OrderLineId);
                cmd.Parameters.AddWithValue("ship", rec.OrderShipId);
                cmd.Parameters.AddWithValue("qty", rec.Quantity);
                cmd.Parameters.AddWithValue("qtychx", rec.QuantityChecked);
                cmd.Parameters.AddWithValue("meas", rec.OrderMeasurementId);
                cmd.Parameters.AddWithValue("item", rec.OrderItemId);
                cmd.Parameters.AddWithValue("ischx", rec.Checked);
                cmd.Parameters.AddWithValue("notes", rec.Notes);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ OrderRecordService Error: {ex.Message}");
                throw;
            }    
        }

        public int DeleteOrderRecordsByShip(Ship ship)
        {
            try
            {
                using var conn = DbConnectionFactory.CreateConnection();
                conn.Open();

                string sql = @"DELETE FROM order_record WHERE order_ship_id = @ship";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("ship", ship.ShipId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ OrderRecordService Error: {ex.Message}");
                throw;
            }
        }
    }
}
