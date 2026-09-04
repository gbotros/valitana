namespace Valitana.Domain.ValueObjects;

/// <summary>
/// Stock ticker symbol. Normalized (trimmed, upper-cased) and never empty.
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

    public override string ToString() => Value;
}
