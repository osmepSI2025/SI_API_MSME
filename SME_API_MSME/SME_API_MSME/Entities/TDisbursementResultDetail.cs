using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TDisbursementResultDetail
{
    public int MonthlyDisbursementId { get; set; }

    public int BudgetActivityId { get; set; }

    public string? MonthName { get; set; }

    public int? Year { get; set; }

    public double? TempValue { get; set; }

    public double? EffectValue { get; set; }

    public virtual TDisbursementResult BudgetActivity { get; set; } = null!;
}
