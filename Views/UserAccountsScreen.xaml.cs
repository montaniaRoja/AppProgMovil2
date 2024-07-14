using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using StarBankApp.Controllers;
using StarBankApp.Models;

namespace StarBankApp.Views
{
    public partial class UserAccountsScreen : ContentPage
    {
        private UsersDB controller;

        public UserAccountsScreen()
        {
            InitializeComponent();
            controller = new UsersDB();
            NavigationPage.SetHasBackButton(this, false);
            InitController();
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
                txtCliente.Text = ultimoUsuario.name;
            }
            else
            {
                await DisplayAlert("Aviso", "No se pudo obtener el nombre del usuario.", "OK");
            }
        }
    }
}
