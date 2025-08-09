using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using StarBankApp.Controllers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Net.Http.Headers;
using System.ComponentModel;
using StarBankApp.Models;
using System.Windows.Input;



namespace StarBankApp.Views
{
    public partial class TransactionsView : ContentPage
    {
        private UsersDB controller;
        private readonly int accountId;
        public TransactionsView(int accountId)
        {
            InitializeComponent();
            controller = new UsersDB();
            this.accountId = accountId;
            InitController();

        }

        private async void InitController()
        {
            await controller.Init();
            await CargarMovimientosAsync(accountId);
        }


        private async Task CargarMovimientosAsync(int accountId)
        {
            var ultimoUsuario = await controller.GetUltimoUsuario();
            if (ultimoUsuario != null)
            {
                Console.WriteLine(ultimoUsuario.token);

                string url = $"{ApiConfig.BaseUrl}cuenta/findtransacciones/{accountId}/transacciones";
                string json = await FetchAccountData(url, ultimoUsuario.token);

                if (!string.IsNullOrEmpty(json))
                {
                    // Establecer el BindingContext
                    Console.WriteLine("estos son los datos de la cuenta ");
                    Console.WriteLine($"{json}");
                    BindingContext = new AccountTransactionsViewModel(json);
                }
            }
            else
            {
                await DisplayAlert("Aviso", "No se pudo obtener el nombre del usuario.", "OK");
            }

        }


        private async Task<string> FetchAccountData(string url, string token)
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
                    await DisplayAlert("Error", "No se pudo obtener los datos de la cuenta.", "OK");
                    return null;
                }
            }
        }

        private void btnHome_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UserAccountsScreen());
        }

    }


    public class AccountTransactionsViewModel : INotifyPropertyChanged
    {
        public AccountTransactionsViewModel(string json)
        {
            Cuenta = DeserializeJson(json);
            Transaccions = new ObservableCollection<Transaccion>(Cuenta.Transaccion ?? new List<Transaccion>());
            NavigateHomeCommand = new Command(NavigateHome);
        }

        private Cuenta cuenta;
        public Cuenta Cuenta
        {
            get => cuenta;
            set
            {
                cuenta = value;
                OnPropertyChanged(nameof(Cuenta));
            }
        }

        public ObservableCollection<Transaccion> Transaccions { get; set; }

        private Cuenta DeserializeJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentException("El JSON proporcionado está vacío o es nulo.", nameof(json));
            }

            var cuenta = JsonConvert.DeserializeObject<Cuenta>(json);
            if (cuenta == null)
            {
                throw new InvalidOperationException("No se pudo deserializar el JSON a un objeto Cuenta.");
            }

            return cuenta;
        }


        public ICommand NavigateHomeCommand { get; }

        private async void NavigateHome()
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class Cuenta
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("no_cuenta")]
        public string NoCuenta { get; set; }

        [JsonProperty("id_cliente")]
        public int IdCliente { get; set; }

        [JsonProperty("moneda")]
        public string Moneda { get; set; }

        [JsonProperty("saldo")]
        public double Saldo { get; set; }

        [JsonProperty("simbolo")]
        public string Simbolo { get; set; }
        public string SaldoConSimbolo => $"{Simbolo}{Saldo:N2}";


        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }

        [JsonProperty("Transaccions")]
        public List<Transaccion> Transaccion { get; set; }


    }

    public class Transaccion
    {
        private string fecha;
        [JsonProperty("fecha")]
        public string Fecha
        {
            get => fecha;
            set
            {
                fecha = value;
                FechaDateTime = DateTime.Parse(fecha);
            }
        }

        public DateTime FechaDateTime { get; set; }

        [JsonProperty("tipo_movimiento")]
        public string TipoMovimiento { get; set; }

        [JsonProperty("monto")]
        public string Monto { get; set; }
    }




}




