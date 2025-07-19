using SME_API_MSME.Entities;
using SME_API_MSME.Models;
using SME_API_MSME.Repository;
using SME_API_MSME.Services;
public class BudgetPlanService
{
    private readonly BudgetPlanRepository _repository;
    private readonly ICallAPIService _serviceApi;
    private readonly IApiInformationRepository _repositoryApi;
    private readonly ProjectService _projectService;
    public BudgetPlanService(BudgetPlanRepository repository, ICallAPIService serviceApi,
        IApiInformationRepository repositoryApi
        , ProjectService projectService)

    {
        _repository = repository;
        _serviceApi = serviceApi;
        _repositoryApi = repositoryApi;
        _projectService = projectService;
    }

    public Task<IEnumerable<MBudgetPlan>> GetAllBudgetPlansAsync()
    {
        return _repository.GetAllAsync();
    }

    //public Task<MBudgetPlan?> GetBudgetPlanByIdAsync(int projectId)
    //{
    //    return _repository.GetByIdAsync(projectId);
    //}
    public async Task<ResultBudgetPlanResponse?> GetBudgetPlanByIdAsync(long? pProjectCode,string pYear)
    {
        var xrerult = new ResultBudgetPlanResponse();
        try
        {

            var dataResult = new List<BudgetPlanResult>();

            IEnumerable<MBudgetPlan>? result = null; // Initialize the variable to null

            if (pProjectCode == 0)
            {
                result = await _repository.GetAllAsync();
            }
            else
            {
                var resultPA = await _repository.GetByIdAsync(pProjectCode,pYear);

                if (resultPA == null)
                {
                    var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "budget-plan" });
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
                    }).FirstOrDefault(); // Use FirstOrDefault to handle empty lists

                    if (apiParam == null)
                    {
                        xrerult.responseCode = 500;
                        xrerult.responseMsg = "Api Service Inccorect.";
                        xrerult.result = new List<BudgetPlanResult>();
                        return xrerult;
                    }

                    var apiResponse = await _serviceApi.GetDataApiAsync_BudgetPlan(apiParam, pProjectCode,pYear);
                    if (apiResponse == null || apiResponse.result.Count == 0)
                    {
                        xrerult.responseCode = 200;
                        xrerult.responseMsg = "No data found";
                        xrerult.result = new List<BudgetPlanResult>();
                        return xrerult;
                    }
                    else
                    {
                        foreach (var item in apiResponse.result)
                        {
                            var proProduct = new MBudgetPlan
                            {
                                ProjectCode = item.ProjectCode, // Corrected from 'project.ProjectCode' to 'item.ProjectCode'
                                ProjectName = item.ProjectName,
                                Year = pYear,// Corrected from 'project.ProjectName' to 'item.ProjectName'
                                TBudgetPlans = item.Items.Select(i => new TBudgetPlan
                                {
                                    OrderIndex = i.OrderIndex ?? 0, // Handle nullable OrderIndex
                                    ItemActivityDetail = i.ItemActivityDetail,
                                    ActivityBudget = i.ActivityBudget ?? 0, // Handle nullable ActivityBudget
                                    ExpenseTypeName = i.ExpenseTypeName,
                                    TBudgeMonthlyPlanDetails = i.ActionResultDetail.Select(x => new TBudgeMonthlyPlanDetail
                                    {
                                        MonthName = x.MonthName,
                                        Year = x.Year, // Handle nullable Year
                                        ResultValue = x.ResultValue ?? 0 // Handle nullable ResultValue
                                        ,TempValue = x.TempValue
                                    }).ToList()
                                }).ToList()
                            };

                            await AddBudgetPlanAsync(proProduct);
                        }

                    }

