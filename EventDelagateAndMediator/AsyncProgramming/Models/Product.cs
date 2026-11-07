namespace EventDelagateAndMediator.AsyncProgramming.Models
{
  // record init only setter olarak çalışır, Immutable yapılardır. Sadece with keyword yeni bir record instance üretilerek değer değişikliği sağlanabilir.
  public record Product(string Id,string Name,decimal Price);
 
}
