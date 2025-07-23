using ShipApp.Core;

namespace ShipApp.MVVM.Models
{
    public class NavigationItem : BaseViewModel
    {
        private bool _isSelected;

        public string? Name { get; set; }
        public string? ViewName { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
