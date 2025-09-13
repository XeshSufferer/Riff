using Riff.ViewModels;

namespace Riff;

public partial class LoginPage : ContentPage
{

	private readonly LoginPageViewModel _vm;

	public LoginPage(LoginPageViewModel vm)
	{
		InitializeComponent();
		_vm = vm;
		BindingContext = vm;
	}

	protected override void OnDisappearing()
	{
		_vm.Dispose();
		base.OnDisappearing();
	}

    protected override void OnAppearing()
    {
        _vm.Sub();
		base.OnAppearing();
    }
}