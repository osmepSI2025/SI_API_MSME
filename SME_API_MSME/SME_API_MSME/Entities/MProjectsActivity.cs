using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class MProjectsActivity
{
    public int ProjectId { get; set; }

    public long ProjectCode { get; set; }

    public string ProjectName { get; set; } = null!;

    public virtual ICollection<TProjectActivity> TProjectActivities { get; set; } = new List<TProjectActivity>();
}
