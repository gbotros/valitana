namespace Valitana.Domain.ValueObjects;

/// <summary>
/// A strictly positive stock price. The invariant lives here, not in the aggregate.
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

    public override string ToString() => Value.ToString("0.####");
}
