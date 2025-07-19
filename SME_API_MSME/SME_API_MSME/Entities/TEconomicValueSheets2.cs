using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TEconomicValueSheets2
{
    public int SheetId { get; set; }

    public long? ProjectCode { get; set; }

    public string? Province { get; set; }

    public int? InterestedBusiness { get; set; }

    public virtual ICollection<TEconomicPromoted> TEconomicPromoteds { get; set; } = new List<TEconomicPromoted>();

    public virtual ICollection<TSmeEconomicDevelopResult> TSmeEconomicDevelopResults { get; set; } = new List<TSmeEconomicDevelopResult>();

    public virtual ICollection<TSmeEconomicDevelop> TSmeEconomicDevelops { get; set; } = new List<TSmeEconomicDevelop>();

    public virtual ICollection<TSmeEconomicFactor> TSmeEconomicFactors { get; set; } = new List<TSmeEconomicFactor>();
}
