using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TExpectedFinalOutcome
{
    public int FinalOutcomeId { get; set; }

    public int ProjectId { get; set; }

    public int? OrderIndex { get; set; }

    public string? ResultFinalOutcome { get; set; }

    public decimal? SumBudget { get; set; }

    public virtual MExpectedOutcome Project { get; set; } = null!;

    public virtual ICollection<TExpectedFinalOutcomeMonthlyPlan> TExpectedFinalOutcomeMonthlyPlans { get; set; } = new List<TExpectedFinalOutcomeMonthlyPlan>();
}
