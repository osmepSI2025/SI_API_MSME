using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TPerformanceResult
{
    public int ActivityId { get; set; }

    public int ProjectId { get; set; }

    public int? OrderIndex { get; set; }

    public string? ItemActivityDetail { get; set; }

    public string? ExpenseTypeName { get; set; }

    public double? ActivityWeight { get; set; }

    public double? Target { get; set; }

    public string? UnitName { get; set; }

    public double? SumEffectValue { get; set; }

    public virtual MPerformanceResult Project { get; set; } = null!;

    public virtual ICollection<TPerformanceResultDetail> TPerformanceResultDetails { get; set; } = new List<TPerformanceResultDetail>();
}
