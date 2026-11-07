using EventDelagateAndMediator.AsyncProgramming.Models;

namespace EventDelagateAndMediator.AsyncProgramming.Services
{
  // Amaç: Birden fazla kargo servismiz var en hızlı kargo servisini sipariş sürecinde kullanmak istiyoruz.
  // Task.WhenAny -> en hızlı olan task çözümler resolve eder.
  public class ShippingService
  {

    // Providers için Neden Tupple kullanıdık ?
    // Anlık ihtiyacımız varsa ve başka bir sınıfta bu provider nesnesini kullanamayacak tupple dan yararlanabiliriz.
    public readonly (string Provider, decimal Fee, int DelayMs)[] Providers =
    {
      ("Express Kargo",50m,1500),
      ("Yurtiçi Kargo",75m,700),
      ("Premium Kargo",100,200)
    };
    public async Task<ShippingQuote> GetFastestQuoteAsync(CancellationToken ct = default)
    {
      using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
      cts.CancelAfter(TimeSpan.FromMilliseconds(300));

      var quoteTasks = Providers
          .Select(x => FetchQuoteAsync(x.Provider, x.Fee, x.DelayMs, x.DelayMs > 500, cts.Token))
          .ToList();

      try
      {
        Task<ShippingQuote> firstCompletedTask = await Task.WhenAny(quoteTasks);
        return await firstCompletedTask;
      }
      catch (OperationCanceledException) when (!ct.IsCancellationRequested) // CancelAfter iptali
      {
        // Dış iptal değil, 300 ms süre aşımı
        throw new TimeoutException("Kargo teklifleri 300 ms içinde alınamadı.");
      }

    }

    // Remote Quote Servis
    public static async Task<ShippingQuote> FetchQuoteAsync(
      string provider,
      decimal fee, 
      int delayMs, 
      bool fails,
      CancellationToken ct= default)
    {
      await Task.Delay(delayMs, ct); // Ağ gecikmesi simülasyonu
      if (fails)
        throw new InvalidOperationException($"{provider} servisi şuan hizmet dışı");
      return new ShippingQuote(provider, fee, delayMs);

    }




  }
}
