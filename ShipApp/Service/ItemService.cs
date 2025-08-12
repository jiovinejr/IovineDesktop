using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Services;
using Npgsql;
using ShipApp.Data;
using ShipApp.MVVM.Models;
using ShipApp.MVVM.ViewModels;
using System.Diagnostics;

namespace ShipApp.Service
{
    public class ItemService
    {
        private readonly IPopupService _popupService;

        public ItemService(IPopupService popupService)
        {
            _popupService = popupService;
        }

        public Item InsertNewItem(Item item)
        {
            try
            {
                using var conn = DbConnectionFactory.CreateConnection();
                conn.Open();

                const string sql = @"
                    INSERT INTO item (original_item_name, clean_item_name, case_weight, item_inventory_id, item_qb_id)
                    VALUES (@original, @clean, @weight, @inventoryId, @qbId)
                    RETURNING item_id";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("original", item.OriginalItemName);
                cmd.Parameters.AddWithValue("clean", item.CleanItemName);
                cmd.Parameters.AddWithValue("weight", item.CaseWeight);
                cmd.Parameters.AddWithValue("inventoryId", item.ItemInventoryId);
                cmd.Parameters.AddWithValue("qbId", item.ItemQbId);

                item.ItemId = (int)cmd.ExecuteScalar();
                return item;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ InsertNewItem failed: {ex.Message}");
                throw;
            }
        }

        public Item? GetItemByOriginalName(string originalName)
        {
            try
            {
                Item? result = null;
                const string sql = "SELECT * FROM item WHERE original_item_name = @originalName";

                using var conn = DbConnectionFactory.CreateConnection();
                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@originalName", originalName);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                    result = MapReaderToItem(reader);

                return result;
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

        private static Item MapReaderToItem(NpgsqlDataReader reader) => new()
        {
            ItemId = reader.GetInt32(reader.GetOrdinal("item_id")),
            OriginalItemName = reader.GetString(reader.GetOrdinal("original_item_name")),
            CleanItemName = reader.GetString(reader.GetOrdinal("clean_item_name")),
            CaseWeight = reader.GetDecimal(reader.GetOrdinal("case_weight")),
            ItemInventoryId = reader.IsDBNull(reader.GetOrdinal("item_inventory_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("item_inventory_id")),
            ItemQbId = reader.IsDBNull(reader.GetOrdinal("item_qb_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("item_qb_id"))
        };

        // Show popup, collect NEW item, then insert it
        public async Task<Item?> HandleUnknownItemAsync(string originalItemName)
        {
            try
            {
                var args = new Dictionary<string, object>
                {
                    [nameof(AddItemViewModel.OriginalItemName)] = originalItemName
                };

                IPopupResult<Item?> popupResult =
                    await _popupService.ShowPopupAsync<AddItemViewModel, Item?>(
                        Shell.Current,
                        options: PopupOptions.Empty,
                        shellParameters: args);

                var newItem = popupResult.Result; // null if canceled
                if (newItem is null) return null;

                return InsertNewItem(newItem); // persist and return with new ID
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Failed to handle unknown item: {ex.Message}");
                return null;
            }
        }
    }
}
