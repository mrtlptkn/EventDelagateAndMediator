using EventDelagateAndMediator.Endpoints;

var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();


app.Use(async (context, next) =>
{
  await next(context);
});

app.UseHttpsRedirection();
app.MapEventDelegateEndpoints();
app.Run();


