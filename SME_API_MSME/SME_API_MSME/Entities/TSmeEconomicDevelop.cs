using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TSmeEconomicDevelop
{
    public int DevelopId { get; set; }

    public int? SheetId { get; set; }

    public int? CapacityEnhanceId { get; set; }

    public string? BusinessBranch { get; set; }

    public int? Micro { get; set; }

    public int? Small { get; set; }

    public int? Medium { get; set; }

    public int? Other { get; set; }

    public string? Cluster { get; set; }

    public virtual TEconomicValueSheets2? Sheet { get; set; }
}
