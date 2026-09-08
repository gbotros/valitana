using MediatR;
using Microsoft.AspNetCore.Mvc;
using Valitana.Application.Commands;

namespace Valitana.Api.Controllers;

[ApiController]
[Route("api/prices")]
public sealed class PricesController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Records a new price for a stock symbol.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetStockPrice(
        [FromBody] SetStockPriceRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new SetStockPriceCommand(request.Symbol, request.Price), cancellationToken);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public sealed record SetStockPriceRequest(string Symbol, decimal Price);
