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
      cts.CancelAfter(100); // 100 ms beklemeden sonra otomatik olarak iptal tetiklensin çalışmıyor bir kontrol edelim.


      await Task.Delay(1000); // 1000 ms içerisinde  istek network üzerinden iptal edilirse aşağıda kod ile sınıf için istek iptal exception fırlatıyoruz.

      // Eğer istek uzun sürerde iptal gerçekleşirse iptal exception'ı fırlat.
      // Kullanıcı bir iptal talebinde bulunursa o zaman iptal exception fırlat
      if (ct.IsCancellationRequested)
      {
        ct.ThrowIfCancellationRequested();
      }

      var qouteTasks = Providers.Select(x => FetchQuoteAsync(x.Provider, x.Fee, x.DelayMs, x.DelayMs > 500, ct)).ToList();

        // elimdeki tüm kargo sağlayıcılarında sadece en hızlı olan çözümle
       Task<ShippingQuote> firstCompletedTask = await Task.WhenAny(qouteTasks);

       return await firstCompletedTask;
      
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
