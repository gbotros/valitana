using MediatR;

namespace Valitana.Application.Commands;

public sealed record SetStockPriceCommand(string Symbol, decimal Price) : IRequest;
