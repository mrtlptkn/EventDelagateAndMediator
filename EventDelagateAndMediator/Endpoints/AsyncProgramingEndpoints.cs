using EventDelagateAndMediator.AsyncProgramming.Models;
using EventDelagateAndMediator.AsyncProgramming.Services;
using System.Diagnostics;
using System.Security.Principal;

namespace EventDelagateAndMediator.Endpoints
{

  // Senkron frakımız Non-blocking I/O işlemleri ile asenkron programlama yapabiliriz. Bu sayede uygulamamızın performansını arttırabiliriz. Örneğin bir web uygulamasında bir kullanıcı bir butona tıkladığında, bu butonun click eventinde bir I/O işlemi yapılacaksa, bu I/O işlemi tamamlanana kadar kullanıcı diğer işlemleri yapabilir. Bu sayede kullanıcı deneyimi artar ve uygulamanın performansı artar.

  // Desktop programlada eğer Asenkron programlama yapmazsak, uygulama bloke olur ve kullanıcı diğer işlemleri yapamaz. Örneğin bir butona tıkladığında, bu butonun click eventinde bir I/O işlemi yapılacaksa, bu I/O işlemi tamamlanana kadar kullanıcı diğer işlemleri yapamaz. Bu sayede kullanıcı deneyimi azalır ve uygulamanın performansı düşer. Asenkron programlama kullanmamak UI threadini bloke eder. 

  // Web programlamlamada UI Thread yoktur. Bunun yerine Main Thread vardır. Main Thread, web uygulamasında gelen istekleri karşılar ve bu istekleri işlemek için thread havuzundan thread alır. Eğer bir istek uzun süren bir I/O işlemi yapıyorsa, bu istek tamamlanana kadar thread havuzundaki thread bloke olur ve diğer istekler beklemek zorunda kalır. Bu sayede web uygulamasının performansı düşer. Asenkron programlama kullanmak thread havuzunu bloke etmez. Bu sayede web uygulamasının performansı artar.

  // Thread sınıfını direkt olarak kullanmayız OS düzeyinde çalışır. Task sınıfı ise .NET Framework tarafından sağlanır ve thread havuzunu kullanır. Task sınıfı ile asenkron programlama yapabiliriz. Task sınıfı ile asenkron programlama yaparken, thread havuzunu bloke etmez. Bu sayede web uygulamasının performansı artar. 

  // Thread ile Task arasındaki farklar:
  // 1. Thread sınıfı OS düzeyinde çalışır. Task sınıfı ise .NET Framework tarafından sağlanır ve thread havuzunu kullanır.
  // 2. Thread sınıfı ile asenkron programlama yaparken, thread havuzunu bloke eder. Task sınıfı ile asenkron programlama yaparken, thread havuzunu bloke etmez.

  // Sunucuya yapılan bir isteğin cevabını başka bir web isteği beklemiyor yani bloke olmuyor.

