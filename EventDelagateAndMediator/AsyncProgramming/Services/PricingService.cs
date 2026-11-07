using EventDelagateAndMediator.AsyncProgramming.Models;
using System.Collections.Concurrent;

namespace EventDelagateAndMediator.AsyncProgramming.Services
{


  // Bu servis ilgili ürünlerin fiyat bilgisi alabileceğimiz bir mock servis
  // Ürün-A 1, 150
  // Ürün-B,2, 200
  public class PricingService
  {
    private readonly ConcurrentDictionary<string,decimal> _cache = new(); // In Memory Database
    public ValueTask<decimal> GetPriceAsync(string ProductId,CancellationToken cts = default)
    {
      if(_cache.TryGetValue(ProductId, out var cachedPrice))
        return ValueTask.FromResult(cachedPrice); // Hızlı çalışacak cacheden çalışır

      return new ValueTask<decimal>(FetchPriceFromRemoteAsync(ProductId,cts)); // yoksa remote call ile ve cache at.
    }

    // Task sadece DB, HttpClient Request, Async I/O gibi işlemlerde kullanırız. içindeki kod full asenkron yazılmalıdır.
    private async Task<decimal> FetchPriceFromRemoteAsync(string ProductID, CancellationToken cts = default)
    {
      await Task.Delay(500); // Ağ geçikmesi yapalım

      decimal price = ProductID 
        switch
      {
        "Ürün-A" => 150.5m,
        "Ürün-B" => 250m,
        "Ürün-C" => 500.2m,
        "Ürün-D" => 550.5m,
        _ => throw new KeyNotFoundException("Ürün bulunamadı")
      };

      _cache[ProductID] = price;

      return price;

    }



  }
}
