using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TExpectedOutcomeResultDetail
{
    public int MonthlyOutcomeId { get; set; }

    public int OutcomeId { get; set; }

    public string? MonthName { get; set; }

    public int? Year { get; set; }

    public double? ResultValue { get; set; }

    public virtual TExpectedOutcomeResult Outcome { get; set; } = null!;
}
