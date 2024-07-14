namespace StarBankApp.Controllers;
using System.Collections.Generic;

public class Meta
{
    public string LastUpdatedAt { get; set; }
}

public class CurrencyData
{
    public string Code { get; set; }
    public decimal Value { get; set; }
}

public class ExchangeRateResponse
{
    public Meta Meta { get; set; }
    public Dictionary<string, CurrencyData> Data { get; set; }
}