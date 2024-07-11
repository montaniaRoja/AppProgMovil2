namespace StarBankApp.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}
	private void AddUserScreen_Clicked(object sender, EventArgs e)
	{
        Navigation.PushAsync(new Views.AddUserScreen());
    }

    private void ChangeTypeScreen_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Views.ChangeTypeScreen());
    }

}