namespace EventDelagateAndMediator.EventDelegates.Order.Rules
{
  public class OrderRules
  {

    // Burası sipariş işlemi yaparken dinamik kurallara göre bir validayon uygulaması sağlar.
    public List<(string Name, Func<Order, bool> Check)> Rules { get; } = new();

    public void AddRule(string name, Func<Order, bool> check)
    {
      Rules.Add((name, check));
    }
  }
}
