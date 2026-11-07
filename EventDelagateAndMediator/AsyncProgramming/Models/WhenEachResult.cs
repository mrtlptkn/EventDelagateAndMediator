namespace EventDelagateAndMediator.AsyncProgramming.Models
{

  // Error ? Service ait hata mesajı
  // Status : Fail or Success
  // ElapsedMs : Ne kadar sürdü
  // Provider : Kargo şirketi
  // Fee: Kargo ücreti

  // birden fazla kargo şirketinin servisine gidelim, her birinin ne kadar süre ile cevap verdiğini bulup listeyen bir servis yapalım.

  public record WhenEachResult(string Provider,decimal? Fee, long ElapsedMs, string Status, string? Error);
 
}
