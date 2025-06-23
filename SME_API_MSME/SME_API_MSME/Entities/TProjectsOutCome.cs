using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TProjectsOutCome
{
    public int OutcomeId { get; set; }

    public int ProjectId { get; set; }

    public int? OrderIndex { get; set; }

    public string? OutputOutcomeName { get; set; }

    public string? YieldTypeName { get; set; }

    public string? MeasureResult { get; set; }

    public int? Target { get; set; }

    public string? UnitName { get; set; }

    public virtual MProjectsOutCome Project { get; set; } = null!;
}
