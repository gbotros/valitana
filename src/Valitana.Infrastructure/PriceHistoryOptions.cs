using System.ComponentModel.DataAnnotations;

namespace Valitana.Infrastructure;

public sealed class PriceHistoryOptions
{
    public const string SectionName = "PriceHistory";

    /// <summary>Maximum number of ticks kept in memory; oldest is dropped on overflow.</summary>
    [Range(1, int.MaxValue)]
    public int MaxCount { get; set; } = 5;
}
