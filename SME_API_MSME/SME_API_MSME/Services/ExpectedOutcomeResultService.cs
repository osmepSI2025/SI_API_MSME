﻿using SME_API_MSME.Entities;
using SME_API_MSME.Models;
using SME_API_MSME.Repository;
using SME_API_MSME.Services;
public class ExpectedOutcomeResultService
{
    private readonly ExpectedOutcomeResultRepository _repository;
    private readonly ICallAPIService _serviceApi;
    private readonly IApiInformationRepository _repositoryApi;
    private readonly ProjectService _projectService;

    public ExpectedOutcomeResultService(ExpectedOutcomeResultRepository repository,
        ICallAPIService serviceApi, IApiInformationRepository repositoryApi
        , ProjectService projectService)
    {
        _repository = repository;
        _serviceApi = serviceApi;
        _repositoryApi = repositoryApi;
        _projectService = projectService;
    }

    public Task<IEnumerable<MExpectedOutcomeResult>> GetAllExpectedOutcomeResultsAsync()
    {
        return _repository.GetAllAsync();
    }

    //public Task<MExpectedOutcomeResult?> GetExpectedOutcomeResultByIdAsync(int projectId)
    //{
    //    return _repository.GetByIdAsync(projectId);
    //}
    public async Task<ResultExpectedOutcomeResultResponse?> GetExpectedOutcomeResultByIdAsync(long? pProjectCode,string pYear)
    {
        var xrerult = new ResultExpectedOutcomeResultResponse();
        try
        {

            var dataResult = new List<ExpectedOutcomeResultProject>();

            IEnumerable<MExpectedOutcomeResult>? result = null; // Initialize the variable to null

            if (pProjectCode == 0)
            {
                result = await _repository.GetAllAsync();
            }
            else
            {
                var resultPA = await _repository.GetByIdAsync(pProjectCode, pYear);

                if (resultPA == null)
                {
                    var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "expected-outcome-result" });
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
                        xrerult.result =new List<ExpectedOutcomeResultProject>();
                        return xrerult;

                    }

                    var apiResponse = await _serviceApi.GetDataApiAsync_ExpectedOutcomeResult(apiParam, pProjectCode, pYear);
                   if (apiResponse == null || apiResponse.responseCode == 0 || apiResponse.result.Count ==0)
                    {
                        xrerult.responseCode = 200;
                        xrerult.responseMsg = "No data found";
                        xrerult.result =new List<ExpectedOutcomeResultProject>();
                        return xrerult;
                    }
                    else
                    {
                        foreach (var item in apiResponse.result)
                        {
                            var proProduct = new MExpectedOutcomeResult
                            {
                                ProjectCode = item.ProjectCode, // Corrected from 'project.ProjectCode' to 'item.ProjectCode'
                                ProjectName = item.ProjectName, 
                                Year = pYear,// Corrected from 'project.ProjectName' to 'item.ProjectName'
                                TExpectedOutcomeResults = item.Items.Select(i => new TExpectedOutcomeResult
                                {
                                    OrderIndex = i.OrderIndex ?? 0, // Handle nullable OrderIndex
                                    ResultFinalOutcome = i.ResultFinalOutcome, // Corrected property name
                                    SumBudget = i.SumBudget ?? 0, // Handle nullable SumBudget
                                    TExpectedOutcomeResultDetails = i.FinalOutcomeDetail.Select(x => new TExpectedOutcomeResultDetail
                                    {
                                        MonthName = x.MonthName,
                                        Year = x.Year, // Convert Year to string
                                        ResultValue = x.ResultValue ?? 0 // Handle nullable ResultValue
                                    }).ToList()
                                }).ToList()
                            };

                            await AddExpectedOutcomeResultAsync(proProduct);
                        }
                    }

                    result = pProjectCode == 0
          ? await _repository.GetAllAsync()
          : new List<MExpectedOutcomeResult> { await _repository.GetByIdAsync(pProjectCode, pYear) };

                }
                else
                {
                    result = new List<MExpectedOutcomeResult> { resultPA };
                }
            }

            if (result != null && result.Any())
            {
                dataResult.AddRange(result.Select(project => new ExpectedOutcomeResultProject
                {
                    ProjectCode = project.ProjectCode,
                    ProjectName = project.ProjectName,
                    Items = project.TExpectedOutcomeResults.Select(item => new ExpectedOutcomeResultItem
                    {
                        OrderIndex = item.OrderIndex ?? 0, // Handle nullable OrderIndex
                        ResultFinalOutcome = item.ResultFinalOutcome, // Corrected property name
                        SumBudget = item.SumBudget ?? 0, // Handle nullable SumBudget
                        FinalOutcomeDetail = item.TExpectedOutcomeResultDetails.Select(x => new ExpectedOutcomeResultFinalOutcomeDetail
                        {
                            MonthName = x.MonthName,
                            Year = x.Year ??0, // Handle null Year by providing a default value
                            ResultValue = x.ResultValue ?? 0 // Handle nullable ResultValue
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
                xrerult.result = new List<ExpectedOutcomeResultProject>();
            }

            return xrerult;
        }
        catch (Exception ex)
        {
            xrerult.responseCode = 500;
            xrerult.responseMsg = ex.Message;
            xrerult.result = new List<ExpectedOutcomeResultProject>();
            return xrerult;
        }

    }
    public Task AddExpectedOutcomeResultAsync(MExpectedOutcomeResult expectedOutcomeResult)
    {
        return _repository.AddAsync(expectedOutcomeResult);
    }

    public Task UpdateExpectedOutcomeResultAsync(MExpectedOutcomeResult expectedOutcomeResult)
    {
        return _repository.UpdateAsync(expectedOutcomeResult);
    }

    public Task DeleteExpectedOutcomeResultAsync(int projectId)
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


                var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "expected-outcome-result" });
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
                    var apiResponse = await _serviceApi.GetDataApiAsync_ExpectedOutcomeResult(apiParam, item.ProjectCode,year.ToString());
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

                            var proProduct = new MExpectedOutcomeResult
                            {
                                ProjectId = resultPA?.ProjectId ?? 0, // Assuming ProjectId is available in the item
                                ProjectCode = item.ProjectCode, // Corrected from 'project.ProjectCode' to 'item.ProjectCode'
                                ProjectName = item.ProjectName, 
                                Year = year.ToString(),// Corrected from 'project.ProjectName' to 'item.ProjectName'
                                TExpectedOutcomeResults = Subitem.Items.Select(i => new TExpectedOutcomeResult
                                {
                                    OrderIndex = i.OrderIndex ?? 0, // Handle nullable OrderIndex
                                    ResultFinalOutcome = i.ResultFinalOutcome, // Corrected property name
                                    SumBudget = i.SumBudget ?? 0, // Handle nullable SumBudget
                                    TExpectedOutcomeResultDetails = i.FinalOutcomeDetail.Select(x => new TExpectedOutcomeResultDetail
                                    {
                                        MonthName = x.MonthName,
                                        Year = x.Year, // Convert Year to string
                                        ResultValue = x.ResultValue ?? 0 // Handle nullable ResultValue
                                    }).ToList()
                                }).ToList()
                            };

                            if (resultPA == null)
                            {
                                await AddExpectedOutcomeResultAsync(proProduct);
                            }
                            else
                            {
                                await UpdateExpectedOutcomeResultAsync(proProduct);
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
