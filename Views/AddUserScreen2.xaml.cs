using System.Text;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using StarBankApp.Controllers;

namespace StarBankApp.Views
{
    public partial class AddUserScreen2 : ContentPage
    {
        private UsersDB controller;

        public AddUserScreen2()
        {
            InitializeComponent();
            controller = new UsersDB();
            InitController();
            NavigationPage.SetHasBackButton(this, false);
        }

        private async void InitController()
        {
            await controller.Init();
        }

        private async void CrearUsuario_Clicked(object sender, EventArgs e)
        {
            var no_cuenta = txtCuenta.Text;
            var usuario = txtUsuario.Text;
            var keyword = txtKeyword.Text;
            var repeatKeyword = txtVerificar.Text;
            int clave = int.Parse(txtClave.Text);

            if (string.IsNullOrEmpty(no_cuenta) ||
                string.IsNullOrEmpty(usuario) ||
                string.IsNullOrEmpty(keyword))
            {
                await DisplayAlert("Error", "Verifique todos los campos", "OK");
                return;
            }

            if (keyword != repeatKeyword)
            {
                await DisplayAlert("Error", "Las contraseñas no son iguales", "OK");
                return;
            }

            await EnviarUsuario(no_cuenta, usuario, keyword, clave);
        }

        private async Task EnviarUsuario(string cuenta, string usuario, string password, int clave)
        {
            using (var client = new HttpClient())
            {
                var uri = new Uri("http://192.168.1.78:3000/api/cuenta/findclave");
                var json = $"{{ \"no_cuenta\": \"{cuenta}\" , \"usuario\": \"{usuario}\" , \"keyword\": \"{password}\", \"clave\": {clave} }}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                Console.WriteLine(json); // Verifica que el JSON sea correcto

                try
                {
                    var response = await client.PostAsync(uri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        

                        try
                        {
                            if (controller != null)
                            {
                                int result = await controller.InsertUsuario(usuario);
                                Console.WriteLine($"InsertUsuario result: {result}"); // Registro del resultado

                                if (result > 0)
                                {
                                    await DisplayAlert("Aviso", "Usuario creado exitosamente", "OK");
                                    txtUsuario.Text = usuario;
                                }
                                else
                                {
                                    await DisplayAlert("Aviso", "El usuario no se creó en la base local", "OK");
                                }
                            }
                            else
                            {
                                await DisplayAlert("Error", "El controlador es nulo", "OK");
                            }
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Error", $"Ocurrió un error durante la inserción: {ex.Message}", "OK");
                        }

                        await Navigation.PushAsync(new MainPage());
                    }
                    else
                    {
                        await DisplayAlert("Error", "Hubo un problema al enviar la solicitud.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Ocurrió un error durante la solicitud HTTP: {ex.Message}", "OK");
                }
            }
        }
    }
}
