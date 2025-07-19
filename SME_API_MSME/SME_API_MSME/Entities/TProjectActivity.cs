using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TProjectActivity
{
    public int ActivityId { get; set; }

    public int ProjectId { get; set; }

    public int? OrderIndex { get; set; }

    public string? ItemActivityDetail { get; set; }

    public double? ActivityWeight { get; set; }

    public double? Target { get; set; }

    public string? UnitName { get; set; }

    public virtual MProjectsActivity Project { get; set; } = null!;

    public virtual ICollection<TProjectActivityPlan> TProjectActivityPlans { get; set; } = new List<TProjectActivityPlan>();
}
