using System.Diagnostics;

namespace EventDelagateAndMediator.AsyncProgramming.Services
{
  // Sms, Email, Accounting Notification
  // Sms 600ms, Email 300ms, Accounting 600ms 

  // await SendSmsAsync()
  // await SendEmailAsync()
  // await SendAccountingAsync(); // Toplam 1500ms
  // Task.WhenAll([SendSmsAsync(),SendEmailAsync(),SendAccountingAsync()]) ~600ms
  // sıralı birbirleri ile sonuç paylaşamayan asenkron kod bloklarınız varsa birbilerini beklemeye gerek yok performasız bir çağır olur. Hepsini aynı anda işlersek daha hızlı bir sonuç döndürürüz.
  public class NotificationService
  {
    public async Task SendEmailAsync(string to, CancellationToken ct= default)
    {
      //throw new Exception("Hata SMS");
      await Task.Delay(300,ct);
    }

    public async Task SendSmsAsync(string phone,CancellationToken ct= default)
    {
      await Task.Delay(600, ct);
    }

    public async Task SendAccountingAsync(string account, CancellationToken ct = default) {

      await Task.Delay(600, ct);
    }


    public async Task<IReadOnlyList<string>> NotifyAllAsync(string email,string phone,string account, CancellationToken ct)
    {
      Stopwatch sp = Stopwatch.StartNew();

      var tasks = new[]
      {
        SendEmailAsync(email,ct),
        SendSmsAsync(phone,ct),
        SendAccountingAsync(account,ct)
      };

      try
      {
        // Task.WhenAll().Wait(); // Kod burada yanlışlıkla kullanımdan kaynalık senkron oldu. Blocked

        await Task.WhenAll(tasks);
        Console.WriteLine($"ElapsedTime" + sp.ElapsedMilliseconds); // toplamda ne kadar sürdüğünü
        sp.Stop();
        return Array.Empty<string>();
      }
      catch (Exception) when (!ct.IsCancellationRequested)
      {
        sp.Stop();
        return tasks.Where(x => x.IsFaulted).SelectMany(x => x.Exception!.InnerExceptions).Select(x => x.Message).ToList();
      }

    }
  }
}
