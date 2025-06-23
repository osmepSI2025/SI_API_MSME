using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TSmeEconomicDevelop
{
    public int DevelopId { get; set; }

    public int? SheetId { get; set; }

    public int? CapacityEnhanceId { get; set; }

    public string? BusinessBranch { get; set; }

    public decimal? Micro { get; set; }

    public decimal? Small { get; set; }

    public decimal? Medium { get; set; }

    public decimal? Other { get; set; }

    public string? Cluster { get; set; }

    public virtual TEconomicValueSheets2? Sheet { get; set; }
}
