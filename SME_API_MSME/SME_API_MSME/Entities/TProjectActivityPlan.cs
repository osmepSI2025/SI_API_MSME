using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TProjectActivityPlan
{
    public int PlanId { get; set; }

    public int ActivityId { get; set; }

    public string? MonthName { get; set; }

    public int? Year { get; set; }

    public double? TempValue { get; set; }

    public double? ResultValue { get; set; }

    public virtual TProjectActivity Activity { get; set; } = null!;
}
