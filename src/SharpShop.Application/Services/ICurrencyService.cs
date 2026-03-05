namespace SharpShop.Application.Services;

public interface ICurrencyService
{
    Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency);
    Task<decimal> ConvertAsync(decimal amountInEuros, string toCurrency);
}
