using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TEconomicPromoted
{
    public int PromotedId { get; set; }

    public int? SheetId { get; set; }

    public int? EntrepreneurId { get; set; }

    public string? Production { get; set; }

    public string? Trade { get; set; }

    public string? Serve { get; set; }

    public string? Agribusiness { get; set; }

    public string? ProductionBranch { get; set; }

    public string? TradeBranch { get; set; }

    public string? ServeBranch { get; set; }

    public string? AgribusinessBranch { get; set; }

    public virtual TEconomicValueSheets2? Sheet { get; set; }
}
