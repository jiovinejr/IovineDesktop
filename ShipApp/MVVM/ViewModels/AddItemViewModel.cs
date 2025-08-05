using ShipApp.Data;
using ShipApp.MVVM.Models;
using ShipApp.Service;
using ShipApp.Core;
using System.Windows.Input;
using System.Diagnostics;

namespace ShipApp.MVVM.ViewModels
{
    public class AddItemViewModel : BaseViewModel
    {
        private readonly ItemService _itemService = new();
        private readonly TaskCompletionSource<Item> _completionSource;
        private readonly RelayCommand _saveItemCommand;

        public List<InventoryItem> InventoryItems { get; }
        public List<QbItem> QbItems { get; }

        // Properties bound to form fields
        private string _originalItemName;
        public string OriginalItemName
        {
            get => _originalItemName;
            set
            {
                SetProperty(ref _originalItemName, value);
                //_saveItemCommand.RaiseCanExecuteChanged();
            }
        }

        private string _cleanItemName;
        public string CleanItemName
        {
            get => _cleanItemName;
            set
            {
                SetProperty(ref _cleanItemName, value);
                _saveItemCommand.RaiseCanExecuteChanged();
                Debug.WriteLine(value);
            }
        }

        private string _caseWeightText = string.Empty;
        public string CaseWeightText
        {
            get => _caseWeightText;
            set
            {
                SetProperty(ref _caseWeightText, value);
                // Try parse and update CaseWeight
                if (decimal.TryParse(value, out var parsed))
                    CaseWeight = parsed;
                else
                    CaseWeight = 0;

                _saveItemCommand.RaiseCanExecuteChanged();
            }
        }

        private decimal _caseWeight;
        public decimal CaseWeight
        {
            get => _caseWeight;
            set => SetProperty(ref _caseWeight, value);
        }


        private InventoryItem _selectedInventoryItem;
        public InventoryItem SelectedInventoryItem
        {
            get => _selectedInventoryItem;
            set
            {
                SetProperty(ref _selectedInventoryItem, value);
                _saveItemCommand.RaiseCanExecuteChanged();
            }
        }

        private QbItem _selectedQbItem;
        public QbItem SelectedQbItem
        {
            get => _selectedQbItem;
            set
            {
                SetProperty(ref _selectedQbItem, value);
                _saveItemCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand SaveItemCommand => _saveItemCommand;

        public AddItemViewModel(string originalName, TaskCompletionSource<Item> completionSource)
        {
            _completionSource = completionSource;
            InventoryItems = AppDataStore.Instance.InventoryItems;
            QbItems = AppDataStore.Instance.QbItems;

            OriginalItemName = originalName;

            _saveItemCommand = new RelayCommand(async () => await SaveItem(), CanSave);
        }

        private async Task SaveItem()
        {
            Debug.WriteLine("CLICKED!");
            try
            {
                var newItem = new Item
                {
                    OriginalItemName = this.OriginalItemName,
                    CleanItemName = this.CleanItemName,
                    CaseWeight = this.CaseWeight,
                    ItemInventoryId = this.SelectedInventoryItem?.Id ?? 0,
                    ItemQbId = this.SelectedQbItem?.Id ?? 0
                };

                var inserted = _itemService.InsertNewItem(newItem);
                _completionSource.SetResult(inserted);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Failed to add item: {ex.Message}");
                _completionSource.SetException(ex);
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(CleanItemName)
                   && CaseWeight > 0
                   && SelectedInventoryItem != null
                   && SelectedQbItem != null;
        }
    }
}
