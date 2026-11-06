namespace EventDelagateAndMediator.EventDelegates
{
  // İndirim stratejisi için bir delegate açalım // DiscountStrategyDelegate

  // İndirim oranlarını hesaplamak için bir kaç method olucak, DiscountSettings
  // 0.50, NoDiscount, 0.10PercentDiscount

  // Sipariş Nesnem olsun // record tipinde tanımlıyalım

  // OrderRules -> Burada ise sipariş ile ilgili validasyon kuralları -> quantity 10 dan fazla olamaz
  // tek siparişte 100000 fazla sipariş tutarı olamaz 

  // OrderService class içinde sipariş sürecini ilgili discountSettings ve DiscountStratgy göre yönetmek istiyorum


  // Amaç : Event Delegate yapısı kullanarak, sipariş işlemini dışarıdan verilen değerler ile nasıl akışı bozmadan yönetebiliriz.

  // her farklı istekte orderservice içinsindeki ana yapıyı akışı bozmamak 

  public delegate decimal DiscountStrategyDelegate(decimal listPrice); // listPrice üzerinden indirim oranını hesaplayacak delegate tanımı yaptım. Bu sayede farklı indirim stratejilerini tek bir delegate üzerinden çağırabiliriz. Bu sayede event-driven bir yapı oluşturabiliriz.

  public class DiscountSettings
  {
    // fiyat indirimi yapmadık
    public static decimal NoDiscount(decimal listPrice) => listPrice;
    public static decimal TenPercentDiscount(decimal listPrice) => listPrice * 0.9m; // 10% indirim uyguladım.
    public static decimal HalfPriceDiscount(decimal listPrice) => listPrice * 0.5m; // 50% indirim uyguladım.
  }

  // Sipariş Nesnesi -> OrderService Bu nesne üzerinden çalışacak
  public record Order(String orderUUID, decimal ListPrice, int Quantity, string ProductName,string CustomerName);

  // Sipariş ile ilgili validasyon kuralları -> quantity 10 dan fazla olamaz, tek siparişte 100000 fazla sipariş tutarı olamaz
  public class OrderRules
  {

    // Burası sipariş işlemi yaparken dinamik kurallara göre bir validayon uygulaması sağlar.
    public List<(string Name, Func<Order, bool> Check)> Rules { get; } = new();

    public void AddRule(string name, Func<Order, bool> check)
    {
      Rules.Add((name, check));
    }
  }

  public class OrderService
  {
    // fields
    private readonly Action<string> _log;
    private readonly OrderRules _orderRules;
    private readonly DiscountStrategyDelegate _discountStrategy;

    // Reject edilen değer ile ilgili neden reject olduğunu döndürdüğümüz bir event
    public event Action<string> OrderRejected;

    public OrderService(Action<string> log, OrderRules orderRules, DiscountStrategyDelegate discountStrategy)
    {
      _log = log;
      _orderRules = orderRules;
      _discountStrategy = discountStrategy;
    }
    public void SubmitOrder(Order order)
    {
      _log($"Processing Order for {order.orderUUID}");


      foreach (var rule in _orderRules.Rules)
      {
        if (!rule.Check(order)) // kurala uyulmadığında ne yapılacak
        {
          // OrderRejected.Invoke();
          OrderRejected?.Invoke(rule.Name);
          return;
        }
      }

      var discountedTotal = _discountStrategy(order.Quantity * order.ListPrice);
      _log($"Order {order.orderUUID} total after discount tutar {discountedTotal}");
      // OrderPlaced.Invoke();

    }
   
  
  }
}
