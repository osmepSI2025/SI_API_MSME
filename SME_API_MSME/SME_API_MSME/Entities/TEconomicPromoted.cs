using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TEconomicPromoted
{
    public int PromotedId { get; set; }

    public int? SheetId { get; set; }

    public int? EntrepreneurId { get; set; }

    public int? Production { get; set; }

    public int? Trade { get; set; }

    public int? Serve { get; set; }

    public int? Agribusiness { get; set; }

    public string? ProductionBranch { get; set; }

    public string? TradeBranch { get; set; }

    public string? ServeBranch { get; set; }

    public string? AgribusinessBranch { get; set; }

    public virtual TEconomicValueSheets2? Sheet { get; set; }
}
