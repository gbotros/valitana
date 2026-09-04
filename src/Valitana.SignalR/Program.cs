using Valitana.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
        .SetIsOriginAllowed(_ => true) // demo only, no auth
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));
builder.Services.AddHostedService<StockPriceConsumerService>();

var app = builder.Build();

app.UseCors();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.MapHub<PriceHub>("/hubs/prices");

app.Run();
