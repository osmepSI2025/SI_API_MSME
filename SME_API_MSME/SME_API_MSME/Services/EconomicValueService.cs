using SME_API_MSME.Entities;
using SME_API_MSME.Models;
using SME_API_MSME.Repository;
using SME_API_MSME.Services;

public class EconomicValueService
{
    private readonly EconomicValueRepository _repository;
    private readonly ICallAPIService _serviceApi;
    private readonly IApiInformationRepository _repositoryApi;
    private readonly ProjectService _projectService;

    public EconomicValueService(EconomicValueRepository repository, ICallAPIService serviceApi,
        IApiInformationRepository repositoryApi, ProjectService projectService)
    {
        _repository = repository;
        _serviceApi = serviceApi;
        _repositoryApi = repositoryApi;
        _projectService = projectService;
    }

    public Task<IEnumerable<MEconomicValueProject>> GetAllEconomicValuesAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task<ResultEconomicValueResponse?> GetEconomicValueByIdAsync(long? pProjectCode, int? pyear)
    {
        var xrerult = new ResultEconomicValueResponse();
        try
        {
            var dataResult = new List<EconomicValueProjectModels>();

            IEnumerable<MEconomicValueProject>? result = null;

            if (pyear == 0)
            {
                result = await _repository.GetAllAsync();
            }
            else
            {
                var resultPA = await _repository.GetByIdAsync(pProjectCode, pyear);

                if (resultPA == null)
                {
                    var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "economic-value" });
                    var apiParam = LApi.Select(x => new MapiInformationModels
                    {
                        ServiceNameCode = x.ServiceNameCode,
                        ApiKey = x.ApiKey,
                        AuthorizationType = x.AuthorizationType,
                        ContentType = x.ContentType,
                        CreateDate = x.CreateDate,
                        Id = x.Id,
                        MethodType = x.MethodType,
                        ServiceNameTh = x.ServiceNameTh,
                        Urldevelopment = x.Urldevelopment,
                        Urlproduction = x.Urlproduction,
                        Username = x.Username,
                        Password = x.Password,
                        UpdateDate = x.UpdateDate,
                        Bearer = x.Bearer,
                    }).FirstOrDefault();

                    if (apiParam == null)
                    {
                        xrerult.responseCode = 500;
                        xrerult.responseMsg = "Api Service Incorrect.";
                        xrerult.result = new List<EconomicValueProjectModels>();
                        return xrerult;
                    }

                    var apiResponse = await _serviceApi.GetDataApiAsync_EconomicValue(apiParam, pProjectCode, pyear);
                    if (apiResponse == null || apiResponse.responseCode == 0 || apiResponse.result.Count == 0)
                    {
                        xrerult.responseCode = 200;
                        xrerult.responseMsg = "No data found";
                        xrerult.result = new List<EconomicValueProjectModels>();
                        return xrerult;
                    }
                    else
                    {
                        foreach (var item in apiResponse.result)
                        {
                            List<TEconomicValue> tecom = new List<TEconomicValue>();
                            var proProduct = new MEconomicValueProject
                            {
                                ProjectCode = item.ProjectCode,
                                ProjectName = item.ProjectName ?? "",
                                Budget = item.Budget ?? 0,
                                BudgetYear = item.BudgetYear ?? 0,

                            };
                            tecom = item.Sheet1?.EconomicValue?.Select(i => new TEconomicValue
                            {
                                MicroEnd = i.MicroEnd,
                                MicroNext = i.MicroNext,
                                SmallEnd = i.SmallEnd,
                                SmallNext = i.SmallNext,
                                MediumEnd = i.MediumEnd,
                                MediumNext = i.MediumNext,
                                OtherEnd = i.OtherEnd,
                                OtherNext = i.OtherNext,
                                EconomicValueId = i.EconomicValueId,
                            }).ToList() ?? new List<TEconomicValue>();
                            var sheet2x = new TEconomicValueSheets2
                            {
                                Province = item.Sheet2?.Province,
                                InterestedBusiness = item.Sheet2?.InterestedBusiness,
                                ProjectCode = item.ProjectCode,
                                TEconomicPromoteds = item.Sheet2?.EconomicPromoted?.Select(x => new TEconomicPromoted
                                {
                                    EntrepreneurId = x.EntrepreneurId,
                                    Production = x.Production,
                                    Trade = x.Trade,
                                    Serve = x.Serve,
                                    Agribusiness = x.Agribusiness,
                                    ProductionBranch = x.ProductionBranch,
                                    TradeBranch = x.TradeBranch,
                                    ServeBranch = x.ServeBranch,
                                    AgribusinessBranch = x.AgribusinessBranch
                                }).ToList(),
                                TSmeEconomicDevelops = item.Sheet2?.SmeEconomicDevelop?.Select(x => new TSmeEconomicDevelop
                                {
                                    CapacityEnhanceId = x.CapacityEnhanceId,
                                    BusinessBranch = x.BusinessBranch,
                                    Micro = x.Micro,
                                    Small = x.Small,
                                    Medium = x.Medium,
                                    Other = x.Other,
                                    Cluster = x.Cluster
                                }).ToList(),
                                TSmeEconomicFactors = item.Sheet2?.SmeEconomicFactor == null ? null : new List<TSmeEconomicFactor>
                                {
                                    new TSmeEconomicFactor
                                    {
                                        BusinessField = item.Sheet2.SmeEconomicFactor.BusinessField,
                                        BusinessPlan = item.Sheet2.SmeEconomicFactor.BusinessPlan,
                                        CourseName = item.Sheet2.SmeEconomicFactor.CourseName,
                                        PersonnelTrained = item.Sheet2.SmeEconomicFactor.PersonnelTrained,
                                        Bds = item.Sheet2.SmeEconomicFactor.Bds,
                                        SupportMoney = item.Sheet2.SmeEconomicFactor.SupportMoney,
                                        AmountMicro = item.Sheet2.SmeEconomicFactor.AmountMicro,
                                        AmountSmall = item.Sheet2.SmeEconomicFactor.AmountSmall,
                                        SubsidyMedium = item.Sheet2.SmeEconomicFactor.SubsidyMedium,
                                        SubsidyOther = item.Sheet2.SmeEconomicFactor.SubsidyOther
                                    }
                                },
                                TSmeEconomicDevelopResults = item.Sheet2?.SmeEconomicDevelopResult == null ? null : new List<TSmeEconomicDevelopResult>
                                {
                                    new TSmeEconomicDevelopResult
                                    {
                                        BusinessExpansion = item.Sheet2.SmeEconomicDevelopResult.BusinessExpansion,
                                        Franchise = item.Sheet2.SmeEconomicDevelopResult.Franchise,
                                        BusinessInvestment = item.Sheet2.SmeEconomicDevelopResult.BusinessInvestment,
                                        BusinessSameSize = item.Sheet2.SmeEconomicDevelopResult.BusinessSameSize,
                                        MicroToSmall = item.Sheet2.SmeEconomicDevelopResult.MicroToSmall,
                                        SmallToMedium = item.Sheet2.SmeEconomicDevelopResult.SmallToMedium,
                                        MediumToLarge = item.Sheet2.SmeEconomicDevelopResult.MediumToLarge,
                                        IncreaseEmployment = item.Sheet2.SmeEconomicDevelopResult.IncreaseEmployment,
                                        BusinessRegistration = item.Sheet2.SmeEconomicDevelopResult.BusinessRegistration,
                                        PvBusinessExpansion = item.Sheet2.SmeEconomicDevelopResult.PvBusinessExpansion,
                                        AmountFranchise = item.Sheet2.SmeEconomicDevelopResult.AmountFranchise,
                                        PvBusinessInvestment = item.Sheet2.SmeEconomicDevelopResult.PvBusinessInvestment,
                                        FieldBusinessSameSize = item.Sheet2.SmeEconomicDevelopResult.FieldBusinessSameSize,
                                        FieldMicroToSmall = item.Sheet2.SmeEconomicDevelopResult.FieldMicroToSmall,
                                        FieldSmallToMedium = item.Sheet2.SmeEconomicDevelopResult.FieldSmallToMedium,
                                        FieldMediumToLarge = item.Sheet2.SmeEconomicDevelopResult.FieldMediumToLarge,
                                        MaintainIncreaseEmployment = item.Sheet2.SmeEconomicDevelopResult.MaintainIncreaseEmployment,
                                        FieldBusinessRegistration = item.Sheet2.SmeEconomicDevelopResult.FieldBusinessRegistration,
                                        InvestmentBusinessExpansion = item.Sheet2.SmeEconomicDevelopResult.InvestmentBusinessExpansion,
                                        InvestmentFranchise = item.Sheet2.SmeEconomicDevelopResult.InvestmentFranchise,
                                        FieldBusinessInvestment = item.Sheet2.SmeEconomicDevelopResult.FieldBusinessInvestment
                                    }
                                }
                            };

                            await AddEconomicValueAsync(proProduct, tecom);
                            await AddEconomicValueSheet2Async(sheet2x);
                        }
                    }

                    result = pyear == 0
                        ? await _repository.GetAllAsync()
                        : new List<MEconomicValueProject> { await _repository.GetByIdAsync(pProjectCode,pyear??0) };
                }
                else
                {
                    result = new List<MEconomicValueProject> { resultPA };
                }
            }