                    result = pProjectCode == 0
          ? await _repository.GetAllAsync()
          : new List<MBudgetPlan> { await _repository.GetByIdAsync(pProjectCode,pYear) };

                }
                else
                {
                    result = new List<MBudgetPlan> { resultPA };
                }
            }

            if (result != null && result.Any())
            {
                dataResult.AddRange(result.Select(project => new BudgetPlanResult
                {
                    ProjectCode = project.ProjectCode,
                    ProjectName = project.ProjectName,
                    Items = project.TBudgetPlans.Select(item => new BudgetPlanItem
                    {
                        OrderIndex = item.OrderIndex ?? 0, // Handle nullable OrderIndex
                        ItemActivityDetail = item.ItemActivityDetail,
                        ActivityBudget = item.ActivityBudget ?? 0, // Corrected from ActivityWeight to ActivityBudget
                        ExpenseTypeName = item.ExpenseTypeName, // Corrected property name
                        ActionResultDetail = item.TBudgeMonthlyPlanDetails.Select(x => new BudgetPlanActionResultDetail
                        {
                            MonthName = x.MonthName,
                            Year = x.Year ?? 0, // Handle null Year by providing a default value
                            ResultValue = x.ResultValue ?? 0 // Handle nullable ResultValue
                             ,
                            TempValue = x.TempValue
                        }).ToList()
                    }).ToList()
                }).ToList());

                xrerult.responseCode = 200;
                xrerult.responseMsg = "success";
                xrerult.result = dataResult;
            }
            else
            {
                xrerult.responseCode = 200;
                xrerult.responseMsg = "No data found";
                xrerult.result = new List<BudgetPlanResult>();
            }

            return xrerult;
        }
        catch (Exception ex)
        {
            xrerult.responseCode = 500;
            xrerult.responseMsg = ex.Message;
            xrerult.result = new List<BudgetPlanResult>();
            return xrerult;
        }

    }
    public Task AddBudgetPlanAsync(MBudgetPlan budgetPlan)
    {
        return _repository.AddAsync(budgetPlan);
    }

    public Task UpdateBudgetPlanAsync(MBudgetPlan budgetPlan)
    {
        return _repository.UpdateAsync(budgetPlan);
    }

    public Task DeleteBudgetPlanAsync(int projectId)
    {
        return _repository.DeleteAsync(projectId);
    }

    public async Task<string> batchEndOfday()
    {
        int currentYear = DateTime.Now.Year;
        int currentYearBE = currentYear < 2500 ? currentYear + 543 : currentYear; // แปลงเป็น พ.ศ. ถ้ายังเป็น ค.ศ.


        for (int year = currentYearBE - 4; year <= currentYearBE; year++)
        {
            //get projects by year  
            var Listprojects = await _projectService.GetProjectByIdAsync(year.ToString());
            if (Listprojects == null || Listprojects.result.Count == 0)
            {
                continue; // Skip to the next year if no projects found
            }
            else if (Listprojects.responseCode == 200)
            {


                var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "budget-plan" });
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
                }).FirstOrDefault(); // Use FirstOrDefault to handle empty lists

                foreach (var item in Listprojects.result)
                {
                    var apiResponse = await _serviceApi.GetDataApiAsync_BudgetPlan(apiParam, item.ProjectCode,year.ToString());
                    if (apiResponse == null || apiResponse.responseCode == 0 || apiResponse.result.Count == 0)
                    {
                        continue; // Skip to the next project if no data found
                    }
                    else
                    {
                        foreach (var Subitem in apiResponse.result)
                        {
                            // Check if existing budget plan for the project
                            var resultPA = await _repository.GetByIdAsync(Subitem.ProjectCode,year.ToString());

                            if (resultPA == null)
                            {
                                var proProduct = new MBudgetPlan
                                {
                                    ProjectCode = Subitem.ProjectCode, // Corrected from 'project.ProjectCode' to 'item.ProjectCode'
                                    ProjectName = Subitem.ProjectName,
                                    Year = year.ToString(), // Corrected from 'project.ProjectName' to 'item.ProjectName'
                                    TBudgetPlans = Subitem.Items.Select(i => new TBudgetPlan
                                    {
                                        OrderIndex = i.OrderIndex ?? 0, // Handle nullable OrderIndex
                                        ItemActivityDetail = i.ItemActivityDetail,
                                        ActivityBudget = i.ActivityBudget ?? 0, // Handle nullable ActivityBudget
                                        ExpenseTypeName = i.ExpenseTypeName,
                                        TBudgeMonthlyPlanDetails = i.ActionResultDetail.Select(x => new TBudgeMonthlyPlanDetail
                                        {
                                            MonthName = x.MonthName,
                                            Year = x.Year, // Handle nullable Year
                                            ResultValue = x.ResultValue ?? 0 // Handle nullable ResultValue
                                        }).ToList()
                                    }).ToList()
                                };

                                await AddBudgetPlanAsync(proProduct);
                            }
                            else
                            {
                                // If it exists, update the existing budget plan
                                resultPA.ProjectName = Subitem.ProjectName; // Update project name if needed
                                resultPA.TBudgetPlans = Subitem.Items.Select(i => new TBudgetPlan
                                {
                                    OrderIndex = i.OrderIndex ?? 0, // Handle nullable OrderIndex
                                    ItemActivityDetail = i.ItemActivityDetail,
                                    ActivityBudget = i.ActivityBudget ?? 0, // Handle nullable ActivityBudget
                                    ExpenseTypeName = i.ExpenseTypeName,
                                    TBudgeMonthlyPlanDetails = i.ActionResultDetail.Select(x => new TBudgeMonthlyPlanDetail
                                    {
                                        MonthName = x.MonthName,
                                        Year = x.Year, // Handle nullable Year
                                        ResultValue = x.ResultValue ?? 0 // Handle nullable ResultValue
                                    }).ToList()
                                }).ToList();
                                await UpdateBudgetPlanAsync(resultPA);
                            }
                         
                        }

                    }

                }


            }

            return "Batch end of day process completed successfully.";
        }

        return "Success";
    }
}
