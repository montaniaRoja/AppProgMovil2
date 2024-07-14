namespace StarBankApp.Controllers;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class ExchangeRateService
{
    private readonly HttpClient _httpClient;

    public ExchangeRateService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<ExchangeRateResponse> GetExchangeRatesAsync()
    {
        try
        {
            // Reemplaza la URL de la API con la URL real de la API que estás usando
            string apiUrl = "https://api.currencyapi.com/v3/latest?apikey=cur_live_h6iWhOIfvt2kBXV7oHOEwdokQI2SKmBGsjAXqZPK";

            // Realiza la solicitud GET a la API
            var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>(apiUrl);

            // Devuelve los datos recibidos
            return response;
        }
        catch (Exception ex)
        {
            // Maneja cualquier error que ocurra durante la solicitud
            Console.WriteLine($"Error fetching exchange rates: {ex.Message}");
            return null;
        }
    }
}
