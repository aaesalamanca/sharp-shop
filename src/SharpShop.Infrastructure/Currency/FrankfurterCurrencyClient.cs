using System.Text.Json;
using SharpShop.Application.Services;

namespace SharpShop.Infrastructure.Currency;

public class FrankfurterCurrencyClient(HttpClient httpClient) : ICurrencyService
{
    private readonly HttpClient _httpClient = httpClient;
    private static readonly Dictionary<string, (decimal Rate, DateTime CachedAt)> _cache = [];
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

    public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
    {
        if (fromCurrency == toCurrency)
            return 1m;

        var cacheKey = $"{fromCurrency}_{toCurrency}";

        if (
            _cache.TryGetValue(cacheKey, out var cached)
            && DateTime.UtcNow - cached.CachedAt < CacheDuration
        )
            return cached.Rate;

        var response = await _httpClient.GetAsync(
            $"https://api.frankfurter.app/latest?from={fromCurrency}&to={toCurrency}"
        );
        _ = response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var rate = doc.RootElement.GetProperty("rates").GetProperty(toCurrency).GetDecimal();

        _cache[cacheKey] = (rate, DateTime.UtcNow);

        return rate;
    }

    public async Task<decimal> ConvertAsync(decimal amountInEuros, string toCurrency)
    {
        if (toCurrency == "EUR")
            return amountInEuros;

        var rate = await GetExchangeRateAsync("EUR", toCurrency);
        return Math.Round(amountInEuros * rate, 2);
    }
}
