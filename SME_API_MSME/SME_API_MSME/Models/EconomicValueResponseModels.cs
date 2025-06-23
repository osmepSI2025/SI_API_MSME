using SME_API_MSME.Entities;

public class ResultEconomicValueResponse
{
    public List<EconomicValueProjectModels> Result { get; set; }
    public int ResponseCode { get; set; }
    public string? ResponseMsg { get; set; }
}
public class ResultEconomicValueResponseEndUser
{
    public List<EconomicValueProjectModelsEndUser> Result { get; set; }
    public int ResponseCode { get; set; }
    public string? ResponseMsg { get; set; }
}
public class EconomicValueProjectModels
{
    public long ProjectCode { get; set; }
    public string? ProjectName { get; set; }
    public decimal? Budget { get; set; }
    public string? BudgetYear { get; set; }
    //public List<EconomicValueModels> Sheet1 { get; set; } = new List<EconomicValueModels>();
    public EconomicValueSheet1 Sheet1 { get; set; }
    public EconomicValueSheet2 Sheet2 { get; set; }
}
public class EconomicValueProjectModelsEndUser
{
    public long ProjectCode { get; set; }
    public string? ProjectName { get; set; }
    public decimal? Budget { get; set; }
    public string? BudgetYear { get; set; }
    public List<EconomicValueModels> Sheet1 { get; set; } = new List<EconomicValueModels>();
    public EconomicValueSheet2 Sheet2 { get; set; }
}

public class EconomicValueSheet1
{
    public List<EconomicValueModels> EconomicValue { get; set; }
}

public class EconomicValueModels
{
    public int EconomicValueId { get; set; }
    public decimal? MicroEnd { get; set; }
    public decimal? MicroNext { get; set; }
    public decimal? SmallEnd { get; set; }
    public decimal? SmallNext { get; set; }
    public decimal? MediumEnd { get; set; }
    public decimal? MediumNext { get; set; }
    public decimal? OtherEnd { get; set; }
    public decimal? OtherNext { get; set; }
}

public class EconomicValueSheet2
{
    public string? Province { get; set; }
    public string? InterestedBusiness { get; set; }
    public List<EconomicPromotedModels> EconomicPromoted { get; set; }
    public List<SmeEconomicDevelopModels> SmeEconomicDevelop { get; set; }
    public SmeEconomicFactorModels SmeEconomicFactor { get; set; }
    public SmeEconomicDevelopResultModels SmeEconomicDevelopResult { get; set; }
}

public class EconomicPromotedModels
{
    public int? EntrepreneurId { get; set; }
    public string? Production { get; set; }
    public string? Trade { get; set; }
    public string? Serve { get; set; }
    public string? Agribusiness { get; set; }
    public string? ProductionBranch { get; set; }
    public string? TradeBranch { get; set; }
    public string? ServeBranch { get; set; }
    public string? AgribusinessBranch { get; set; }
}

public class SmeEconomicDevelopModels
{
    public int? CapacityEnhanceId { get; set; }
    public string? BusinessBranch { get; set; }
    public decimal? Micro { get; set; }
    public decimal? Small { get; set; }
    public decimal? Medium { get; set; }
    public decimal? Other { get; set; }
    public string? Cluster { get; set; }
}

public class SmeEconomicFactorModels
{
    public decimal? LoginThaiSmeGp { get; set; }
    public decimal? ProductsServices { get; set; }
    public decimal? SmeEntrepreneurs { get; set; }
    public decimal? RegisterThaiSmeGp { get; set; }
    public decimal? NewEntrepreneur { get; set; }
    public decimal? OriginalEntrepreneur { get; set; }
    public decimal? ImproveLaw { get; set; }
    public string? UpdatedLaw { get; set; }
    public string? BusinessPlan { get; set; }
    public string? TrainingCourse { get; set; }
    public string? CourseName { get; set; }
    public decimal? PersonnelTrained { get; set; }
    public decimal? Bds { get; set; }
    public string? BusinessField { get; set; }
    public decimal? SupportMoney { get; set; }
    public decimal? AmountMicro { get; set; }
    public decimal? AmountSmall { get; set; }
    public decimal? SubsidyMedium { get; set; }
    public decimal? SubsidyOther { get; set; }
    public decimal? StoryDeveloped { get; set; }
    public decimal? BusinessServiceProvider { get; set; }
}

public class SmeEconomicDevelopResultModels
{
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
}





