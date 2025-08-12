using Npgsql;
using ShipApp.Data;
using ShipApp.MVVM.Models;
using ShipApp.MVVM.Views;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApp.Service
{
    public class MeasurementService
    {
        public Measurement InsertNewMeasurement(Measurement measurement)
        {
            try
            {
                using var conn = DbConnectionFactory.CreateConnection();
                conn.Open();

                string sql = @"INSERT INTO measurement (original_measurement_name, clean_measurement_name)
                               VALUES (@original, @clean)
                               RETURNING measurement_id";
                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("original", measurement.OriginalMeasurementName);
                cmd.Parameters.AddWithValue("clean", measurement.CleanMeasurementName);

                int insertedId = (int)cmd.ExecuteScalar();
                measurement.MeasurementId = insertedId;
                return measurement;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ InsertNewItem failed: {ex.Message}");
                throw;
            }
        }
        public Measurement GetMeasurementObjectByOriginalName(string originalName)
        {
            Measurement result = null;

            string sql = "SELECT * FROM measurement WHERE original_measurement_name = @name";

            using (var conn = DbConnectionFactory.CreateConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("name", originalName);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = MapReaderToMeasurement(reader);
                        }
                    }
                }
            }

            return result;
        }


        public static Measurement MapReaderToMeasurement(DbDataReader reader)
        {
            return new Measurement
            {
                MeasurementId = reader.GetInt32(reader.GetOrdinal("measurement_id")),
                OriginalMeasurementName = reader.GetString(reader.GetOrdinal("original_measurement_name")),
                CleanMeasurementName = reader.GetString(reader.GetOrdinal("clean_measurement_name"))
            };
        }

        public async Task<Measurement?> HandleUnknownMeasurement(string originalMeasurementName)
        {
            try
            {
                var tcs = new TaskCompletionSource<Measurement>();
                var addMeasurementPage = new AddMeasurementPage(originalMeasurementName, tcs);

                await Shell.Current.Navigation.PushModalAsync(addMeasurementPage);

                var measurement = await tcs.Task;
                await Shell.Current.Navigation.PopModalAsync();

                return measurement;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Failed to handle unknown measurement: {ex.Message}");
                return null;
            }
        }
    }
}
