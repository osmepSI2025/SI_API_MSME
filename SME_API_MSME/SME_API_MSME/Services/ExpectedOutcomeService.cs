using SME_API_MSME.Entities;
using SME_API_MSME.Models;
using SME_API_MSME.Repository;
using SME_API_MSME.Services;
public class ExpectedOutcomeService
{
    private readonly ExpectedOutcomeRepository _repository;
    private readonly ICallAPIService _serviceApi;
    private readonly IApiInformationRepository _repositoryApi;

    private readonly ProjectService _projectService;
    public ExpectedOutcomeService(ExpectedOutcomeRepository repository, ICallAPIService serviceApi
        , IApiInformationRepository repositoryApi, ProjectService projectService)

    {
        _repository = repository;
        _serviceApi = serviceApi;
        _repositoryApi = repositoryApi;
        _projectService = projectService;
    }

    public Task<IEnumerable<MExpectedOutcome>> GetAllExpectedOutcomesAsync()
    {
        return _repository.GetAllAsync();
    }

    //public Task<MExpectedOutcome?> GetExpectedOutcomeByIdAsync(int projectId)
    //{
    //    return _repository.GetByIdAsync(projectId);
    //}
    public async Task<ResultExpectOutcomeResponse?> GetExpectedOutcomeByIdAsync(long? pProjectCode)
    {
        var xrerult = new ResultExpectOutcomeResponse();
        try
        {

            var dataResult = new List<ExpectOutcomeResult>();

            IEnumerable<MExpectedOutcome>? result = null; // Initialize the variable to null

            if (pProjectCode == 0)
            {
                result = await _repository.GetAllAsync();
            }
            else
            {
                var resultPA = await _repository.GetByIdAsync(pProjectCode);

                if (resultPA == null)
                {
                    var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "expected-outcome" });
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
                        xrerult.ResponseCode = 500;
                        xrerult.ResponseMsg = "Api Service Inccorect.";
                        xrerult.Result = new List<ExpectOutcomeResult>();
                        return xrerult;

                    }

                    var apiResponse = await _serviceApi.GetDataApiAsync_ExpectOutcome(apiParam, pProjectCode);
                  if (apiResponse == null || apiResponse.ResponseCode == 0 || apiResponse.Result.Count==0)
                    {
                        xrerult.ResponseCode = 200;
                        xrerult.ResponseMsg = "No data found";
                        xrerult.Result =new List<ExpectOutcomeResult>();
                        return xrerult;
                    }

                    else
                    {
                        foreach (var item in apiResponse.Result)
                        {
                            var proProduct = new MExpectedOutcome
                            {
                                ProjectCode = item.ProjectCode, // Corrected from 'project.ProjectCode' to 'item.ProjectCode'
                                ProjectName = item.ProjectName, // Corrected from 'project.ProjectName' to 'item.ProjectName'
                                TExpectedFinalOutcomes = item.Items.Select(i => new TExpectedFinalOutcome
                                {
                                    OrderIndex = i.OrderIndex,
                                    ResultFinalOutcome = i.ResultFinalOutcome,
                                    SumBudget = i.SumBudget,
                                    TExpectedFinalOutcomeMonthlyPlans = i.FinalOutcomeDetail.Select(x => new TExpectedFinalOutcomeMonthlyPlan
                                    {
                                        MonthName = x.MonthName,
                                        Year = x.Year,
                                        ResultValue = x.ResultValue
                                    }).ToList()
                                }).ToList()
                            };

                            await AddExpectedOutcomeAsync(proProduct);
                        }
                    }

                    result = pProjectCode == 0
          ? await _repository.GetAllAsync()
          : new List<MExpectedOutcome> { await _repository.GetByIdAsync(pProjectCode) };

                }
                else
                {
                    result = new List<MExpectedOutcome> { resultPA };
                }
            }

            if (result != null && result.Any())
            {
                dataResult.AddRange(result.Select(project => new ExpectOutcomeResult
                {
                    ProjectCode = project.ProjectCode,
                    ProjectName = project.ProjectName,
                    Items = project.TExpectedFinalOutcomes.Select(item => new ExpectOutcomeItem
                    {
                        OrderIndex = item.OrderIndex,
                        ResultFinalOutcome = item.ResultFinalOutcome,
                        SumBudget = item.SumBudget,
                        FinalOutcomeDetail = item.TExpectedFinalOutcomeMonthlyPlans.Select(x => new ExpectOutcomeFinalOutcomeDetail
                        {
                            MonthName = x.MonthName,
                            Year = x.Year,
                            ResultValue = x.ResultValue
                        }).ToList()
                    }).ToList()
                }).ToList());

                xrerult.ResponseCode = 200;
                xrerult.ResponseMsg = "success";
                xrerult.Result = dataResult;
            }
            else
            {
               xrerult.ResponseCode = 200;
                xrerult.ResponseMsg = "No data found";
                xrerult.Result = new List<ExpectOutcomeResult>();
            }

            return xrerult;
        }
        catch (Exception ex)
        {
            xrerult.ResponseCode = 500;
            xrerult.ResponseMsg = ex.Message;
            xrerult.Result = new List<ExpectOutcomeResult>();
            return xrerult;
        }

    }

    public Task AddExpectedOutcomeAsync(MExpectedOutcome expectedOutcome)
    {
        return _repository.AddAsync(expectedOutcome);
    }

    public Task UpdateExpectedOutcomeAsync(MExpectedOutcome expectedOutcome)
    {
        return _repository.UpdateAsync(expectedOutcome);
    }

    public Task DeleteExpectedOutcomeAsync(int projectId)
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
            if (Listprojects == null || Listprojects.Result.Count == 0)
            {
                continue; // Skip to the next year if no projects found
            }
            else if (Listprojects.ResponseCode == 200)
            {


                var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "expected-outcome" });
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

                foreach (var item in Listprojects.Result)
                {
                    var apiResponse = await _serviceApi.GetDataApiAsync_ExpectOutcome(apiParam, item.ProjectCode);
                    if (apiResponse == null || apiResponse.ResponseCode == 0 || apiResponse.Result.Count == 0)
                    {
                        continue; // Skip to the next project if no data found
                    }
                    else
                    {
                        foreach (var Subitem in apiResponse.Result)
                        {
                            // Check if existing budget plan for the project
                            var resultPA = await _repository.GetByIdAsync(Subitem.ProjectCode);

                            var proProduct = new MExpectedOutcome
                            {
                                ProjectId = resultPA?.ProjectId ?? 0, // Assuming ProjectId is available in the item
                                ProjectCode = Subitem.ProjectCode,
                                ProjectName = Subitem.ProjectName,
                                TExpectedFinalOutcomes = Subitem.Items?.Select(i => new TExpectedFinalOutcome
                                {
                                    OrderIndex = i.OrderIndex,
                                    ResultFinalOutcome = i.ResultFinalOutcome,
                                    SumBudget = i.SumBudget,
                                    TExpectedFinalOutcomeMonthlyPlans = i.FinalOutcomeDetail?.Select(x => new TExpectedFinalOutcomeMonthlyPlan
                                    {
                                        MonthName = x.MonthName,
                                        Year = x.Year,
                                        ResultValue = x.ResultValue
                                    }).ToList() ?? new List<TExpectedFinalOutcomeMonthlyPlan>()
                                }).ToList() ?? new List<TExpectedFinalOutcome>()
                            };

                            if (resultPA == null)
                            {
                                await AddExpectedOutcomeAsync(proProduct);
                            }
                            else
                            {
                                await UpdateExpectedOutcomeAsync(proProduct);
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
