namespace EventDelagateAndMediator.EventDelegates.Order
{
  // Sipariş Nesnesi -> OrderService Bu nesne üzerinden çalışacak
  public record OrderRequest(String orderUUID, decimal ListPrice, int Quantity, string ProductName, string CustomerName);
}
