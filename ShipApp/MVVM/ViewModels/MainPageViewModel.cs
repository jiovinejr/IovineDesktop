using System.Collections.ObjectModel;
using System.Windows.Input;
using ShipApp.Core;
using ShipApp.MVVM.Models;
using ShipApp.MVVM.Views;

namespace ShipApp.MVVM.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private ContentView _currentContent;
        public ObservableCollection<NavigationItem> NavigationItems { get; set; }

        public ContentView CurrentContent
        {
            get => _currentContent;
            set
            {
                _currentContent = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateCommand { get; }

        public MainPageViewModel()
        {
            NavigationItems = new ObservableCollection<NavigationItem>
            {
                new NavigationItem { Name = "Home", ViewName = "Home", IsSelected = true },
                new NavigationItem { Name = "Inventory", ViewName = "Inventory", IsSelected = false },
                new NavigationItem { Name = "Settings", ViewName = "Settings", IsSelected = false }
            };

            NavigateCommand = new RelayCommand<NavigationItem>(OnNavigate);

            CurrentContent = new HomeView();
        }

        private void OnNavigate(NavigationItem selectedItem)
        {
            if (selectedItem == null) return;

            foreach (var item in NavigationItems)
                item.IsSelected = false;

            selectedItem.IsSelected = true;

            CurrentContent = selectedItem.ViewName switch
            {
                "Home" => new HomeView(),
                "Inventory" => new InventoryView(),
                "Settings" => new SettingsView(),
                _ => new HomeView()
            };
        }
    }
}
