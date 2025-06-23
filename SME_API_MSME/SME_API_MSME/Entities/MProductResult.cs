using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class MProductResult
{
    public int ProjectId { get; set; }

    public long ProjectCode { get; set; }

    public string ProjectName { get; set; } = null!;

    public virtual ICollection<TProductResultOutput> TProductResultOutputs { get; set; } = new List<TProductResultOutput>();
}
