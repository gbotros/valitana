namespace Valitana.Domain.ValueObjects;

/// <summary>
/// Ticker; trimmed, upper-cased, not empty.
/// </summary>
public sealed record Symbol
{
    public string Value { get; }

    private Symbol(string value) => Value = value;

    public static Symbol From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Symbol must be a non-empty string.", nameof(value));
        }

        return new Symbol(value.Trim().ToUpperInvariant());
    }

    public override string ToString()
    {
        return Value;
    }
}
