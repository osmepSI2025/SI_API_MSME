using SME_API_MSME.Entities;

public class ResultEconomicValueResponse
{
    public List<EconomicValueProjectModels> result { get; set; }
    public int responseCode { get; set; }
    public string? responseMsg { get; set; }
}
public class ResultEconomicValueResponseEndUser
{
    public List<EconomicValueProjectModelsEndUser> Result { get; set; }
    public int responseCode { get; set; }
    public string? responseMsg { get; set; }
}
public class EconomicValueProjectModels
{
    public long ProjectCode { get; set; }
    public string? ProjectName { get; set; }
    public double? Budget { get; set; }
    public int? BudgetYear { get; set; }
    //public List<EconomicValueModels> Sheet1 { get; set; } = new List<EconomicValueModels>();
    public EconomicValueSheet1 Sheet1 { get; set; }
    public EconomicValueSheet2 Sheet2 { get; set; }
}
public class EconomicValueProjectModelsEndUser
{
    public long ProjectCode { get; set; }
    public string? ProjectName { get; set; }
    public double? Budget { get; set; }
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
    public double? MicroEnd { get; set; }
    public double? MicroNext { get; set; }
    public double? SmallEnd { get; set; }
    public double? SmallNext { get; set; }
    public double? MediumEnd { get; set; }
    public double? MediumNext { get; set; }
    public double? OtherEnd { get; set; }
    public double? OtherNext { get; set; }
}
    
public class EconomicValueSheet2
{
    public string? Province { get; set; }
    public int? InterestedBusiness { get; set; }
    public List<EconomicPromotedModels> EconomicPromoted { get; set; }
    public List<SmeEconomicDevelopModels> SmeEconomicDevelop { get; set; }
    public SmeEconomicFactorModels SmeEconomicFactor { get; set; }
    public SmeEconomicDevelopResultModels SmeEconomicDevelopResult { get; set; }
}

public class EconomicPromotedModels
{
    public int? EntrepreneurId { get; set; }
    public int? Production { get; set; }
    public int? Trade { get; set; }
    public int? Serve { get; set; }
    public int? Agribusiness { get; set; }
    public string? ProductionBranch { get; set; }
    public string? TradeBranch { get; set; }
    public string? ServeBranch { get; set; }
    public string? AgribusinessBranch { get; set; }
}

public class SmeEconomicDevelopModels
{
    public int? CapacityEnhanceId { get; set; }
    public string? BusinessBranch { get; set; }
    public int? Micro { get; set; }
    public int? Small { get; set; }
    public int? Medium { get; set; }
    public int? Other { get; set; }
    public string? Cluster { get; set; }
}

public class SmeEconomicFactorModels
{
    public int? LoginThaiSmeGp { get; set; }
    public int? ProductsServices { get; set; }
    public int? SmeEntrepreneurs { get; set; }
    public int? RegisterThaiSmeGp { get; set; }
    public int? NewEntrepreneur { get; set; }
    public int? OriginalEntrepreneur { get; set; }
    public int? ImproveLaw { get; set; }
    public string? UpdatedLaw { get; set; }
    public string? BusinessPlan { get; set; }
    public string? TrainingCourse { get; set; }
    public string? CourseName { get; set; }
    public int? PersonnelTrained { get; set; }
    public int? Bds { get; set; }
    public string? BusinessField { get; set; }
    public int? SupportMoney { get; set; }
    public int? AmountMicro { get; set; }
    public int? AmountSmall { get; set; }
    public int? SubsidyMedium { get; set; }
    public int? SubsidyOther { get; set; }
    public int? StoryDeveloped { get; set; }
    public int? BusinessServiceProvider { get; set; }
}

public class SmeEconomicDevelopResultModels
{
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
}





