using EventDelagateAndMediator.EventDelegates;
using EventDelagateAndMediator.EventDelegates.Email;
using EventDelagateAndMediator.EventDelegates.Order;
using EventDelagateAndMediator.EventDelegates.Order.Rules;
using EventDelagateAndMediator.EventDelegates.Order.Services;
using EventDelagateAndMediator.EventDelegates.Order.Settings;
using System.Reflection;

namespace EventDelagateAndMediator.Endpoints
{
  public static class EventDelegateEndpoints
  {
    // Bu örnek için delegate bir method içerisine parametre olarak gönderildi. Bu sayede delegate methodu içerisinde farklı işlemler yapılabilir. Örneğin toplama, çıkarma, çarpma ve bölme işlemleri yapılabilir.
    public static double Calculate(double a, double b, MathOps ops) => 3 * ops(a, b);

    // Minimal Api

    public static class MathOperation
    {
      public static double Add(double a, double b) => a + b;
      public static double Substract(double a, double b) => a - b;
      public static double Multiply(double a, double b) => a * b;
      public static double Divide(double a, double b) => a / b;
    }

    public static IEndpointRouteBuilder MapEventDelegateEndpoints(this IEndpointRouteBuilder builder)
    {

      // Route gruplarımı /api/eventdelegates ana route altında topladım. Bu sayede tüm event delegate endpointlerini tek bir route altında toplayabilirim.
      var group = builder.MapGroup("/api/eventdelegates").WithTags("Event Delegate");


      group.MapGet("/demo", () =>
      {
        // Bir delegate üzerinden birdeb fazka aynı imzaya sahip methodu çalıştırdık.

        // 1.Örnek de Delegates ile çalışma yaptık

        MathOps add = (a, b) => a + b; // Lambda Expression ile toplama işlemi yaptım.
        var result = add(3, 5);

        MathOps substract = (a, b) => a - b; // Lambda Expression ile çıkarma işlemi yaptım.
        var result2 = substract(5, 7);

        var result3 = Calculate(10, 5, add); // Lambda Expression ile çarpma işlemi yaptım. // 3*15 45
        var result4 = Calculate(5, 7, substract); // Lambda Expression ile bölme işlemi yaptım. // 3*(-2)= -6

        // Method referansı
        MathOps divide = MathOperation.Divide; // Delegate üzerinden Divide methodunu çağırdım.
        Console.WriteLine($"Calculate(10, 2, divide): {Calculate(10, 2, divide)}"); // 5


        // 2.Örnek Multicast Delegate Örneği : Bir delegate ile birden fazla methodu çağırabiliriz. Bu sayede birden fazla methodu tek bir delegate üzerinden çağırabiliriz. Örneğin aşağıdaki örnekte bir delegate ile 3 farklı methodu çağırdık. Bu sayede 3 farklı methodu tek bir delegate üzerinden çağırabiliriz.
        
        // Actionlar void tanımlanmış hazır delegate ifadeleridir. 

        Action<string> log = (message) => Console.WriteLine($"Message: {message}"); // Lambda Expression ile mesaj yazdırdım.
        log += (message) => Console.WriteLine($"Message Length: {message.Length}"); // Lambda Expression ile mesaj uzunluğunu yazdırdım.

        log += (message) =>  {
          
          Console.WriteLine($"Message Upper: {message.ToUpper()}"); // Lambda Expression ile mesajı büyük harfe çevirip yazdırdım.

        };

        log("Merhaba!"); // Delegate üzerinden log methodunu çağırdım. Bu sayede 3 farklı methodu tek bir delegate üzerinden çağırdım.

        // 3.Örnek Action, Function ve Predicate ile Delegate tanımı

        Action<string> print = text => Console.WriteLine(text);
        print("Action Delegate");

        // Function delegate ise yine framework tarafında tanımlı hazır değer döndüren bir delegate. Actiondan farkı değer döndürebilmesi.
        Func<double, double, double> sum = MathOperation.Add;
        Console.WriteLine(sum(2, 3));

        // Kriterli değer döndüren özel tanımlı delegate örneği Predicate
        Predicate<int> isEven = n => n % 2 == 0; // delagate bool Predicate<in T>(T obj);
        Console.WriteLine($"isEven(4): {isEven(4)}");
        Console.WriteLine($"isEven(5): {isEven(5)}");


        // Kendi Classımız üzerinden bir event çalıştırma örneği yapalım. Delegate ile event tanımlayabiliriz. Bu sayede bir event tetiklendiğinde birden fazla methodu çağırabiliriz. Bu sayede event-driven bir yapı oluşturabiliriz.

        Downloader downloader = new (); // Downloader classını örnekledim. Bu class içerisinde bir event tanımladım. Bu event tetiklendiğinde birden fazla methodu çağırabiliriz. Bu sayede event-driven bir yapı oluşturabiliriz.
        // nesnenin instance aldığımız gibi subscription adımlarını açarız.

        downloader.DownloadCompleted += Downloader_DownloadCompleted;
        downloader.DownloadCompleted +=(message) => Console.WriteLine($"Download completed 01: {message}"); // Lambda Expression ile mesaj yazdırdım.


        //downloader.DownloadCompleted = (message) => Console.WriteLine($"Download completed 03: {message}"); // Lambda Expression ile mesaj yazdırdım. Bu sayede event-driven bir yapı oluşturabiliriz.

        downloader.Download(); // Download methodunu çağırdım. Bu method içerisinde event tetikleniyor. Bu sayede event-driven bir yapı oluşturabiliriz.

        downloader.DownloadCompleted -= Downloader_DownloadCompleted; // Event subscription'ı kaldırdım. Bu sayede event-driven bir yapı oluşturabiliriz.

        downloader.Download(); // Download methodunu çağırdım. Bu method içerisinde event tetikleniyor. Bu sayede event-driven bir yapı oluşturabiliriz.


        // 4.Örnek .Net EventHandler delagete ve EventArgs ile event yönetimi

        Shop s = new Shop();
        // subscription
        s.OrderSubmitted += (object? sender, OrderSubmittedEventArgs args) =>
        {
          Console.WriteLine($"Order submitted: {args.OrderId}, {args.CustomerName}"); // OrderSubmitted eventini tetikledim. Bu event tetiklendiğinde birden fazla methodu çağırabiliriz. Bu sayede event-driven bir yapı oluşturabiliriz.
        };

        s.OnAdded += MathOperation.Add;

        s.SubmitOrder("John Doe"); // SubmitOrder methodunu çağırdım. Bu method içerisinde event tetikleniyor. Bu sayede event-driven bir yapı oluşturabiliriz.

        s.Add(3, 5); // Add methodunu çağırdım. Bu method içerisinde event tetikleniyor. Bu sayede event-driven bir yapı oluşturabiliriz.


        // 5.Örnek Reflection ile Delegate Kullanımı

        // 1. Yöntem MethodInfo sınıfı üzerinden ilgili delegate çalıştırma
        // method.Invoke ile çalıştırma
        // Bu tarz programlama yapılarında method isimleri ve methodlara gönderilecek parametreler request nesnesi üzerinden alınabilir.
        MethodInfo method = typeof(MathOperation).GetMethod("Add1");

        object? result5 = method?.Invoke(null, new object[] { 3.0, 5.0 }); // Add methodunu çalıştırdım. Bu sayede event-driven bir yapı oluşturabiliriz.
        Console.WriteLine($"Reflection ile Add methodu çalıştırıldı: {result5}"); // Reflection ile Add methodu çalıştırıldı: 8

        // 2. Yöntem -> Delegate.CreateDelegate(typeof(MathOp), method); // daha tip güvenli ve hızlı bir yöntem
        // hızlı ve tip günvenli bir teknik.

        if(method is not null)
        {
          var mtd = (MathOps)Delegate.CreateDelegate(typeof(MathOperation), method);
          double d = mtd(3, 6);
          Console.WriteLine("d result:" + d);
        }

 
        return Results.Ok(new { Add = result, Substract = result2, Multiply = result3, Divide = result4 });

      })
      .WithName("Event Delegate Demo");


      group.MapPost("/placeOrder", () =>
      {

        var orderRequest = new Order(Guid.NewGuid().ToString(), 1500, 10, "Ürün A", "Ali");

        Action<string> log = (message) => Console.WriteLine("[Log]" + message);

        DiscountStrategyDelegate strategy = DiscountSettings.HalfPriceDiscount;

        var rules = new OrderRules();
        rules.AddRule("En fazla 100 adet aynı anda sipariş edilebilir", x => x.Quantity < 100);
        rules.AddRule("Customer Name boş olmaz", x => !String.IsNullOrEmpty(x.CustomerName));

        var orderService = new OrderService(log, rules, strategy);
        // event listener
        orderService.OrderRejected += (message) => Console.WriteLine($"İşlem İptal Edildi: {message}");

        orderService.SubmitOrder(orderRequest);

        //****************************************************************//

        DiscountStrategyDelegate strategy2 = DiscountSettings.NoDiscount;

        var rules2 = new OrderRules();
        rules2.AddRule("List Price 0 olamaz", x => x.ListPrice != 0);
        rules2.AddRule("OrderID ve CustomerName Empty veya Null geçilemez", x => !String.IsNullOrEmpty(x.orderUUID) && !String.IsNullOrEmpty(x.CustomerName));

        // Subscriber
        EmailService e = new EmailService();

        var orderService2 = new OrderService(log, rules2, strategy2);
        orderService2.OrderRejected += (message) => Console.WriteLine($"İşlem İptal Edildi: {message}");
        orderService2.OrderPlaced += e.OnOrderPlaced; // email servis methodunu tetikle.

        orderService2.SubmitOrder(orderRequest);

      });


      return builder;

    }

   

    private static void Downloader_DownloadCompleted(string obj)
    {
      Console.WriteLine($"Download completed 02: {obj}"); // DownloadCompleted eventini tetikledim. Bu event tetiklendiğinde birden fazla methodu çağırabiliriz. Bu sayede event-driven bir yapı oluşturabiliriz.
    }
  }
}
 