  // Task: Asenkron bir işi yapmamızı sağlayan bir sınıftır. Task sınıfı ile asenkron programlama yapabiliriz. Task sınıfı ile asenkron programlama yaparken, thread havuzunu bloke etmez. Bu sayede web uygulamasının performansı artar.
  // ValueTask: ValueTask: ValueTask sınıfı, Task sınıfına göre daha az bellek kullanır ve daha hızlıdır. ValueTask sınıfı, Task sınıfına göre daha az bellek kullanır ve daha hızlıdır.Eğer bir asenkron method hem senkron hemde asenkron sonuç döndürebiliyorsa, ValueTask sınıfını kullanabiliriz. ValueTask sınıfı içerisinde tek bir sonuç döndürebilir. Birden fazla awaitable işlemi birleştirmek için Task sınıfını kullanabiliriz. 
  // await : await anahtar kelimesi, bir asenkron methodun sonucunu beklemek için kullanılır. await anahtar kelimesi, bir asenkron methodun sonucunu beklerken, thread havuzunu bloke etmez. Bu sayede web uygulamasının performansı artar. 
  // Task.WhenAny(): Task.WhenAny() metodu, birden fazla asenkron işlemi başlatır ve herhangi birinin tamamlanmasını bekler. Bu sayede, en hızlı tamamlanan işlemin sonucunu alabiliriz. Örneğin, birden fazla web isteği yapıyorsak, en hızlı tamamlanan isteğin sonucunu alabiliriz. Task.WhenAny() metodu, thread havuzunu bloke etmez. Bu sayede web uygulamasının performansı artar. Eğer tasklardan biri hata verirse, Task.WhenAny() metodu AggregateException fırlatır. AggregateException içerisinde tüm hataları görebiliriz.
  // Task.WhenAll(): Task.WhenAll() metodu, birden fazla asenkron işlemi başlatır ve hepsinin tamamlanmasını bekler. Bu sayede, tüm işlemlerin sonucunu alabiliriz. Task.WhenAll() metodu, thread havuzunu bloke etmez. Bu sayede web uygulamasının performansı artar. Eğer tasklardan biri hata verirse, Task.WhenAll() metodu AggregateException fırlatır. AggregateException içerisinde tüm hataları görebiliriz.
  // Task.WaitAll(): Task.WaitAll() metodu, birden fazla asenkron işlemi başlatır ve hepsinin tamamlanmasını bekler. Bu sayede, tüm işlemlerin sonucunu alabiliriz. Task.WaitAll() metodu, thread havuzunu bloke eder. Bu sayede web uygulamasının performansı düşer. Eğer tasklardan biri hata verirse, Task.WaitAll() metodu AggregateException fırlatır. AggregateException içerisinde tüm hataları görebiliriz.
  // Task.Run(): Task.Run() metodu, bir asenkron işlemi başlatır ve thread havuzundan bir thread alır. Bu sayede, asenkron işlemi başlatabiliriz. Task.Run() metodu, thread havuzunu bloke etmez. Bu sayede web uygulamasının performansı artar. Senkron kod bloğunu Task.Run içerisine alarak asenkron olarak çalıştırabiliriz. 
  // Task.Delay(): Ağ Gecikmesi gibi durumları simüle etmek için kullanılır
  // Task.FromResult(): Task Başarılı olduğunda dönen cevap
  // Task.FromException(): Task Başarısız olduğunda dönen cevap, Hata durumunda döner
  // Task.FromCanceled(): Taskların iptal durumlarında dönen cevap, CancellationToken ile iptal edilen tasklar için döner, Yada timeout belirleyerek iptal döneriz.
  // Task.WhenEach()  Task.WhenEach() metodu, birden fazla asenkron işlemi başlatır ve her birinin tamamlanmasını bekler. Bu sayede, her bir işlemin sonucunu alabiliriz. Task.WhenEach() metodu, thread havuzunu bloke etmez. Bu sayede web uygulamasının performansı artar. Eğer tasklardan biri hata verirse, Task.WhenEach() metodu AggregateException fırlatır. AggregateException içerisinde tüm hataları görebiliriz.
  // CancellationToken: İptal durumlarından bu nesneden yararlanırız. CancellationToken, bir asenkron işlemi iptal etmek için kullanılır. CancellationToken, thread havuzunu bloke etmez. 

  public static class AsyncProgramingEndpoints
  {

    public static IEndpointRouteBuilder MapAsyncProgramingEndpoints(this IEndpointRouteBuilder builder)
    {

      // Route gruplarımı /api/eventdelegates ana route altında topladım. Bu sayede tüm event delegate endpointlerini tek bir route altında toplayabilirim.
      var group = builder.MapGroup("/api/async-programming").WithTags("Async Programming");

      group.MapPost("demo", async (
        PricingService pricingService, 
        ShippingService shippingService, 
        NotificationService notificationService,
        CancellationToken ct) =>
      {
        // await Task.Delay(1000);

        try
        {
          decimal price = await pricingService.GetPriceAsync("Ürün-A", ct);
          Console.WriteLine($"price: {price}");

          ShippingQuote shippingQuote = await shippingService.GetFastestQuoteAsync(ct);
          Console.WriteLine($"shippingQuote:{shippingQuote.Provider}");


          // Uygulama 1: Performansız bir kod yazmış olduk. toplam 600ms+600ms+300ms sürdü

          //Stopwatch sp = Stopwatch.StartNew();

          //await notificationService.SendSmsAsync("874545454", ct);
          //await notificationService.SendEmailAsync("ali", ct);
          //await notificationService.SendAccountingAsync("sadsad", ct);

          //sp.Stop();

          //Console.WriteLine("Elapsed" + sp.ElapsedMilliseconds);


          // Uygulama 2:Ama bu şekilde yazarsak toplam en uzun süren kaydar sürüp sonuçlandı.

          var results = await notificationService.NotifyAllAsync("ali@test.com", "05444441414", "78454512", ct);



          return Results.Ok(new { price, shippingQuote });
        }
        catch(OperationCanceledException c)
        {
          Console.WriteLine(c.Message);
          return Results.BadRequest();
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return Results.BadRequest();
        }


      });

      return builder;

    }

  }
}
