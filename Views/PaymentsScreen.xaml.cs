using System;
using System.Text;
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
//using static UIKit.UIGestureRecognizer;



namespace StarBankApp.Views
{
    
    public partial class PaymentsScreen : ContentPage
    {
        private UsersDB controller;
        private readonly int accountId;
        private readonly string accountNumber;
        private readonly decimal saldoCuenta;
        public PaymentsScreen(int accountId, string accountNumber, decimal saldoCuenta)
        {
            InitializeComponent();
            controller = new UsersDB();
            this.accountId = accountId;
            this.accountNumber = accountNumber;
            this.saldoCuenta = saldoCuenta;
            InitController();
            ObtenerListaDePagos(accountNumber);
            

        }
        private async void InitController()
        {
            await controller.Init();
            
        }

        private async void btnPagar_Clicked(object sender, EventArgs e)
        {
            await EjecutarPago();

        }

        private async void btnBuscarPago_Clicked(object sender, EventArgs e)
        {
            var claveEnviar = txtClave.Text;
            var status = "Pendiente";

            string ruta = $"{ApiConfig.BaseUrl}detallepago/find/{claveEnviar}/{status}";

            string jsonMonto = await FetchPaymentData(ruta);

            if (!string.IsNullOrEmpty(jsonMonto))
            {
                Console.WriteLine("Datos de pago recibidos");
                Console.WriteLine(jsonMonto);

                try
                {
                    // Deserializa el JSON en un objeto PagoDetalle
                    var pagoDetalle = JsonConvert.DeserializeObject<PagoDetalle>(jsonMonto);

                    if (pagoDetalle != null)
                    {
                        // Asigna el valor de 'monto' al campo 'txtMonto'
                        txtMonto.Text = pagoDetalle.Monto.ToString("F2");
                    }
                }
                catch (JsonSerializationException ex)
                {
                    Console.WriteLine("Error deserializando el JSON: " + ex.Message);
                }
            }
        }


        private async Task<string> FetchPaymentData(string ruta)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(ruta);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    await DisplayAlert("Error", "Este pago no existe o ya fue cancelado.", "OK");
                    return null;
                }
            }
        }

        private async void ObtenerListaDePagos(string accountNumber)
        {
            txtNumeroDeCuenta.Text = accountNumber;
            string url = $"{ApiConfig.BaseUrl}pago/list";
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

        private void btnBack_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UserAccountsScreen());
        }

        private async Task EjecutarPago()
        {
            var clvPago = txtClave.Text;
            var noCuenta = accountId;
            var monto = Decimal.Parse(txtMonto.Text);
            var saldo = saldoCuenta;
            //var tipoMovimiento = "Pago " + txtPago.SelectedItem.ToString(); // Obtener el valor del picker

            if (monto > saldo)
            {
                await DisplayAlert("Error", "El pago excede el saldo en la cuenta", "OK");
                return;
            }

            var nuevoSaldo = saldo - monto;

            var selectedPago = txtPago.SelectedItem as Pago;
            if (selectedPago == null)
            {
                await DisplayAlert("Error", "Debe seleccionar un tipo de pago", "OK");
                return;
            }
            var tipoMovimiento = "Pago " + selectedPago.Nombrepago;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                   
                    var cuentaUrl = $"{ApiConfig.BaseUrl}cuenta/{noCuenta}";
                    var saldoJson = JsonConvert.SerializeObject(new { saldo = nuevoSaldo });
                    var saldoContent = new StringContent(saldoJson, Encoding.UTF8, "application/json");

                    var saldoRequest = new HttpRequestMessage(new HttpMethod("PATCH"), cuentaUrl)
                    {
                        Content = saldoContent
                    };

                    var saldoResponse = await client.SendAsync(saldoRequest);

                    if (!saldoResponse.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Error", "Error actualizando el saldo", "OK");
                        return;
                    }
                   
                    var pagoUrl = $"{ApiConfig.BaseUrl}detallepago/update/{clvPago}";

                    var statusRequest = new HttpRequestMessage(new HttpMethod("PATCH"), pagoUrl);

                    var statusResponse = await client.SendAsync(statusRequest);

                    if (!statusResponse.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Error", "Error actualizando el estado del pago", "OK");
                        return;
                    }

                    // Enviar la transacción a la dirección transaccion/create'
                    var transaccionUrl = $"{ApiConfig.BaseUrl}transaccion/create";
                    var transaccionJson = JsonConvert.SerializeObject(new
                    {
                        cuenta_id = noCuenta,
                        tipo_movimiento = tipoMovimiento,
                        monto = monto
                    });
                    var transaccionContent = new StringContent(transaccionJson, Encoding.UTF8, "application/json");
                    Console.WriteLine($"transaccionUrl: {transaccionUrl}");


                    var transaccionResponse = await client.PostAsync(transaccionUrl, transaccionContent);

                    if (!transaccionResponse.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Error", "Error registrando la transacción", "OK");
                        return;
                    }

                    await DisplayAlert("Éxito", "El pago se ha realizado correctamente", "OK");
                    await Navigation.PushAsync(new UserAccountsScreen());
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
                }
            }
        }


        // Clase para deserializar el JSON
        public class PagoDetalle
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("id_pago")]
            public int IdPago { get; set; }

            [JsonProperty("clavepago")]
            public string Clavepago { get; set; }

            [JsonProperty("monto")]
            public decimal Monto { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("createdAt")]
            public string CreatedAt { get; set; }

            [JsonProperty("updatedAt")]
            public string UpdatedAt { get; set; }
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
