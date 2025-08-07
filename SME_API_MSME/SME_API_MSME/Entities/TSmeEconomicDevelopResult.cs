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

    public string? PvBusinessExpansion { get; set; }

    public int? AmountFranchise { get; set; }

    public string? PvBusinessInvestment { get; set; }

    public string? FieldBusinessSameSize { get; set; }

    public string? FieldMicroToSmall { get; set; }

    public string? FieldSmallToMedium { get; set; }

    public string? FieldMediumToLarge { get; set; }

    public int? MaintainIncreaseEmployment { get; set; }

    public string? FieldBusinessRegistration { get; set; }

    public double? InvestmentBusinessExpansion { get; set; }

    public int? InvestmentFranchise { get; set; }

    public string? FieldBusinessInvestment { get; set; }

    public virtual TEconomicValueSheets2? Sheet { get; set; }
}
