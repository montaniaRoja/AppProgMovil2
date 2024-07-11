using System.Text;
using System;
using System.Net.Http;

using System.Threading.Tasks;
using Microsoft.Maui.Controls;


namespace StarBankApp.Views;

public partial class AddUserScreen2 : ContentPage
{
	public AddUserScreen2()
	{
		InitializeComponent();
	}
	private async void CrearUsuario_Clicked(object sender, EventArgs e)
	{
		var no_cuenta=txtCuenta.Text;
		var usuario=txtUsuario.Text;
		var keyword=txtKeyword.Text;
		var repeatKeyword=txtVerificar.Text;
        int clave = int.Parse(txtClave.Text);

        if (string.IsNullOrEmpty(no_cuenta) ||
			string.IsNullOrEmpty(usuario) ||
			string.IsNullOrEmpty(keyword) )
        {
            await DisplayAlert("Error", "Verifique todos los campos", "OK");
            return;
        }

		if (keyword!=repeatKeyword)
		{
            await DisplayAlert("Error", "Las contraseñas no son iguales", "OK");
            return;
        }

		await EnviarUsuario(no_cuenta, usuario, keyword, clave);

    }

    private async Task EnviarUsuario(string a, string b, string c, int d)
    {
        using (var client = new HttpClient())
        {
            var uri = new Uri("http://34.42.1.3:3000/api/cuenta/findclave");
            var json = $"{{ \"no_cuenta\": \"{a}\" , \"usuario\": \"{b}\" , \"keyword\": \"{c}\", \"clave\": {d} }}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            Console.WriteLine(json);
            try
            {
                var response = await client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    // Manejar la respuesta exitosa aquí, si es necesario
                    await DisplayAlert("Éxito", "Solicitud enviada correctamente.", "OK");
                    await Navigation.PushAsync(new Views.AccountList());
                }
                else
                {
                    // Manejar la respuesta de error aquí, si es necesario
                    await DisplayAlert("Error", "Hubo un problema al enviar la solicitud.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Manejar las excepciones aquí
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }
    }

}