using ShipApp.MVVM.ViewModels;

namespace ShipApp.MVVM.Views;


public partial class HomeView : ContentView
{
	public HomeView()
	{
		InitializeComponent();
		BindingContext = new HomeViewModel();
    }
}