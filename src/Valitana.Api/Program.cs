using MediatR;
using Valitana.Application;
using Valitana.Application.Commands;
using Valitana.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapPost("/api/prices", async (SetStockPriceRequest request, ISender sender, CancellationToken cancellationToken) =>
{
    try
    {
        await sender.Send(new SetStockPriceCommand(request.Symbol, request.Price), cancellationToken);
        return Results.NoContent();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("SetStockPrice")
.WithSummary("Records a new price for a stock symbol.");

app.Run();

/// <summary>Request body for POST /api/prices.</summary>
public sealed record SetStockPriceRequest(string Symbol, decimal Price);
