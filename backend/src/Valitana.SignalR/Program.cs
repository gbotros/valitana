using MassTransit;
using Valitana.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
        .SetIsOriginAllowed(_ => true) // demo only, no auth
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

var rabbit = builder.Configuration.GetSection("RabbitMq");
var host = rabbit["HostName"] ?? "localhost";
var port = ushort.TryParse(rabbit["Port"], out var parsedPort) ? parsedPort : (ushort)5672;
var user = rabbit["UserName"] ?? "guest";
var password = rabbit["Password"] ?? "guest";

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StockPriceUpdatedConsumer>();
    x.AddConfigureEndpointsCallback((_, _, cfg) =>
    {
        cfg.UseMessageRetry(r => r.Immediate(3));
    });
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(host, port, "/", h =>
        {
            h.Username(user);
            h.Password(password);
        });
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseCors();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.MapHub<PriceHub>("/hubs/prices");

app.Run();
