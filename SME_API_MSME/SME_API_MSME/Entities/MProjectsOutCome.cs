using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class MProjectsOutCome
{
    public int ProjectId { get; set; }

    public long ProjectCode { get; set; }

    public string ProjectName { get; set; } = null!;

    public virtual ICollection<TProjectsOutCome> TProjectsOutComes { get; set; } = new List<TProjectsOutCome>();
}
