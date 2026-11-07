using EventDelagateAndMediator.AsyncProgramming.Services;
using EventDelagateAndMediator.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<PricingService>();
builder.Services.AddSingleton<ShippingService>();
builder.Services.AddSingleton<NotificationService>();

var app = builder.Build();


app.Use(async (context, next) =>
{
  await next(context);
});

app.UseHttpsRedirection();
app.MapEventDelegateEndpoints();
app.MapAsyncProgramingEndpoints();
app.Run();


