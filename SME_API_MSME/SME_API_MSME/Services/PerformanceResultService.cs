using SME_API_MSME.Entities;
using SME_API_MSME.Models;
using SME_API_MSME.Repository;
using SME_API_MSME.Services;
public class PerformanceResultService
{
    private readonly PerformanceResultRepository _repository;
    private readonly ICallAPIService _serviceApi;
    private readonly IApiInformationRepository _repositoryApi;
    private readonly ProjectService _projectService;
    public PerformanceResultService(PerformanceResultRepository repository, ICallAPIService serviceApi,
        IApiInformationRepository repositoryApi
        , ProjectService projectService)

    {
        _repository = repository; _serviceApi = serviceApi;
        _repositoryApi = repositoryApi;
    _projectService = projectService;
    }

    public Task<IEnumerable<MPerformanceResult>> GetAllPerformanceResultsAsync()
    {
        return _repository.GetAllAsync();
    }

    //public Task<MPerformanceResult?> GetPerformanceResultByIdAsync(int projectId)
    //{
    //    return _repository.GetByIdAsync(projectId);
    //}
    public async Task<ResultPerformanceResultResponse?> GetPerformanceResultByIdAsync(long? pProjectCode,string pYear)
    {
        var xrerult = new ResultPerformanceResultResponse();
        try
        {

            var dataResult = new List<PerformanceResultProject>();

            IEnumerable<MPerformanceResult>? result = null; // Initialize the variable to null

            if (pProjectCode == 0)
            {
                result = await _repository.GetAllAsync();
            }
            else
            {
                var resultPA = await _repository.GetByIdAsync(pProjectCode, pYear);

                if (resultPA == null)
                {
                    var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "performance-result" });
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
                        xrerult.result = new List<PerformanceResultProject>();
                        return xrerult;

                    }

                    var apiResponse = await _serviceApi.GetDataApiAsync_PerformanceResult(apiParam, pProjectCode, pYear);
                   if (apiResponse == null || apiResponse.responseCode == 0 || apiResponse.result.Count ==0)
                    {
                        xrerult.responseCode = 200;
                        xrerult.responseMsg = "No data found";
                        xrerult.result =new List<PerformanceResultProject>();
                        return xrerult;
                    }

                    else
                    {
                        foreach (var item in apiResponse.result)
                        {
                            var proProduct = new MPerformanceResult
                            {
                                ProjectCode = item.ProjectCode, // Corrected from 'project.ProjectCode' to 'item.ProjectCode'
                                ProjectName = item.ProjectName,
                                Year = pYear,// Corrected from 'project.ProjectName' to 'item.ProjectName'
                                TPerformanceResults = item.Items.Select(i => new TPerformanceResult
                                {
                                    OrderIndex = i.OrderIndex ?? 0, // Handle nullable OrderIndex
                                    ItemActivityDetail = i.ItemActivityDetail,
                                    ExpenseTypeName = i.ExpenseTypeName, // Corrected property name
                                    ActivityWeight = i.ActivityWeight ?? 0, // Handle nullable ActivityWeight
                                    Target = i.Target ?? 0, // Handle nullable Target
                                    UnitName = i.UnitName,
                                    SumEffectValue = i.SumEffectValue ?? 0, // Handle nullable SumEffectValue
                                    TPerformanceResultDetails = i.ActionResultDetail.Select(x => new TPerformanceResultDetail
                                    {
                                        MonthName = x.MonthName,
                                        Year = x.Year, // Convert Year to string
                                        EffectValue = x.EffectValue ?? 0, // Handle nullable EffectValue
                                        TempValue = x.TempValue ?? 0 // Handle nullable TempValue
                                    }).ToList()
                                }).ToList()
                            };

                            await AddPerformanceResultAsync(proProduct);
                        }



                    }

                    result = pProjectCode == 0
          ? await _repository.GetAllAsync()
          : new List<MPerformanceResult> { await _repository.GetByIdAsync(pProjectCode,pYear) };

                }
                else
                {
                    result = new List<MPerformanceResult> { resultPA };
                }
            }

            if (result != null && result.Any())
            {
                dataResult.AddRange(result.Select(project => new PerformanceResultProject
                {
                    ProjectCode = project.ProjectCode,
                    ProjectName = project.ProjectName,
                    
                    Items = project.TPerformanceResults.Select(item => new PerformanceResultItem
                    {
                        OrderIndex = item.OrderIndex ?? 0, // Handle nullable OrderIndex
                        ItemActivityDetail = item.ItemActivityDetail,
                        ExpenseTypeName = item.ExpenseTypeName, // Corrected property name
                        ActivityWeight = item.ActivityWeight ?? 0, // Handle nullable ActivityWeight
                        Target = item.Target ?? 0, // Handle nullable Target
                        UnitName = item.UnitName,
                        SumEffectValue = item.SumEffectValue, // Handle nullable or invalid SumEffectValue
                        ActionResultDetail = item.TPerformanceResultDetails.Select(x => new PerformanceActionResultDetail
                        {
                            MonthName = x.MonthName,
                            Year = x.Year ?? 0, // Handle null Year by providing a default value
                            EffectValue = x.EffectValue ?? 0, // Handle nullable EffectValue
                            TempValue = (int?)x.TempValue ?? 0 // Handle nullable TempValue
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
                xrerult.result = null;
            }

            return xrerult;
        }
        catch (Exception ex)
        {
            xrerult.responseCode = 500;
            xrerult.responseMsg =ex.Message;
            xrerult.result = new List<PerformanceResultProject>();
            return xrerult;
        }

    }

    public Task AddPerformanceResultAsync(MPerformanceResult performanceResult)
    {
        return _repository.AddAsync(performanceResult);
    }

    public Task UpdatePerformanceResultAsync(MPerformanceResult performanceResult)
    {
        return _repository.UpdateAsync(performanceResult);
    }

    public Task DeletePerformanceResultAsync(int projectId)
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


                var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "performance-result" });
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
                    var apiResponse = await _serviceApi.GetDataApiAsync_PerformanceResult(apiParam, item.ProjectCode,year.ToString());
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

                            var proProduct = new MPerformanceResult
                            {
                                ProjectId = resultPA?.ProjectId ?? 0, // Assuming ProjectId is available in the item
                                ProjectCode = item.ProjectCode, // Corrected from 'project.ProjectCode' to 'item.ProjectCode'
                                ProjectName = item.ProjectName,
                                Year = year.ToString(), // Assuming Year is a string, adjust if necessary
                                // Corrected from 'project.ProjectName' to 'item.ProjectName'
                                TPerformanceResults = Subitem.Items.Select(i => new TPerformanceResult
                                {
                                    OrderIndex = i.OrderIndex ?? 0, // Handle nullable OrderIndex
                                    ItemActivityDetail = i.ItemActivityDetail,
                                    ExpenseTypeName = i.ExpenseTypeName, // Corrected property name
                                    ActivityWeight = i.ActivityWeight ?? 0, // Handle nullable ActivityWeight
                                    Target = i.Target ?? 0, // Handle nullable Target
                                    UnitName = i.UnitName,
                                    SumEffectValue = i.SumEffectValue ?? 0, // Handle nullable SumEffectValue
                                    TPerformanceResultDetails = i.ActionResultDetail.Select(x => new TPerformanceResultDetail
                                    {
                                        MonthName = x.MonthName,
                                        Year = x.Year??0, // Convert Year to string
                                        EffectValue = x.EffectValue ?? 0, // Handle nullable EffectValue
                                        TempValue = x.TempValue ?? 0 // Handle nullable TempValue
                                    }).ToList()
                                }).ToList()
                            };

                            if (resultPA == null)
                            {
                                await AddPerformanceResultAsync(proProduct);
                            }
                            else
                            {
                                await UpdatePerformanceResultAsync(proProduct);
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
