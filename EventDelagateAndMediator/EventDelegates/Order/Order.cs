namespace EventDelagateAndMediator.EventDelegates.Order
{
  // Sipariş Nesnesi -> OrderService Bu nesne üzerinden çalışacak
  public record Order(String orderUUID, decimal ListPrice, int Quantity, string ProductName, string CustomerName);
}
