using EventDelagateAndMediator.EventDelegates.Order.Rules;

namespace EventDelagateAndMediator.EventDelegates.Order.Services
{
  public class OrderService
  {
    // fields
    private readonly Action<string> _log;
    private readonly OrderRules _orderRules;
    private readonly DiscountStrategyDelegate _discountStrategy;

    // Reject edilen değer ile ilgili neden reject olduğunu döndürdüğümüz bir event
    public event Action<string> OrderRejected;

    public event EventHandler<OrderPlacedEventArgs> OrderPlaced;

    public OrderService(Action<string> log, OrderRules orderRules, DiscountStrategyDelegate discountStrategy)
    {
      _log = log;
      _orderRules = orderRules;
      _discountStrategy = discountStrategy;
    }
    public void SubmitOrder(OrderRequest order)
    {
      _log($"Processing Order for {order.orderUUID}");

      foreach (var rule in _orderRules.Rules)
      {
        if (!rule.Check(order)) // kurala uyulmadığında ne yapılacak
        {
          OrderRejected?.Invoke(rule.Name);
          return;
        }
      }

      var discountedTotal = _discountStrategy(order.Quantity * order.ListPrice);
      _log($"Order {order.orderUUID} total after discount tutar {discountedTotal}");
      OrderPlaced?.Invoke(this, new OrderPlacedEventArgs(order.orderUUID, discountedTotal, order.CustomerName));

    }

  }
}
