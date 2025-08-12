using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Services;
using ShipApp.Core;
using ShipApp.Data;
using ShipApp.MVVM.Models;
using System.Diagnostics;
using System.Windows.Input;
using IQueryAttributable = Microsoft.Maui.Controls.IQueryAttributable;

namespace ShipApp.MVVM.ViewModels
{
    // This VM is constructed by PopupService
    public class AddItemViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IPopupService _popupService;
        private readonly RelayCommand _saveItemCommand;
        private readonly RelayCommand _cancelCommand;

        public List<InventoryItem> InventoryItems { get; }
        public List<QbItem> QbItems { get; }

        public string OriginalItemName { get => _originalItemName; set => SetProperty(ref _originalItemName, value); }
        private string _originalItemName = string.Empty;

        public string CleanItemName { get => _cleanItemName; set { SetProperty(ref _cleanItemName, value); _saveItemCommand.RaiseCanExecuteChanged(); } }
        private string _cleanItemName = string.Empty;

        public string CaseWeightText
        {
            get => _caseWeightText;
            set
            {
                SetProperty(ref _caseWeightText, value);
                CaseWeight = decimal.TryParse(value, out var d) ? d : 0;
                _saveItemCommand.RaiseCanExecuteChanged();
            }
        }
        private string _caseWeightText = string.Empty;

        public decimal CaseWeight { get => _caseWeight; set => SetProperty(ref _caseWeight, value); }
        private decimal _caseWeight;

        public InventoryItem? SelectedInventoryItem
        {
            get => _selectedInventoryItem;
            set { SetProperty(ref _selectedInventoryItem, value); _saveItemCommand.RaiseCanExecuteChanged(); }
        }
        private InventoryItem? _selectedInventoryItem;

        public QbItem? SelectedQbItem
        {
            get => _selectedQbItem;
            set { SetProperty(ref _selectedQbItem, value); _saveItemCommand.RaiseCanExecuteChanged(); }
        }
        private QbItem? _selectedQbItem;

        public ICommand SaveItemCommand => _saveItemCommand;
        public ICommand CancelCommand => _cancelCommand;

        public AddItemViewModel(IPopupService popupService)
        {
            _popupService = popupService;

            // load options (from your in-memory store)
            InventoryItems = AppDataStore.Instance.InventoryItems;
            QbItems = AppDataStore.Instance.QbItems;

            _saveItemCommand = new RelayCommand(async () => await SaveAsync(), CanSave);
            _cancelCommand = new RelayCommand(async () => await _popupService.ClosePopupAsync(Shell.Current));
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Debug.WriteLine($"[Popup] keys: {string.Join(", ", query.Keys)}");
            if (query.TryGetValue(nameof(OriginalItemName), out var v) && v is string s)
                OriginalItemName = s;
        }

        private bool CanSave() =>
            !string.IsNullOrWhiteSpace(CleanItemName) &&
            CaseWeight > 0 &&
            SelectedInventoryItem is not null &&
            SelectedQbItem is not null;

        private async Task SaveAsync()
        {
            // return a NEW, unsaved Item (service will insert)
            var newItem = new Item
            {
                OriginalItemName = OriginalItemName,
                CleanItemName = CleanItemName,
                CaseWeight = CaseWeight,
                ItemInventoryId = SelectedInventoryItem?.Id ?? 0,
                ItemQbId = SelectedQbItem?.Id ?? 0
            };

            await _popupService.ClosePopupAsync(Shell.Current, newItem);
        }
    }
}
