using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TProjectActivityPlan
{
    public int PlanId { get; set; }

    public int ActivityId { get; set; }

    public string? MonthName { get; set; }

    public string? Year { get; set; }

    public decimal? TempValue { get; set; }

    public decimal? ResultValue { get; set; }

    public virtual TProjectActivity Activity { get; set; } = null!;
}
