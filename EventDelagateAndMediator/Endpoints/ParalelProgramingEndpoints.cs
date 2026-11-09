using System.Collections.Concurrent;
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


      group.MapPost("/demo4", async () =>
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

      group.MapPost("/demo5", () =>
      {
        // shared Data
        int counter = 0;


        Parallel.For(0, 100000, (i) =>
        {
          Thread.SpinWait(1000);
          counter++; // her dönüş işleminde shared data güncellemesi var 
          // Acaba threadler bu veriyi bozucak mı ?


        });

        return Results.Ok(new { counter }); // 100000

      });


      // Birinci yöntem
      group.MapPost("/demo6", () =>
      {
        // shared Data
        int counter = 0;

        object SyncObject = new object(); // boş bir kilit objesi tanımladık


        Parallel.For(0, 100000, (i) =>
        {
          Thread.SpinWait(1000);

          lock(SyncObject) // thread-safe çalışabilmek için bir kilit uyguladık.
            counter++; // her dönüş işleminde shared data güncellemesi var 
          // Acaba threadler bu veriyi bozucak mı ?


        });

        return Results.Ok(new { counter }); // 100000

      });

      // Interlock kendi içinde lockObject kullanmadan thread-safe çalışıp threadler arasında ortak veri paylaşımını yapmamızı sağlayan bir sınıf
      group.MapPost("/demo7", () =>
      {
        // shared Data
        int counter = 0;
        ConcurrentBag<int> data = new(); // Elde ettiğimiz sonuçları listelere doldurma gibi durumlarımız varsa bu durumda normal collection kullanamayız bunun yeni, ConcurentDicrionary ve ConcurentBag gibi thread-safe collection kullanmamız gerekir. 
        // içerisine atılan listedeki verilerin sıra garantisi vermez. Sıra garantisi istersek BlockingCollection gibi Thread-Safe collection kullanmamız lazım fakat concurentBag göre çok daha yavaş çalışır. 

        Parallel.For(0, 100000, (i) =>
        {
          Thread.SpinWait(1000);

          Interlocked.Increment(ref counter);
          // Acaba threadler bu veriyi bozucak mı ?
          data.Add(counter);


        });

        // Nedeni ise threadlerin çalışma sırasını bilmiyoruz.
        // paralel işleme bittikten sonra yapılması gerekir. 
       var orderedList =   data.OrderBy(x=>x);

        return Results.Ok(new { counter, data }); // 100000

      });



      // Eğer normal lock kullanmak istemezse bu durumda aslında bunun tam karşılığı olan bir Monitor sınıfı var 
      // Monitor.Enter(lockObject) // Kilit alma (try) bloğunun öncesinde yazarız 
      // Monitor.Exist(lockObject) // Kilit bırakama (finally) bloğunda yazarız
      group.MapPost("/demo8", () =>
      {
        // Inprocess süreçlerde bu tarz kilitler ilgili processin atomik olmasını sağlar.
        // shared Data
        int counter = 0;

        object SyncObject = new object(); // boş bir kilit objesi tanımladık

        Parallel.For(0, 100000, (i) =>
        {
          
          Monitor.Enter(SyncObject);
          try
          {
            Thread.SpinWait(1000);
            counter++; // Kilit varken Thread arası uygulanan logic
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
          } 
          finally
          {
            Monitor.Exit(SyncObject); // Bir sonraki thread shared Data üzerinde işlem yapabilsin diye kilidi bıraktık
          }
        });

        return Results.Ok(new { counter }); // 100000

      });


      // Not: Eğer bir uygulama kodu farklı uygulamalardan tetikleniyorsa, bu farklı uygulamalardan tetiklenen eş zamanlı concurent durumlarda in process olarak kullanılan yapılar yerine Mutex değimiz out process ilgili sınıflar kullanır.
      // Mutex işletim sistemi düzeyinde direk olarak Thread sınıfları ile çalışır.

      group.MapPost("/demo9", () =>
      {
        int mutexCounter = 0;
        using Mutex m = new Mutex(initiallyOwned: false, name: "LogDosyası"); // Farklı processler burayı ortak olarak kullanacak ise mutex Name alanına bir isim verin. Bu ortak isim üzerinden kilit açar


        Thread[] mutexThreads = new Thread[100];

        for (int i = 0; i < 100; i++)
        {
          mutexThreads[i] = new Thread(() =>
          {

            m.WaitOne(); // Sırası gelene kadar thread beklesin kilit kısmı

            try
            {
              Thread.SpinWait(100); // Algoritma kodu
              mutexCounter++;

              File.AppendAllText("log.txt",$" -> PID {Environment.ProcessId} \n");

            }
            finally
            {
              m.ReleaseMutex();// kilidi geri bırak
            }
            
          });
        }

        foreach (Thread thread in mutexThreads)
          thread.Start();

        foreach (Thread thread in mutexThreads)
          thread.Join(); // Main Thread bağlama -> Eğer Main Thread sonlarınırsa thread ThreadPool devredilir. Yoksa arka planda çalışmaya devam ederler.

        return Results.Ok(mutexCounter);
      });




      return builder;

    }


  }
}
