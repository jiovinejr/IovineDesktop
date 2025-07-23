using Npgsql;
using ShipApp.Data;
using ShipApp.MVVM.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ShipApp.Service
{
    class FileUploadService
    {
        public void InsertFile(FileUpload file)
        {
            try
            {
                using var conn = DbConnectionFactory.CreateConnection();
                conn.Open();

                string sql = @"INSERT INTO file_uploads (file_name, file_drive_id, is_processed, time_uploaded)
                       VALUES (@fileName, @fileDriveId, @isProcessed, @timeUploaded)";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("fileName", file.FileName);
                cmd.Parameters.AddWithValue("fileDriveId", file.FileDriveId);
                cmd.Parameters.AddWithValue("isProcessed", file.IsProcessed);
                cmd.Parameters.AddWithValue("timeUploaded", file.TimeUploaded);

                cmd.ExecuteNonQuery();
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
            finally
            {
                GetFilesToProcess();
            }
        }

        public ObservableCollection<FileUpload> GetFilesToProcess() 
        {
            ObservableCollection<FileUpload> filesToProcess = new ObservableCollection<FileUpload>();

            try
            {
                using var conn = DbConnectionFactory.CreateConnection();
                conn.Open();

                //NEED TO WRITE THIS CODE
                string sql = @"SELECT id, file_name, file_drive_id, is_processed, time_uploaded 
                       FROM file_uploads 
                       WHERE is_processed = false";

                using var cmd = new NpgsqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var file = new FileUpload
                    {
                        Id = reader.GetInt64(reader.GetOrdinal("id")),
                        FileName = reader.GetString(reader.GetOrdinal("file_name")),
                        FileDriveId = reader.GetString(reader.GetOrdinal("file_drive_id")),
                        IsProcessed = reader.GetBoolean(reader.GetOrdinal("is_processed")),
                        TimeUploaded = reader.GetDateTime(reader.GetOrdinal("time_uploaded"))
                    };

                    filesToProcess.Add(file);
                }

                return filesToProcess;
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

    }
}
