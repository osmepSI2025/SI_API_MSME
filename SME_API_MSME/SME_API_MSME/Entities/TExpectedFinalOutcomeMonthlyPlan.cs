using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TExpectedFinalOutcomeMonthlyPlan
{
    public int MonthlyPlanId { get; set; }

    public int FinalOutcomeId { get; set; }

    public string? MonthName { get; set; }

    public int? Year { get; set; }

    public decimal? ResultValue { get; set; }

    public virtual TExpectedFinalOutcome FinalOutcome { get; set; } = null!;
}
