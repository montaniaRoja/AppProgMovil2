using StarBankApp.Controllers;

namespace StarBankApp.Views;

public partial class ChangeTypeScreen : ContentPage
{
    private readonly ExchangeRateService _exchangeRateService;
    public ChangeTypeScreen()
	{
		InitializeComponent();
        _exchangeRateService = new ExchangeRateService();
        //LoadExchangeRates();
        NavigationPage.SetHasBackButton(this, false);
    }

    private async void LempirasToDollars_Clicked(object sender, EventArgs e)
    {
        var exchangeRates = await _exchangeRateService.GetExchangeRatesAsync();

        if (exchangeRates != null)
        {
            Decimal lempiras = Decimal.Parse(txtLempiras.Text);
            Decimal totalDolares = 0;
            
            var hnlRate = exchangeRates.Data["HNL"];
            totalDolares = lempiras / hnlRate.Value;
            txtDolares.Text = totalDolares.ToString("F4"); // Formatea a 4 decimales
            lblDolares.Text = "Total en Dolares";
            
                      
        }
        else
        {
            txtDolares.Text = "Error";
            Console.WriteLine("error al recibir los tipos de cambio");
        }
    }

    private async void DollarsToLempiras_Clicked(object sender, EventArgs e)
    {
        var exchangeRates = await _exchangeRateService.GetExchangeRatesAsync();

        if (exchangeRates != null)
        {
            Decimal dolares = Decimal.Parse(txtLempiras.Text);
            Decimal totalLempiras = 0;

            var hnlRate = exchangeRates.Data["HNL"];
            totalLempiras = dolares * hnlRate.Value;
            txtDolares.Text = totalLempiras.ToString("F4"); // Formatea a 4 decimales
            lblDolares.Text = "Total en Lempiras";


        }
        else
        {
            txtDolares.Text = "Error";
            Console.WriteLine("error al recibir los tipos de cambio");
        }
    }

}