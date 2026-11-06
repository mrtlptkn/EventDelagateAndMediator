namespace EventDelagateAndMediator.EventDelegates
{
  public delegate double MathOps(double a, double b); // Toplama, Çıkarama, Çarpma ve Bölme 2 değerleri tüm işlemlerde kullanılabilir. 


  // Download methdonu çağırınca bir event tetiklemek istiyoruz.
  // Kendi classlarımızda event-driven nasıl çalışırız.
  public class Downloader
  {

    public event Action<string>? DownloadCompleted; // DownloadCompleted eventini tanımladım. Bu event tetiklendiğinde birden fazla methodu çağırabiliriz. Bu sayede event-driven bir yapı oluşturabiliriz.

    public void Download()
    {
      Console.WriteLine("Downloading...");
      // Event fırlatma akışı
      // ? ile event subscribe edilmiş mi kontrol ediyorum. Eğer subscribe edilmişse event tetiklenir. Eğer subscribe edilmemişse event tetiklenmez. Bu sayede event-driven bir yapı oluşturabiliriz.
      DownloadCompleted?.Invoke("Download completed!"); // DownloadCompleted eventini tetikledim. Bu event tetiklendiğinde birden fazla methodu çağırabiliriz. Bu sayede event-driven bir yapı oluşturabiliriz.
    }
  }


  // Eventler ile çalışırken .net framework tarafında tanımlı hazır EventArgs classını kullanabiliriz. Bu sayede event-driven bir yapı oluşturabiliriz.

  // EventArgs classı event-driven bir yapıda event tetiklendiğinde event ile ilgili bilgileri taşımak için kullanılır. Bu sayede event-driven bir yapı oluşturabiliriz.

  // Event üzerinden taşınacak değerlere event Args deriz.

  public class OrderSubmittedEventArgs : EventArgs
  {
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
  }
  // Not: Kendi delegatemizi tanımlayıp onun üzerinden eventte çalışırabiliriz.
  public class Shop
  {
    public event EventHandler<OrderSubmittedEventArgs>? OrderSubmitted; // Desktop uygulama geliştirirken kullanılan tüm eventler EventHandler üzerinden tanımlıdır.
    public event MathOps? OnAdded; // bir değer toplanacağı zaman tanımlı event.

    public void SubmitOrder(string customerName)
    {
      OrderSubmitted?.Invoke(this, new OrderSubmittedEventArgs { OrderId = 1, CustomerName = customerName }); // OrderSubmitted eventini tetikledim. Bu event tetiklendiğinde birden fazla methodu çağırabiliriz. Bu sayede event-driven bir yapı oluşturabiliriz.
    }

    public void Add(double a, double b)
    {
      if (OnAdded != null)
      {
        double result = OnAdded?.Invoke(a, b) ?? 0; // OnAdded eventini tetikledim. Bu event tetiklendiğinde birden fazla methodu çağırabiliriz. Bu sayede event-driven bir yapı oluşturabiliriz.
        Console.WriteLine($"Result: {result}");
      }
    }

  }


 
}
