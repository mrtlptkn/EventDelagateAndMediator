namespace EventDelagateAndMediator.AsyncProgramming.Models
{

  // NotificationService -> Bildirim servici
  // ShippingService -> Sipariş ile ilgili kargo süreçlerini yöneten servis
  // PricingService -> ValueTask örneği için ürünün fiyat bilgisini dönen servis

  // Models -> Order, Product, ShippingQuote (Kargo Teklifi)
  public record Order(string ProductId, int Quantity,string CustomerEmail);
  public record OrderResponse(string OrderId, string ProductId, int Quantity, decimal UnitPrice, decimal Total, ShippingQuote ShippingQuote, IReadOnlyList<string> Errors);
 
}
