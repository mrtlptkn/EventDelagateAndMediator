namespace EventDelagateAndMediator.EventDelegates.Order
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

  // Sipariş ile ilgili validasyon kuralları -> quantity 10 dan fazla olamaz, tek siparişte 100000 fazla sipariş tutarı olamaz
  public record OrderPlacedEventArgs(string OrderId, decimal total, string customer);

 

 
}
