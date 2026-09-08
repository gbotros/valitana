using Valitana.Domain.ValueObjects;

namespace Valitana.UnitTests.Domain;

public sealed class PriceTests
{
    [Fact]
    public void From_accepts_a_positive_value()
    {
        var price = Price.From(12.3456m);

        Assert.Equal(12.3456m, price.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [InlineData(-10)]
    public void From_rejects_zero_and_negative_values(decimal value)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Price.From(value));

        Assert.Equal("value", ex.ParamName);
        Assert.Equal(value, ex.ActualValue);
    }

    [Fact]
    public void ToString_formats_up_to_four_decimal_places()
    {
        Assert.Equal("10", Price.From(10m).ToString());
        Assert.Equal("10.5", Price.From(10.5m).ToString());
        Assert.Equal("10.1235", Price.From(10.1235m).ToString());
    }
}
