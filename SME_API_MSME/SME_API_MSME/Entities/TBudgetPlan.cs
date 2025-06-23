using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TBudgetPlan
{
    public int BudgetPlanId { get; set; }

    public int ProjectId { get; set; }

    public int? OrderIndex { get; set; }

    public string? ItemActivityDetail { get; set; }

    public decimal? ActivityBudget { get; set; }

    public string? ExpenseTypeName { get; set; }

    public virtual MBudgetPlan Project { get; set; } = null!;

    public virtual ICollection<TBudgeMonthlyPlanDetail> TBudgeMonthlyPlanDetails { get; set; } = new List<TBudgeMonthlyPlanDetail>();
}
