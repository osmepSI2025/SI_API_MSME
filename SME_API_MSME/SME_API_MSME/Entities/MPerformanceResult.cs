using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class MPerformanceResult
{
    public int ProjectId { get; set; }

    public long ProjectCode { get; set; }

    public string ProjectName { get; set; } = null!;

    public virtual ICollection<TPerformanceResult> TPerformanceResults { get; set; } = new List<TPerformanceResult>();
}
