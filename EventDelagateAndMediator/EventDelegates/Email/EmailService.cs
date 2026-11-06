using EventDelagateAndMediator.EventDelegates.Order;

namespace EventDelagateAndMediator.EventDelegates.Email
{
  public class EmailService
  {
    // EventHandler delegate imzasına sahip bir method.
    public void OnOrderPlaced(object? sender, OrderPlacedEventArgs e)
    {
      Console.WriteLine($"[Email] confimation sent to {e.customer} , total: {e.total}");
    }
  }
}