            if (result != null && result.Any())
            {
                dataResult = result.Select(project => new EconomicValueProjectModels
                {
                    ProjectCode = project.ProjectCode,
                    ProjectName = project.ProjectName,
                    Budget = project.Budget,
                    BudgetYear = project.BudgetYear,
                    //Sheet1 = new EconomicValueSheet1
                    //{
                    //    EconomicValue = project.TEconomicValues.Select(x => new EconomicValueModels
                    //    {
                    //        EconomicValueId = x.EconomicValueId,
                    //        MicroEnd = x.MicroEnd,
                    //        MicroNext = x.MicroNext,
                    //        SmallEnd = x.SmallEnd,
                    //        SmallNext = x.SmallNext,
                    //        MediumEnd = x.MediumEnd,
                    //        MediumNext = x.MediumNext,
                    //        OtherEnd = x.OtherEnd,
                    //        OtherNext = x.OtherNext
                    //    }).ToList()
                    //},
                    //Sheet2 = new EconomicValueSheet2
                    //{
                    //    Province = project.TEconomicValueSheets2s.FirstOrDefault()?.Province,
                    //    InterestedBusiness = project.TEconomicValueSheets2s.FirstOrDefault()?.InterestedBusiness,
                    //    EconomicPromoted = project.TEconomicValueSheets2s
                    //        .SelectMany(s => s.TEconomicPromoteds)
                    //        .Select(x => new EconomicPromotedModels
                    //        {
                    //            EntrepreneurId = x.EntrepreneurId,
                    //            Production = x.Production,
                    //            Trade = x.Trade,
                    //            Serve = x.Serve,
                    //            Agribusiness = x.Agribusiness,
                    //            ProductionBranch = x.ProductionBranch,
                    //            TradeBranch = x.TradeBranch,
                    //            ServeBranch = x.ServeBranch,
                    //            AgribusinessBranch = x.AgribusinessBranch
                    //        }).ToList(),
                    //    SmeEconomicDevelop = project.TEconomicValueSheets2s
                    //        .SelectMany(s => s.TSmeEconomicDevelops)
                    //        .Select(x => new SmeEconomicDevelopModels
                    //        {
                    //            CapacityEnhanceId = x.CapacityEnhanceId,
                    //            BusinessBranch = x.BusinessBranch,
                    //            Micro = x.Micro,
                    //            Small = x.Small,
                    //            Medium = x.Medium,
                    //            Other = x.Other,
                    //            Cluster = x.Cluster
                    //        }).ToList(),
                    //    SmeEconomicFactor = project.TEconomicValueSheets2s
                    //        .FirstOrDefault()?
                    //        .TSmeEconomicFactors
                    //        .Select(x => new SmeEconomicFactorModels
                    //        {
                    //            BusinessField = x.BusinessField,
                    //            BusinessPlan = x.BusinessPlan,
                    //            CourseName = x.CourseName,
                    //            PersonnelTrained = x.PersonnelTrained,
                    //            Bds = x.Bds,
                    //            SupportMoney = x.SupportMoney,
                    //            AmountMicro = x.AmountMicro,
                    //            AmountSmall = x.AmountSmall,
                    //            SubsidyMedium = x.SubsidyMedium,
                    //            SubsidyOther = x.SubsidyOther
                    //        }).FirstOrDefault(),
                    //    SmeEconomicDevelopResult = project.TEconomicValueSheets2s
                    //        .FirstOrDefault()?
                    //        .TSmeEconomicDevelopResults
                    //        .Select(x => new SmeEconomicDevelopResultModels
                    //        {
                    //            BusinessExpansion = x.BusinessExpansion,
                    //            Franchise = x.Franchise,
                    //            BusinessInvestment = x.BusinessInvestment,
                    //            BusinessSameSize = x.BusinessSameSize,
                    //            MicroToSmall = x.MicroToSmall,
                    //            SmallToMedium = x.SmallToMedium,
                    //            MediumToLarge = x.MediumToLarge,
                    //            IncreaseEmployment = x.IncreaseEmployment,
                    //            BusinessRegistration = x.BusinessRegistration,
                    //            PvBusinessExpansion = x.PvBusinessExpansion,
                    //            AmountFranchise = x.AmountFranchise,
                    //            PvBusinessInvestment = x.PvBusinessInvestment,
                    //            FieldBusinessSameSize = x.FieldBusinessSameSize,
                    //            FieldMicroToSmall = x.FieldMicroToSmall,
                    //            FieldSmallToMedium = x.FieldSmallToMedium,
                    //            FieldMediumToLarge = x.FieldMediumToLarge,
                    //            MaintainIncreaseEmployment = x.MaintainIncreaseEmployment,
                    //            FieldBusinessRegistration = x.FieldBusinessRegistration,
                    //            InvestmentBusinessExpansion = x.InvestmentBusinessExpansion,
                    //            InvestmentFranchise = x.InvestmentFranchise,
                    //            FieldBusinessInvestment = x.FieldBusinessInvestment
                    //        }).FirstOrDefault()
                    //}
                }).ToList();

                xrerult.responseCode = 200;
                xrerult.responseMsg = "success";
                xrerult.result = dataResult;
            }
            else
            {
                xrerult.responseCode = 200;
                xrerult.responseMsg = "No data found";
                xrerult.result = new List<EconomicValueProjectModels>();
            }

