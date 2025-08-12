using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using ShipApp.Core;
using ShipApp.MVVM.Models;
using ShipApp.MVVM.Views;

namespace ShipApp.MVVM.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly IServiceProvider _services;

        private ContentView _currentContent = null!;
        public ContentView CurrentContent
        {
            get => _currentContent;
            set { _currentContent = value; OnPropertyChanged(); }
        }

        public ObservableCollection<NavigationItem> NavigationItems { get; }
        public ICommand NavigateCommand { get; }

        // DI constructor
        public MainPageViewModel(IServiceProvider services)
        {
            _services = services;

            NavigationItems = new ObservableCollection<NavigationItem>
            {
                new() { Name = "Home",      ViewType = typeof(HomeView),      IsSelected = true  },
                new() { Name = "Inventory", ViewType = typeof(InventoryView), IsSelected = false },
                new() { Name = "Settings",  ViewType = typeof(SettingsView),  IsSelected = false }
            };

            NavigateCommand = new RelayCommand<NavigationItem>(OnNavigate);

            // set initial view via DI
            CurrentContent = (ContentView)_services.GetRequiredService(typeof(HomeView));
        }

        private void OnNavigate(NavigationItem? selected)
        {
            if (selected is null) return;

            foreach (var n in NavigationItems) n.IsSelected = false;
            selected.IsSelected = true;

            // resolve a fresh instance each time
            CurrentContent = (ContentView)_services.GetRequiredService(selected.ViewType);
        }
    }
}
