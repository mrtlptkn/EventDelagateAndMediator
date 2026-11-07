using EventDelagateAndMediator.AsyncProgramming.Services;
using EventDelagateAndMediator.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<PricingService>();
builder.Services.AddSingleton<ShippingService>();

var app = builder.Build();


app.Use(async (context, next) =>
{
  await next(context);
});

app.UseHttpsRedirection();
app.MapEventDelegateEndpoints();
app.MapAsyncProgramingEndpoints();
app.Run();


