using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TSmeEconomicDevelopResult
{
    public int ResultId { get; set; }

    public int? SheetId { get; set; }

    public decimal? BusinessExpansion { get; set; }

    public decimal? Franchise { get; set; }

    public decimal? BusinessInvestment { get; set; }

    public decimal? BusinessSameSize { get; set; }

    public decimal? MicroToSmall { get; set; }

    public decimal? SmallToMedium { get; set; }

    public decimal? MediumToLarge { get; set; }

    public decimal? IncreaseEmployment { get; set; }

    public decimal? BusinessRegistration { get; set; }

    public decimal? PvBusinessExpansion { get; set; }

    public decimal? AmountFranchise { get; set; }

    public decimal? PvBusinessInvestment { get; set; }

    public decimal? FieldBusinessSameSize { get; set; }

    public decimal? FieldMicroToSmall { get; set; }

    public decimal? FieldSmallToMedium { get; set; }

    public decimal? FieldMediumToLarge { get; set; }

    public decimal? MaintainIncreaseEmployment { get; set; }

    public decimal? FieldBusinessRegistration { get; set; }

    public decimal? InvestmentBusinessExpansion { get; set; }

    public decimal? InvestmentFranchise { get; set; }

    public decimal? FieldBusinessInvestment { get; set; }

    public virtual TEconomicValueSheets2? Sheet { get; set; }
}