            return xrerult;
        }
        catch (Exception ex)
        {
            xrerult.responseCode = 500;
            xrerult.responseMsg = ex.Message;
            xrerult.result = new List<EconomicValueProjectModels>();
            return xrerult;
        }
    }

    public Task AddEconomicValueAsync(MEconomicValueProject economicValue, List<TEconomicValue> tecom)
    {
        return _repository.AddAsync(economicValue, tecom);
    }

    public Task AddEconomicValueSheet2Async(TEconomicValueSheets2 economicValue)
    {
        return _repository.AddSheet2Async(economicValue);
    }

    public Task UpdateEconomicValueAsync(MEconomicValueProject economicValue, List<TEconomicValue> tecom)
    {
        return _repository.UpdateAsync(economicValue, tecom);
    }
    public Task UpdateEconomicValueSheet2Async(TEconomicValueSheets2 economicValue)
    {
        return _repository.UpdateSheet2Async(economicValue);
    }

    public Task DeleteEconomicValueAsync(int projectCode)
    {
        return _repository.DeleteAsync(projectCode);
    }

    public async Task<string> batchEndOfday()
    {
        int currentYear = DateTime.Now.Year;
        int currentYearBE = currentYear < 2500 ? currentYear + 543 : currentYear;
        var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "economic-value" });
        var apiParam = LApi.Select(x => new MapiInformationModels
        {
            ServiceNameCode = x.ServiceNameCode,
            ApiKey = x.ApiKey,
            AuthorizationType = x.AuthorizationType,
            ContentType = x.ContentType,
            CreateDate = x.CreateDate,
            Id = x.Id,
            MethodType = x.MethodType,
            ServiceNameTh = x.ServiceNameTh,
            Urldevelopment = x.Urldevelopment,
            Urlproduction = x.Urlproduction,
            Username = x.Username,
            Password = x.Password,
            UpdateDate = x.UpdateDate,
            Bearer = x.Bearer,
        }).FirstOrDefault();

        for (int year = currentYearBE - 5; year <= currentYearBE; year++)
        {
            var Listprojects = await _projectService.GetProjectByIdAsync(year.ToString());
            if (Listprojects == null || Listprojects.result.Count == 0)
            {
                continue; // Skip to the next year if no projects found
            }
            else
            {
                foreach (var itemPro in Listprojects.result)
                {
                    var apiResponse = await _serviceApi.GetDataApiAsync_EconomicValue(apiParam, itemPro.ProjectCode, year);
                    if (apiResponse == null || apiResponse.responseCode == 0 || apiResponse.result.Count == 0)
                    {
                        continue;
                    }
                    else
                    {
                        foreach (var Subitem in apiResponse.result)
                        {
                            var resultPA = await _repository.GetCheckByIdAsync(itemPro.ProjectCode, Subitem.BudgetYear??0);

                            if (resultPA == null)
                            {

                                foreach (var item in apiResponse.result)
                                {
                                    List<TEconomicValue> tecom = new List<TEconomicValue>();
                                    var proProduct = new MEconomicValueProject
                                    {
                                        ProjectCode = item.ProjectCode,
                                        ProjectName = item.ProjectName ?? "",
                                        Budget = item.Budget ?? 0,
                                        BudgetYear = item.BudgetYear ?? 0,


                                    };
                                    tecom = item.Sheet1?.EconomicValue?.Select(i => new TEconomicValue
                                    {
                                        MicroEnd = i.MicroEnd,
                                        MicroNext = i.MicroNext,
                                        SmallEnd = i.SmallEnd,
                                        SmallNext = i.SmallNext,
                                        MediumEnd = i.MediumEnd,
                                        MediumNext = i.MediumNext,
                                        OtherEnd = i.OtherEnd,
                                        OtherNext = i.OtherNext,
                                        EconomicValueId = i.EconomicValueId,
                                    }).ToList() ?? new List<TEconomicValue>();
                                    var sheet2x = new TEconomicValueSheets2
                                    {
                                        Province = item.Sheet2?.Province,
                                        InterestedBusiness = item.Sheet2?.InterestedBusiness,
                                        ProjectCode = item.ProjectCode,
                                        TEconomicPromoteds = item.Sheet2?.EconomicPromoted?.Select(x => new TEconomicPromoted
                                        {
                                            EntrepreneurId = x.EntrepreneurId,
                                            Production = x.Production,
                                            Trade = x.Trade,
                                            Serve = x.Serve,
                                            Agribusiness = x.Agribusiness,
                                            ProductionBranch = x.ProductionBranch,
                                            TradeBranch = x.TradeBranch,
                                            ServeBranch = x.ServeBranch,
                                            AgribusinessBranch = x.AgribusinessBranch
                                        }).ToList(),
                                        TSmeEconomicDevelops = item.Sheet2?.SmeEconomicDevelop?.Select(x => new TSmeEconomicDevelop
                                        {
                                            CapacityEnhanceId = x.CapacityEnhanceId,
                                            BusinessBranch = x.BusinessBranch,
                                            Micro = x.Micro,
                                            Small = x.Small,
                                            Medium = x.Medium,
                                            Other = x.Other,
                                            Cluster = x.Cluster
                                        }).ToList(),
                                        TSmeEconomicFactors = item.Sheet2?.SmeEconomicFactor == null ? null : new List<TSmeEconomicFactor>
                                {
                                    new TSmeEconomicFactor
                                    {
                                        BusinessField = item.Sheet2.SmeEconomicFactor.BusinessField,
                                        BusinessPlan = item.Sheet2.SmeEconomicFactor.BusinessPlan,
                                        CourseName = item.Sheet2.SmeEconomicFactor.CourseName,
                                        PersonnelTrained = item.Sheet2.SmeEconomicFactor.PersonnelTrained,
                                        Bds = item.Sheet2.SmeEconomicFactor.Bds,
                                        SupportMoney = item.Sheet2.SmeEconomicFactor.SupportMoney,
                                        AmountMicro = item.Sheet2.SmeEconomicFactor.AmountMicro,
                                        AmountSmall = item.Sheet2.SmeEconomicFactor.AmountSmall,
                                        SubsidyMedium = item.Sheet2.SmeEconomicFactor.SubsidyMedium,
                                        SubsidyOther = item.Sheet2.SmeEconomicFactor.SubsidyOther
                                    }
                                },
                                        TSmeEconomicDevelopResults = item.Sheet2?.SmeEconomicDevelopResult == null ? null : new List<TSmeEconomicDevelopResult>
                                {
                                    new TSmeEconomicDevelopResult
                                    {
                                        BusinessExpansion = item.Sheet2.SmeEconomicDevelopResult.BusinessExpansion,
                                        Franchise = item.Sheet2.SmeEconomicDevelopResult.Franchise,
                                        BusinessInvestment = item.Sheet2.SmeEconomicDevelopResult.BusinessInvestment,
                                        BusinessSameSize = item.Sheet2.SmeEconomicDevelopResult.BusinessSameSize,
                                        MicroToSmall = item.Sheet2.SmeEconomicDevelopResult.MicroToSmall,
                                        SmallToMedium = item.Sheet2.SmeEconomicDevelopResult.SmallToMedium,
                                        MediumToLarge = item.Sheet2.SmeEconomicDevelopResult.MediumToLarge,
                                        IncreaseEmployment = item.Sheet2.SmeEconomicDevelopResult.IncreaseEmployment,
                                        BusinessRegistration = item.Sheet2.SmeEconomicDevelopResult.BusinessRegistration,
                                        PvBusinessExpansion = item.Sheet2.SmeEconomicDevelopResult.PvBusinessExpansion,
                                        AmountFranchise = item.Sheet2.SmeEconomicDevelopResult.AmountFranchise,
                                        PvBusinessInvestment = item.Sheet2.SmeEconomicDevelopResult.PvBusinessInvestment,
                                        FieldBusinessSameSize = item.Sheet2.SmeEconomicDevelopResult.FieldBusinessSameSize,
                                        FieldMicroToSmall = item.Sheet2.SmeEconomicDevelopResult.FieldMicroToSmall,
                                        FieldSmallToMedium = item.Sheet2.SmeEconomicDevelopResult.FieldSmallToMedium,
                                        FieldMediumToLarge = item.Sheet2.SmeEconomicDevelopResult.FieldMediumToLarge,
                                        MaintainIncreaseEmployment = item.Sheet2.SmeEconomicDevelopResult.MaintainIncreaseEmployment,
                                        FieldBusinessRegistration = item.Sheet2.SmeEconomicDevelopResult.FieldBusinessRegistration,
                                        InvestmentBusinessExpansion = item.Sheet2.SmeEconomicDevelopResult.InvestmentBusinessExpansion,
                                        InvestmentFranchise = item.Sheet2.SmeEconomicDevelopResult.InvestmentFranchise,
                                        FieldBusinessInvestment = item.Sheet2.SmeEconomicDevelopResult.FieldBusinessInvestment
                                    }
                                }
                                    };

                                    await AddEconomicValueAsync(proProduct, tecom);

                                    await AddEconomicValueSheet2Async(sheet2x);
                                }
                            }
                            else
                            {
                                resultPA.ProjectName = Subitem.ProjectName;
                                resultPA.Budget = Subitem.Budget ?? 0;
                                resultPA.BudgetYear = Subitem.BudgetYear ?? 0;
                                List<TEconomicValue> tecom = new List<TEconomicValue>();
                                //   resultPA.TEconomicValues.Clear();
                                if (Subitem.Sheet1?.EconomicValue != null)
                                {
                                    foreach (var i in Subitem.Sheet1.EconomicValue)
                                    {
                                        tecom.Add(new TEconomicValue
                                        {
                                            MicroEnd = i.MicroEnd,
                                            MicroNext = i.MicroNext,
                                            SmallEnd = i.SmallEnd,
                                            SmallNext = i.SmallNext,
                                            MediumEnd = i.MediumEnd,
                                            MediumNext = i.MediumNext,
                                            OtherEnd = i.OtherEnd,
                                            OtherNext = i.OtherNext,
                                            EconomicValueId = i.EconomicValueId,
                                        });
                                    }
                                }
                                TEconomicValueSheets2 shet2 = new TEconomicValueSheets2();
                                if (Subitem.Sheet2 != null)
                                {
                                    shet2 = new TEconomicValueSheets2
                                    {

                                        ProjectCode = Subitem.ProjectCode,
                                        Province = Subitem.Sheet2.Province,
                                        InterestedBusiness = Subitem.Sheet2.InterestedBusiness,
                                        TEconomicPromoteds = Subitem.Sheet2.EconomicPromoted?.Select(x => new TEconomicPromoted
                                        {
                                            EntrepreneurId = x.EntrepreneurId,
                                            Production = x.Production,
                                            Trade = x.Trade,
                                            Serve = x.Serve,
                                            Agribusiness = x.Agribusiness,
                                            ProductionBranch = x.ProductionBranch,
                                            TradeBranch = x.TradeBranch,
                                            ServeBranch = x.ServeBranch,
                                            AgribusinessBranch = x.AgribusinessBranch
                                        }).ToList(),
                                        TSmeEconomicDevelops = Subitem.Sheet2.SmeEconomicDevelop?.Select(x => new TSmeEconomicDevelop
                                        {
                                            CapacityEnhanceId = x.CapacityEnhanceId,
                                            BusinessBranch = x.BusinessBranch,
                                            Micro = x.Micro,
                                            Small = x.Small,
                                            Medium = x.Medium,
                                            Other = x.Other,
                                            Cluster = x.Cluster
                                        }).ToList(),
                                        TSmeEconomicFactors = Subitem.Sheet2.SmeEconomicFactor == null ? null : new List<TSmeEconomicFactor>
        {
            new TSmeEconomicFactor
            {
                BusinessField = Subitem.Sheet2.SmeEconomicFactor.BusinessField,
                BusinessPlan = Subitem.Sheet2.SmeEconomicFactor.BusinessPlan,
                CourseName = Subitem.Sheet2.SmeEconomicFactor.CourseName,
                PersonnelTrained = Subitem.Sheet2.SmeEconomicFactor.PersonnelTrained,
                Bds = Subitem.Sheet2.SmeEconomicFactor.Bds,
                SupportMoney = Subitem.Sheet2.SmeEconomicFactor.SupportMoney,
                AmountMicro = Subitem.Sheet2.SmeEconomicFactor.AmountMicro,
                AmountSmall = Subitem.Sheet2.SmeEconomicFactor.AmountSmall,
                SubsidyMedium = Subitem.Sheet2.SmeEconomicFactor.SubsidyMedium,
                SubsidyOther = Subitem.Sheet2.SmeEconomicFactor.SubsidyOther
            }
        },
                                        TSmeEconomicDevelopResults = Subitem.Sheet2.SmeEconomicDevelopResult == null ? null : new List<TSmeEconomicDevelopResult>
        {
            new TSmeEconomicDevelopResult
            {
                BusinessExpansion = Subitem.Sheet2.SmeEconomicDevelopResult.BusinessExpansion,
                Franchise = Subitem.Sheet2.SmeEconomicDevelopResult.Franchise,
                BusinessInvestment = Subitem.Sheet2.SmeEconomicDevelopResult.BusinessInvestment,
                BusinessSameSize = Subitem.Sheet2.SmeEconomicDevelopResult.BusinessSameSize,
                MicroToSmall = Subitem.Sheet2.SmeEconomicDevelopResult.MicroToSmall,
                SmallToMedium = Subitem.Sheet2.SmeEconomicDevelopResult.SmallToMedium,
                MediumToLarge = Subitem.Sheet2.SmeEconomicDevelopResult.MediumToLarge,
                IncreaseEmployment = Subitem.Sheet2.SmeEconomicDevelopResult.IncreaseEmployment,
                BusinessRegistration = Subitem.Sheet2.SmeEconomicDevelopResult.BusinessRegistration,
                PvBusinessExpansion = Subitem.Sheet2.SmeEconomicDevelopResult.PvBusinessExpansion,
                AmountFranchise = Subitem.Sheet2.SmeEconomicDevelopResult.AmountFranchise,
                PvBusinessInvestment = Subitem.Sheet2.SmeEconomicDevelopResult.PvBusinessInvestment,
                FieldBusinessSameSize = Subitem.Sheet2.SmeEconomicDevelopResult.FieldBusinessSameSize,
                FieldMicroToSmall = Subitem.Sheet2.SmeEconomicDevelopResult.FieldMicroToSmall,
                FieldSmallToMedium = Subitem.Sheet2.SmeEconomicDevelopResult.FieldSmallToMedium,
                FieldMediumToLarge = Subitem.Sheet2.SmeEconomicDevelopResult.FieldMediumToLarge,
                MaintainIncreaseEmployment = Subitem.Sheet2.SmeEconomicDevelopResult.MaintainIncreaseEmployment,
                FieldBusinessRegistration = Subitem.Sheet2.SmeEconomicDevelopResult.FieldBusinessRegistration,
                InvestmentBusinessExpansion = Subitem.Sheet2.SmeEconomicDevelopResult.InvestmentBusinessExpansion,
                InvestmentFranchise = Subitem.Sheet2.SmeEconomicDevelopResult.InvestmentFranchise,
                FieldBusinessInvestment = Subitem.Sheet2.SmeEconomicDevelopResult.FieldBusinessInvestment
            }
        }
                                    };
                                }

                                await UpdateEconomicValueAsync(resultPA, tecom);
                                await UpdateEconomicValueSheet2Async(shet2);
                            }
                        }
                    }
                }
            }


        }

        return "Batch end of day process completed successfully.";
    }
}


