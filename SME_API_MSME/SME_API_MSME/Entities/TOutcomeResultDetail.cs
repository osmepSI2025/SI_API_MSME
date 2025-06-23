using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TOutcomeResultDetail
{
    public int MonthlyOutcomeResultId { get; set; }

    public int OutcomeId { get; set; }

    public string? MonthName { get; set; }

    public string? Year { get; set; }

    public decimal? ResultOfYear { get; set; }

    public decimal? ResultOffEffect { get; set; }

    public string? Detail { get; set; }

    public string? Problem { get; set; }

    public string? Solution { get; set; }

    public virtual TOutcomeResult Outcome { get; set; } = null!;
}
