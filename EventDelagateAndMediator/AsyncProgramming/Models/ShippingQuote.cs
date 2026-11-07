namespace EventDelagateAndMediator.AsyncProgramming.Models
{
  // Provider: Kargo Sağlayıcısı
  // Fee: Kargo ücreti
  // DeliveryDate: Teslimat tarihi
  public record ShippingQuote(string Provider, decimal Fee, int DeliveryDate);
  
}
