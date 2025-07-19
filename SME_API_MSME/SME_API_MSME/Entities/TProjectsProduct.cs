using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TProjectsProduct
{
    public int OutputId { get; set; }

    public int ProjectId { get; set; }

    public int? OrderIndex { get; set; }

    public string? OutputOutcomeName { get; set; }

    public string? YieldTypeName { get; set; }

    public string? MeasureResult { get; set; }

    public double? Target { get; set; }

    public string? CountUnitName { get; set; }

    public virtual MProjectsProduct Project { get; set; } = null!;
}
