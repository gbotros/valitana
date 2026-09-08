using Valitana.Domain.ValueObjects;

namespace Valitana.UnitTests.Domain;

public sealed class SymbolTests
{
    [Fact]
    public void From_trims_and_uppercases()
    {
        var symbol = Symbol.From(" aapl ");

        Assert.Equal("AAPL", symbol.Value);
        Assert.Equal("AAPL", symbol.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void From_rejects_null_empty_and_whitespace(string? value)
    {
        var ex = Assert.Throws<ArgumentException>(() => Symbol.From(value!));

        Assert.Equal("value", ex.ParamName);
    }
}
