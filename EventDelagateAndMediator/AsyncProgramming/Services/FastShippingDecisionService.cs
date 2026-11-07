using EventDelagateAndMediator.AsyncProgramming.Models;
using System.Diagnostics;

namespace EventDelagateAndMediator.AsyncProgramming.Services
{
  // elimizde bir sürü servis var bunlar hepsi asenkron bu servislerden hangisi sağlıklı çalışıyor ve ne kadar sürüyor sonuçları almamız gibi bir senary geldi. Amacımız tüm Providerlar içersinde dönüp en uygun optimize olan asenkron servisi bulmak
  public class FastShippingDecisionService
  {
   public readonly (string Provider, decimal Fee, int DelayMs)[] Providers =
   {
      ("Express Kargo",50m,1500),
      ("Yurtiçi Kargo",75m,700),
      ("Premium Kargo",100,200)
    };

    public  async Task<IReadOnlyList<WhenEachResult>> RunAsync(CancellationToken ct = default)
    {
      Stopwatch sp = Stopwatch.StartNew();

      var tasks = new List<Task<ShippingQuote>>();
      var names = new Dictionary<Task<ShippingQuote>,string>(); // local değişken olarak kullanacağız.


      foreach (var item in Providers)
      {
        var task = ShippingService.FetchQuoteAsync(item.Provider, item.Fee, item.DelayMs, item.DelayMs > 1000, ct);
        tasks.Add(task);
        names[task] = item.Provider;
      }

      var results = new List<WhenEachResult>(); // taskların herbirinde gezip elimdeli listeyi doldurmak istiyorum

      // .Net 9.0 sonrası gelen yeni bir özellik. Task.WhenEach() her bir asenkron task içinde gezinmemizi sağlar
      await foreach (var task in Task.WhenEach(tasks)) {
     
        string provider =  names[task];
        long elapsedTimeMs = sp.ElapsedMilliseconds; // kaçıncı msdeyiz.

        try
        {
          var quote = await task; // çözümle
          results.Add(new WhenEachResult(provider, quote.Fee, elapsedTimeMs, "Success",""));
        }
        catch (Exception ex)
        {
          results.Add(new WhenEachResult(provider, null, elapsedTimeMs, "Fail", ex.Message));
        } 
       
      }

      sp.Stop();
      return results;

    }
  }

}
