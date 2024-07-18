using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using StarBankApp.Controllers;
using StarBankApp.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
//using Windows.System;

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
                Console.WriteLine(ultimoUsuario.token);

                string url = $"http://34.42.1.3:3000/api/cliente/findWithToken/{ultimoUsuario.userId}/cuentas";
                string json = await FetchClientData(url, ultimoUsuario.token);

                if (!string.IsNullOrEmpty(json))
                {
                    // Establecer el BindingContext
                    BindingContext = new UserAccountsViewModel(json);
                }
            }
            else
            {
                await DisplayAlert("Aviso", "No se pudo obtener el nombre del usuario.", "OK");
            }
        }

        private async Task<string> FetchClientData(string url, string token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo obtener los datos del cliente.", "OK");
                    return null;
                }
            }
        }

        private async void CerrarSesion_Clicked(object sender, EventArgs e)
        {
            // Obtiene el último usuario
            var ultimoUsuario = await controller.GetUltimoUsuario();
            if (ultimoUsuario != null)
            {
                // Actualiza el token del usuario a una cadena vacía
                ultimoUsuario.token = string.Empty;

                // Actualiza el usuario en la base de datos
                await controller.ActualizarUsuario(ultimoUsuario);

                // Limpia la pila de navegación y navega a la página principal
                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
        }





    }

    public class UserAccountsViewModel : INotifyPropertyChanged
    {
        private Cliente cliente;
        public Cliente Cliente
        {
            get => cliente;
            set
            {
                cliente = value;
                OnPropertyChanged(nameof(Cliente));
            }
        }

        public ObservableCollection<Account> Accounts { get; set; }
        public ICommand ViewHistoryCommand { get; set; }
        public ICommand MakePaymentCommand { get; set; }
        public ICommand TransferCommand { get; set; }

        public UserAccountsViewModel(string json)
        {
            Cliente = DeserializeJson(json);
            Accounts = new ObservableCollection<Account>(Cliente.Cuenta);

            ViewHistoryCommand = new Command<Account>(ViewHistory);
            MakePaymentCommand = new Command<Account>(MakePayment);
            TransferCommand = new Command<Account>(Transfer);
        }

        private void ViewHistory(Account account)
        {
            // Lógica para ver historial
        }

        private void MakePayment(Account account)
        {
            // Lógica para realizar un pago
        }

        private void Transfer(Account account)
        {
            // Lógica para transferencias
        }

        private Cliente DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<Cliente>(json);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Cliente
    {
        public int Id { get; set; }
        public string NoDoc { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }
        public string Usuario { get; set; }
        public int ClaveSecreta { get; set; }
        public string Keyword { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<Account> Cuenta { get; set; }
    }

    public class Account
    {
        public int Id { get; set; }
        public string No_Cuenta { get; set; }
        public string Moneda { get; set; }
        public decimal Saldo { get; set; }
    }
}
