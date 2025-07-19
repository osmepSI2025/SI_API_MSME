using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class MProjectsProduct
{
    public int ProjectId { get; set; }

    public long ProjectCode { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? Year { get; set; }

    public virtual ICollection<TProjectsProduct> TProjectsProducts { get; set; } = new List<TProjectsProduct>();
}
