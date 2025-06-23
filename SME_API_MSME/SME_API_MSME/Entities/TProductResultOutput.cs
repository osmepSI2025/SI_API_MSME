using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TProductResultOutput
{
    public int OutputId { get; set; }

    public int ProjectId { get; set; }

    public int? OrderIndex { get; set; }

    public string? OutputOutcomeName { get; set; }

    public string? YieldTypeName { get; set; }

    public int? Target { get; set; }

    public string? UnitName { get; set; }

    public virtual MProductResult Project { get; set; } = null!;

    public virtual ICollection<TProductResultOutputDetail> TProductResultOutputDetails { get; set; } = new List<TProductResultOutputDetail>();
}
