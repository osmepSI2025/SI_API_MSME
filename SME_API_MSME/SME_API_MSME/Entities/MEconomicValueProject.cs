using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class MEconomicValueProject
{
    public long ProjectCode { get; set; }

    public string? ProjectName { get; set; }

    public decimal? Budget { get; set; }

    public string BudgetYear { get; set; } = null!;


    //public List<TEconomicValue> TEconomicValues { get; set; }
    //public List<TEconomicValueSheets2> TEconomicValueSheets2s { get; set; } // Add this property
}
