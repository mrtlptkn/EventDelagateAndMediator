using System.Diagnostics;

namespace EventDelagateAndMediator.Endpoints
{
  public static class ParalelProgramingEndpoints
  {
    public static IEndpointRouteBuilder MapParallelProgramingEndpoints(this IEndpointRouteBuilder builder)
    {

      // Route gruplarımı /api/eventdelegates ana route altında topladım. Bu sayede tüm event delegate endpointlerini tek bir route altında toplayabilirim.
      var group = builder.MapGroup("/api/parallel-programming").WithTags("Parallel Programming");

      group.MapPost("/demo", () =>
      {

        Stopwatch sp = Stopwatch.StartNew();

        // Konu anlatımı

        Parallel.For(0, 10000, (i) =>
        {
          // double result = Math.Round(Math.PI * Math.Sqrt(i) + Math.Pow(2, i)) / Math.Log(i);
          Thread.SpinWait(1000); // her bir dönüşte 1000 tane loop olan bir kod bloğu tetikle -> CPU tüketimi açtık
          // CPU tüketimini simüle ettim.
          // Console.WriteLine($"Paralel: [ThreadId]: {Thread.CurrentThread.ManagedThreadId}"); 
          // Thread Pooldaki kaç farklı thread ile çalışmışız. MultiThreading
        });

        Console.WriteLine($"[Paralel] ms: {sp.ElapsedMilliseconds}");

        sp.Stop();

        Stopwatch sp1 = Stopwatch.StartNew();

        for (int i = 0; i < 10000; i++)
        {
          // double result = Math.Round(Math.PI * Math.Sqrt(i) + Math.Pow(2, i)) / Math.Log(i); 
          Thread.SpinWait(1000);
          // Console.WriteLine($"Sync: [ThreadId]: {Thread.CurrentThread.ManagedThreadId}");
        }

        Console.WriteLine($"[Sync] ms: {sp1.ElapsedMilliseconds}");

        sp1.Stop();

      });


      group.MapPost("/demo2", (CancellationToken ct) =>
      {

        // csv dosyasında milyonlara veriyi okuyup, transform değerini güncellemek calculation hesaplama. (CPU)
        // Örneğin bu listedeki şehirlerin türkiye ve dünya nufusuna göre yaşam koşulları, hava kirlilik oranları, nüfus yoğunluk durumları gibi istatistikler çıkararak hesaplama yapmak istiyorum.
        List<string> cities = ["İstanbul", "İzmir", "Ankara", "Antalya", "Bursa", "Kocaeli", "Konya"];


        using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(ct);


        ParallelOptions options = new ParallelOptions();
        options.CancellationToken = cts.Token;
        options.MaxDegreeOfParallelism = 3; // Bu iş için en fazla 3 thread kullan


        // Senkron Programlamada Thread.CurrentThread.ManagedThreadId değer sadece Main Thread -> Uygulamayı çalıştıran Thread'i verir. Amaç uygulayı çalıştıran instance bloke etmeden farklı threadler üzerinde işlem yaptırarak non-blocking çalışmak veya birden fazla thread ile yükü dengelemek, ölçeklenebilirlik sağlamak.

        try
        {
          Parallel.ForEach(cities, options, (item) =>
          {
            Console.WriteLine($"[Sehir] -> {item} - ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            Thread.SpinWait(1000); // Cpu bounded iş
            throw new ArgumentException("Hata"); // aynı anda 3 thread bu hatayı aldığı için tek bir exception dönmez çoklu exception döner.
          });
        }
        catch (OperationCanceledException op)
        {
          Console.WriteLine(op.Message);
        }
        catch (AggregateException ag)
        {
          List<string> errors = ag.InnerExceptions.Select(x => x.Message).ToList();
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }

      });

      // Paralel.Invoke methodu ile birden fazla paralel işlemi çalıştırabiliriz. Task.WhenAll() benzer

      group.MapPost("/demo3", async () =>
      {

        Stopwatch sw = Stopwatch.StartNew();

        var action1 = new Action(() =>
        {
          Console.WriteLine($"[Paralel 1] -> {Thread.CurrentThread.ManagedThreadId}");
          Thread.SpinWait(10000);

        });

        var action2 = new Action(() =>
        {
          Console.WriteLine($"[Paralel 2] -> {Thread.CurrentThread.ManagedThreadId}");
          Thread.SpinWait(50000);

        });

        var action3 = new Action(() =>
        {
          Console.WriteLine($"[Paralel 3] -> {Thread.CurrentThread.ManagedThreadId}");
          Thread.SpinWait(40000);

        });


        Parallel.Invoke(action1, action2, action3);

        Console.WriteLine($"Toplam Süre {sw.ElapsedMilliseconds}");

        sw.Stop();

      });


      group.Map("/demo4", async () =>
      {

        var action1 = new Action(() =>
        {
          Console.WriteLine($"[Task 1] -> {Thread.CurrentThread.ManagedThreadId}");
          Thread.SpinWait(10000);

        });

        var action2 = new Action(() =>
        {
          Console.WriteLine($"[Task 2] -> {Thread.CurrentThread.ManagedThreadId}");
          Thread.SpinWait(50000);

        });

        var action3 = new Action(() =>
        {
          Console.WriteLine($"[Task 3] -> {Thread.CurrentThread.ManagedThreadId}");
          Thread.SpinWait(40000);

        });


        Stopwatch sp = Stopwatch.StartNew();

        // Not: Task.Run kullanınca Thread Pool yeni bir thread üzerinden işinizi yönetmeye zorluyoruz.
        // Yani aynı thread kullanımı yerine yeni bir thread atıyor
        // Cpu bound bir tüketim varsa eğer kodda asenkron çalışırken thread birbirinden izole olsun diye Task.Run tercih edibilir. 
        var task1 = new Task(action1);
        task1.Start();

        //await task1;

        var task2 = new Task(action2); // 1.Yöntem
        task2.Start();

        //await task2;


        var task3 = new Task(action3);
        task3.Start();

        // yukarıdaki yöntemde task start etmeyi unutursak kod çözümlenmez bu sebeple task.Run ile aşağıdaki gibi yazılması önerilir. best practicesdır. 
        // var task3 = Task.Run(() => action3()); // 2.Yöntem (Yeni bir thread polldaki thread zorlama)
        //task3.Start();

        //await task3;

        await Task.WhenAll(task1, task2, task3);

        sp.Stop();
        Console.WriteLine($"Task Toplam Süre: {sp.ElapsedMilliseconds}");

      });


      // Kilitleri nasıl uygularız, race condition nasıl oluşur ? Bu durumdan kurtulmak için ne tür sınıflar thread-safe var
      




      return builder;

    }


  }
}
