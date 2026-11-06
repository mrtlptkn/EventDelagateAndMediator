namespace EventDelagateAndMediator.EventDelegates.Order.Settings
{
  public class DiscountSettings
  {
    // fiyat indirimi yapmadık
    public static decimal NoDiscount(decimal listPrice) => listPrice;
    public static decimal TenPercentDiscount(decimal listPrice) => listPrice * 0.9m; // 10% indirim uyguladım.
    public static decimal HalfPriceDiscount(decimal listPrice) => listPrice * 0.5m; // 50% indirim uyguladım.
  }
}
