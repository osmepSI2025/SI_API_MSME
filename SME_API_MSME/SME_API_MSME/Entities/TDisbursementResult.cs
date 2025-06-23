using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TDisbursementResult
{
    public int BudgetActivityId { get; set; }

    public int ProjectId { get; set; }

    public int? OrderIndex { get; set; }

    public string? ItemActivityDetail { get; set; }

    public decimal? ActivityBudget { get; set; }

    public string? ExpenseTypeName { get; set; }

    public decimal? SumEffectValue { get; set; }

    public virtual MDisbursementResult Project { get; set; } = null!;

    public virtual ICollection<TDisbursementResultDetail> TDisbursementResultDetails { get; set; } = new List<TDisbursementResultDetail>();
}
