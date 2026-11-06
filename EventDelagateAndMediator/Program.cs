using EventDelagateAndMediator.Endpoints;

var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();

app.UseHttpsRedirection();
app.MapEventDelegateEndpoints();
app.Run();


