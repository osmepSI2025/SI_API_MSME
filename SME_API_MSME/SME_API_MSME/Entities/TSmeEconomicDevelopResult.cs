using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TSmeEconomicDevelopResult
{
    public int ResultId { get; set; }

    public int? SheetId { get; set; }

    public int? BusinessExpansion { get; set; }

    public int? Franchise { get; set; }

    public int? BusinessInvestment { get; set; }

    public int? BusinessSameSize { get; set; }

    public int? MicroToSmall { get; set; }

    public int? SmallToMedium { get; set; }

    public int? MediumToLarge { get; set; }

    public int? IncreaseEmployment { get; set; }

    public int? BusinessRegistration { get; set; }

    public int? PvBusinessExpansion { get; set; }

    public int? AmountFranchise { get; set; }

    public int? PvBusinessInvestment { get; set; }

    public int? FieldBusinessSameSize { get; set; }

    public int? FieldMicroToSmall { get; set; }

    public int? FieldSmallToMedium { get; set; }

    public int? FieldMediumToLarge { get; set; }

    public int? MaintainIncreaseEmployment { get; set; }

    public int? FieldBusinessRegistration { get; set; }

    public double? InvestmentBusinessExpansion { get; set; }

    public int? InvestmentFranchise { get; set; }

    public string? FieldBusinessInvestment { get; set; }

    public virtual TEconomicValueSheets2? Sheet { get; set; }
}
