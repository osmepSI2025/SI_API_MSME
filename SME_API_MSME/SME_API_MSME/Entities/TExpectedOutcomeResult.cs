using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TExpectedOutcomeResult
{
    public int OutcomeId { get; set; }

    public int ProjectId { get; set; }

    public int? OrderIndex { get; set; }

    public string? ResultFinalOutcome { get; set; }

    public decimal? SumBudget { get; set; }

    public virtual MExpectedOutcomeResult Project { get; set; } = null!;

    public virtual ICollection<TExpectedOutcomeResultDetail> TExpectedOutcomeResultDetails { get; set; } = new List<TExpectedOutcomeResultDetail>();
}
