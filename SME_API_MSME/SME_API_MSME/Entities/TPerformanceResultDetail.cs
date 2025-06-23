using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TPerformanceResultDetail
{
    public int MonthlyActivityResultId { get; set; }

    public int ActivityId { get; set; }

    public string? MonthName { get; set; }

    public string? Year { get; set; }

    public decimal? EffectValue { get; set; }

    public decimal? TempValue { get; set; }

    public virtual TPerformanceResult Activity { get; set; } = null!;
}
