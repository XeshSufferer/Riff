using Riff.ViewModels;

namespace Riff;

public partial class ChatPage : ContentPage
{

	private readonly ChatPageViewModel _vm;


	public ChatPage(ChatPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		_vm = vm;
	}

	protected override void OnAppearing()
	{
		_vm.Sub();
		base.OnAppearing();
	}

    protected override void OnDisappearing()
    {
        _vm.Dispose();
        base.OnDisappearing();
    }


}