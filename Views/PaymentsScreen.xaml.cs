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
    
    public partial class PaymentsScreen : ContentPage
    {
        private UsersDB controller;
        private readonly int accountId;
        private readonly string accountNumber;
        public PaymentsScreen(int accountId, string accountNumber)
        {
            InitializeComponent();
            controller = new UsersDB();
            this.accountId = accountId;
            this.accountNumber = accountNumber;
            InitController();
            ObtenerListaDePagos(accountNumber);
            

        }
        private async void InitController()
        {
            await controller.Init();
            
        }

        private async void ObtenerListaDePagos(string accountNumber)
        {
            txtNumeroDeCuenta.Text = accountNumber;
            string url = $"http://34.42.1.3:3000/api/pago/list";
            string json=await FetchClientData(url);

            if (!string.IsNullOrEmpty(json))
            {
                Console.WriteLine("lista de pagos ");
                Console.WriteLine(json);
                // Pasa la instancia de Navigation al ViewModel
                BindingContext = new PagosViewModel(json);

            }


        }

        private async Task<string> FetchClientData(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo obtener la lista de pagos", "OK");
                    return null;
                }
            }
        }

    }

    public class PagosViewModel : INotifyPropertyChanged
    {
        public PagosViewModel(string json)
        {
            Pagos = DeserializeJson(json);
        }

        public ObservableCollection<Pago> Pagos { get; set; }

        private ObservableCollection<Pago> DeserializeJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentException("El JSON proporcionado está vacío o es nulo.", nameof(json));
            }

            var pagos = JsonConvert.DeserializeObject<List<Pago>>(json);
            if (pagos == null)
            {
                throw new InvalidOperationException("No se pudo deserializar el JSON a una lista de objetos Pago.");
            }

            return new ObservableCollection<Pago>(pagos);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Pago
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("nombre_pago")]
        public string Nombrepago { get; set; }
        [JsonProperty("createdAt")]
        public string Createdat { get; set; }
        [JsonProperty("UpdatedAt")]
        public string Updatedat { get; set; }
    }

}
