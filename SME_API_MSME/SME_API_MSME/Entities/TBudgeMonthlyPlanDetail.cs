using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TBudgeMonthlyPlanDetail
{
    public int MonthlyDetailId { get; set; }

    public int BudgetPlanId { get; set; }

    public string? MonthName { get; set; }

    public string? Year { get; set; }

    public decimal? TempValue { get; set; }

    public decimal? ResultValue { get; set; }

    public virtual TBudgetPlan BudgetPlan { get; set; } = null!;
}
