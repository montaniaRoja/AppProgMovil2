using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using System.Net.Http;
using Microsoft.Maui.Controls;
using StarBankApp.Controllers;
using System.Text.Json;
using StarBankApp.Models;

namespace StarBankApp.Views
{
    public partial class MainPage : ContentPage
    {
        private UsersDB controller;

        public MainPage()
        {
            InitializeComponent();
            controller = new UsersDB();
            InitController();
            NavigationPage.SetHasBackButton(this, false);
        }

        private async void InitController()
        {
            await controller.Init();
            await SetUltimoUsuario();
        }

        private async Task SetUltimoUsuario()
        {
            var ultimoUsuario = await controller.GetUltimoUsuario();
            if (ultimoUsuario != null)
            {
                txtUsuario.Text = ultimoUsuario.usuario;
            }
        }

        private void AddUserScreen_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddUserScreen());
        }

        private void ChangeTypeScreen_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ChangeTypeScreen());
        }

        private async void UserLogin_Clicked(object sender, EventArgs e)
        {
            var usuario=txtUsuario.Text;
            var password=txtPassword.Text;

            if (string.IsNullOrEmpty(usuario) ||
               string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Verifique todos los campos", "OK");
                return;
            }

            await LoginUsuario(usuario, password);
        }

        private async Task LoginUsuario(string usuario, string password)
        {
            using (var client = new HttpClient())
            {
                var uri = new Uri($"http://34.42.1.3:3000/api/cliente/finduser/{Uri.EscapeDataString(usuario)}/{Uri.EscapeDataString(password)}");
                try
                {
                    var response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseContent);

                        var userResponse = JsonSerializer.Deserialize<UserResponse>(responseContent);

                        if (userResponse != null && userResponse.user != null)
                        {
                            var dbUser = await controller.GetUltimoUsuario();
                            if (dbUser != null)
                            {
                                dbUser.userId = userResponse.user.id;
                                dbUser.no_doc = userResponse.user.no_doc;
                                dbUser.name = userResponse.user.nombre;
                                dbUser.correo = userResponse.user.correo;
                                dbUser.direccion = userResponse.user.direccion;
                                dbUser.token = userResponse.token;
                                dbUser.usuario = userResponse.user.usuario;

                                int result = await controller.UpdateUsuario(dbUser);
                                Console.WriteLine($"UpdateUsuario result: {result}"); // Registro del resultado
                                /*
                                if (result > 0)
                                {
                                    await DisplayAlert("Éxito", "El usuario ha sido actualizado.", "OK");
                                }
                                else
                                {
                                    await DisplayAlert("Error", "El usuario no se pudo actualizar en la base local.", "OK");
                                }
                                */
                            }
                            else
                            {
                                dbUser.userId = userResponse.user.id;
                                dbUser.no_doc = userResponse.user.no_doc;
                                dbUser.name = userResponse.user.nombre;
                                dbUser.correo = userResponse.user.correo;
                                dbUser.direccion = userResponse.user.direccion;
                                dbUser.token = userResponse.token;
                                dbUser.usuario = userResponse.user.usuario;
                                int nuevoUsuario = await controller.InsertarNuevoUsuario(dbUser);
                            }

                        }

                        Application.Current.MainPage = new NavigationPage(new UserAccountsScreen());
                        //await Navigation.PushAsync(new Views.UserAccountsScreen());
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
