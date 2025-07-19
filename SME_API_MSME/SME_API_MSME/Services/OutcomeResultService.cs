using SME_API_MSME.Entities;
using SME_API_MSME.Models;
using SME_API_MSME.Repository;
using SME_API_MSME.Services;
public class OutcomeResultService
{
    private readonly OutcomeResultRepository _repository;
    private readonly ICallAPIService _serviceApi;
    private readonly IApiInformationRepository _repositoryApi;
    private readonly ProjectService _projectService;
    public OutcomeResultService(OutcomeResultRepository repository, ICallAPIService serviceApi,
        IApiInformationRepository repositoryApi, ProjectService projectService)

    {
        _repository = repository;
        _serviceApi = serviceApi;
        _repositoryApi = repositoryApi;
        _projectService = projectService;
    }

    public Task<IEnumerable<MOutcomeResult>> GetAllOutcomeResultsAsync()
    {
        return _repository.GetAllAsync();
    }

    //public Task<MOutcomeResult?> GetOutcomeResultByIdAsync(int projectId)
    //{
    //    return _repository.GetByIdAsync(projectId);
    //}
    public async Task<ResultOutcomeResultResponse?> GetOutcomeResultByIdAsync(long? pProjectCode,string pYear)
    {
        var xrerult = new ResultOutcomeResultResponse();
        try
        {

            var dataResult = new List<OutcomeResultProject>();

            IEnumerable<MOutcomeResult>? result = null; // Initialize the variable to null

            if (pProjectCode == 0)
            {
                result = await _repository.GetAllAsync();
            }
            else
            {
                var resultPA = await _repository.GetByIdAsync(pProjectCode, pYear);

                if (resultPA == null)
                {
                    var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "outcome-result" });
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
                        xrerult.result = new List<OutcomeResultProject>();
                        return xrerult;

                    }

                    var apiResponse = await _serviceApi.GetDataApiAsync_OutcomeResult(apiParam, pProjectCode, pYear);
                   if (apiResponse == null || apiResponse.responseCode == 0 || apiResponse.result.Count ==0)
                    {
                        xrerult.responseCode = 200;
                        xrerult.responseMsg = "No data found";
                        xrerult.result = new List<OutcomeResultProject>();
                        return xrerult;
                    }
                    else
                    {
                        foreach (var item in apiResponse.result)
                        {
                            var proProduct = new MOutcomeResult
                            {
                                ProjectCode = item.ProjectCode, // Corrected from 'project.ProjectCode' to 'item.ProjectCode'
                                ProjectName = item.ProjectName,
                                Year = pYear,// Corrected from 'project.ProjectName' to 'item.ProjectName'
                                TOutcomeResults = item.Items.Select(i => new TOutcomeResult
                                {
                                    OrderIndex = i.OrderIndex ?? 0, // Handle nullable OrderIndex
                                    OutputOutcomeName = i.OutputOutComeName, // Corrected property name
                                    YieldTypeName = i.YieldTypeName, // Corrected property name
                                    UnitName = i.UnitName, // Corrected property name
                                    Target = i.Target ?? 0, // Corrected property name
                                    TOutcomeResultDetails = i.OutcomeResult.Select(x => new TOutcomeResultDetail
                                    {
                                        MonthName = x.MonthName,
                                        Year = x.Year, // Corrected to match the type
                                        ResultOfYear = x.ResultOfYear, // Corrected property name
                                        ResultOffEffect = x.ResultOffEffect, // Corrected property name
                                        Detail = x.Detail,
                                        Problem = x.Problem,
                                        Solution = x.Solution
                                    }).ToList()
                                }).ToList()
                            };

                            await AddOutcomeResultAsync(proProduct);
                        }




                    }

                    result = pProjectCode == 0
          ? await _repository.GetAllAsync()
          : new List<MOutcomeResult> { await _repository.GetByIdAsync(pProjectCode, pYear) };

                }
                else
                {
                    result = new List<MOutcomeResult> { resultPA };
                }
            }

            if (result != null && result.Any())
            {
                dataResult.AddRange(result.Select(project => new OutcomeResultProject
                {
                    ProjectCode = project.ProjectCode,
                    ProjectName = project.ProjectName,
                    Items = project.TOutcomeResults.Select(item => new OutcomeResultItem
                    {
                        OrderIndex = item.OrderIndex ?? 0, // Handle nullable OrderIndex
                        OutputOutComeName = item.OutputOutcomeName, // Corrected property name
                        YieldTypeName = item.YieldTypeName, // Corrected property name
                        Target = item.Target ?? 0, // Corrected property name
                        UnitName = item.UnitName, // Corrected property name
                        
                        OutcomeResult = item.TOutcomeResultDetails.Select(x => new OutcomeResultDetail
                        {
                            MonthName = x.MonthName,
                            Year = x.Year ?? 0, // Handle null Year by providing a default value
                            ResultOfYear = x.ResultOfYear ?? 0, // Corrected property name and type
                            ResultOffEffect = x.ResultOffEffect ?? 0, // Corrected property name and type
                            Detail = x.Detail,
                            Problem = x.Problem,
                            Solution = x.Solution
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
                xrerult.result = new List<OutcomeResultProject>();
            }

            return xrerult;
        }
        catch (Exception ex)
        {
            xrerult.responseCode = 500;
            xrerult.responseMsg = ex.Message;
            xrerult.result = new List<OutcomeResultProject>();
            return xrerult;
        }

    }
    public Task AddOutcomeResultAsync(MOutcomeResult outcomeResult)
    {
        return _repository.AddAsync(outcomeResult);
    }

    public Task UpdateOutcomeResultAsync(MOutcomeResult outcomeResult)
    {
        return _repository.UpdateAsync(outcomeResult);
    }

    public Task DeleteOutcomeResultAsync(int projectId)
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


                var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "outcome-result" });
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
                    var apiResponse = await _serviceApi.GetDataApiAsync_OutcomeResult(apiParam, item.ProjectCode, year.ToString());
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

                            var proProduct = new MOutcomeResult
                            {
                                ProjectId = resultPA?.ProjectId ?? 0, // Assuming ProjectId is available in the item
                                ProjectCode = item.ProjectCode, // Corrected from 'project.ProjectCode' to 'item.ProjectCode'
                                ProjectName = item.ProjectName, 
                                Year = year.ToString(),// Corrected from 'project.ProjectName' to 'item.ProjectName'
                                TOutcomeResults = Subitem.Items.Select(i => new TOutcomeResult
                                {
                                    OrderIndex = i.OrderIndex ?? 0, // Handle nullable OrderIndex
                                    OutputOutcomeName = i.OutputOutComeName, // Corrected property name
                                    YieldTypeName = i.YieldTypeName, // Corrected property name
                                    UnitName = i.UnitName, // Corrected property name
                                    Target = i.Target ?? 0, // Corrected property name
                                    TOutcomeResultDetails = i.OutcomeResult.Select(x => new TOutcomeResultDetail
                                    {
                                        MonthName = x.MonthName,
                                        Year = x.Year, // Corrected to match the type
                                        ResultOfYear = x.ResultOfYear, // Corrected property name
                                        ResultOffEffect = x.ResultOffEffect, // Corrected property name
                                        Detail = x.Detail,
                                        Problem = x.Problem,
                                        Solution = x.Solution
                                    }).ToList()
                                }).ToList()
                            };

                            if (resultPA == null)
                            {
                                await AddOutcomeResultAsync(proProduct);
                            }
                            else
                            {
                                await UpdateOutcomeResultAsync(proProduct);
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
