using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class MBudgetPlan
{
    public int ProjectId { get; set; }

    public long ProjectCode { get; set; }

    public string ProjectName { get; set; } = null!;

    public virtual ICollection<TBudgetPlan> TBudgetPlans { get; set; } = new List<TBudgetPlan>();
}
