using System.Globalization;

namespace Valitana.Domain.ValueObjects;

/// <summary>
/// A stock price greater than zero.
/// </summary>
public sealed record Price
{
    public decimal Value { get; }

    private Price(decimal value) => Value = value;

    public static Price From(decimal value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Price must be greater than zero.");
        }

        return new Price(value);
    }

    public override string ToString()
    {
        return Value.ToString("0.####", CultureInfo.InvariantCulture);
    }
}
