using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TProductResultOutputDetail
{
    public int MonthlyResultId { get; set; }

    public int OutputId { get; set; }

    public string? MonthName { get; set; }

    public int? Year { get; set; }

    public double? ResultOfYear { get; set; }

    public double? ResultOffEffect { get; set; }

    public string? Detail { get; set; }

    public string? Problem { get; set; }

    public string? Solution { get; set; }

    public virtual TProductResultOutput Output { get; set; } = null!;
}
