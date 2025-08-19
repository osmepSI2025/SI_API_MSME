using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class MEconomicValueProject
{
    public long ProjectCode { get; set; }

    public string? ProjectName { get; set; }

    public double? Budget { get; set; }

    public int BudgetYear { get; set; }
}
