
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace StarBankApp.Views
{
    public partial class AddUserScreen : ContentPage
    {
        public AddUserScreen()
        {
            InitializeComponent();
        }

        private async void SolicitarClave_Clicked(object sender, EventArgs e)
        {
            var correo = txtCorreo.Text;
            Console.WriteLine(correo);
            if (string.IsNullOrEmpty(correo))
            {
                await DisplayAlert("Error", "Por favor, ingrese un correo electrónico.", "OK");
                return;
            }

            await EnviarCorreo(correo);
        }

        private async Task EnviarCorreo(string correo)
        {
            using (var client = new HttpClient())
            {
                var uri = new Uri($"http://34.42.1.3:3000/api/cliente/findcorreo/?correo={Uri.EscapeDataString(correo)}");

                try
                {
                    var response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        await DisplayAlert("Éxito", "Favor Verifique si recibio clave de 6 digitos en su correo.", "OK");
                        await Navigation.PushAsync(new Views.AddUserScreen2());
                    }
                    else
                    {
                        await DisplayAlert("Error", "Hubo un problema al enviar la solicitud.", "OK");
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    await DisplayAlert("Error de conexión", $"Error de conexión: {httpEx.Message}", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
                }
            }
        }
    }
    }