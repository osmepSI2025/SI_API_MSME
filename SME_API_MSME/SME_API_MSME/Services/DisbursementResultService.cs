using SME_API_MSME.Entities;
using SME_API_MSME.Models;
using SME_API_MSME.Repository;
using SME_API_MSME.Services;
public class DisbursementResultService
{
    private readonly DisbursementResultRepository _repository;
    private readonly ICallAPIService _serviceApi;
    private readonly IApiInformationRepository _repositoryApi;
    private readonly ProjectService _projectService;
    public DisbursementResultService(DisbursementResultRepository repository, ICallAPIService serviceApi,
        IApiInformationRepository repositoryApi,  ProjectService projectService)

    {
        _repository = repository;
        _serviceApi = serviceApi;
        _repositoryApi = repositoryApi;
        _projectService = projectService;
    }

    public Task<IEnumerable<MDisbursementResult>> GetAllDisbursementResultsAsync()
    {
        return _repository.GetAllAsync();
    }

    //public Task<MDisbursementResult?> GetDisbursementResultByIdAsync(int projectId)
    //{
    //    return _repository.GetByIdAsync(projectId);
    //}
    public async Task<ResultDisbursementResultResponse?> GetDisbursementResultByIdAsync(long? pProjectCode,string pYear)
    {
        var xrerult = new ResultDisbursementResultResponse();
        try
        {

            var dataResult = new List<DisbursementResultProject>();

            IEnumerable<MDisbursementResult>? result = null; // Initialize the variable to null

            if (pProjectCode == 0)
            {
                result = await _repository.GetAllAsync();
            }
            else
            {
                var resultPA = await _repository.GetByIdAsync(pProjectCode, pYear);

                if (resultPA == null)
                {
                    var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "disbursement-result" });
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
						xrerult.result = new List<DisbursementResultProject>();

					}

					var apiResponse = await _serviceApi.GetDataApiAsync_DisbursementResult(apiParam, pProjectCode, pYear);
					 if (apiResponse == null || apiResponse.responseCode == 0 || apiResponse.result.Count ==0)
					{
						xrerult.responseCode = 200;
						xrerult.responseMsg = "No data found";
						xrerult.result = new List<DisbursementResultProject>();
						return xrerult;
					}

					else
					{
                        foreach (var item in apiResponse.result)
                        {
                            var proProduct = new MDisbursementResult
                            {
                                ProjectCode = item.ProjectCode, // Corrected from 'project.ProjectCode' to 'item.ProjectCode'
                                ProjectName = item.ProjectName,
                                Year = pYear,// Corrected from 'project.ProjectName' to 'item.ProjectName'
                                TDisbursementResults = item.Items.Select(i => new TDisbursementResult
                                {
                                    OrderIndex = i.OrderIndex ?? 0, // Handle nullable OrderIndex
                                    ItemActivityDetail = i.ItemActivityDetail,
                                    ActivityBudget = i.ActivityBudget ?? 0, // Corrected property name
                                    ExpenseTypeName = i.ExpenseTypeName, // Corrected property name
                                    SumEffectValue = i.SumEffectValue ?? 0, // Handle nullable SumEffectValue
                                    TDisbursementResultDetails = i.ActionResultDetail.Select(x => new TDisbursementResultDetail
                                    {
                                        MonthName = x.MonthName,
                                        Year = x.Year, // Corrected to match the type
                                        TempValue = x.TempValue ?? 0, // Handle nullable TempValue
                                        EffectValue = x.EffectValue ?? 0 // Handle nullable EffectValue
                                    }).ToList()
                                }).ToList()
                            };

                            await AddDisbursementResultAsync(proProduct);
                        }
                    }

                    result = pProjectCode == 0
          ? await _repository.GetAllAsync()
          : new List<MDisbursementResult> { await _repository.GetByIdAsync(pProjectCode, pYear) };

                }
                else
                {
                    result = new List<MDisbursementResult> { resultPA };
                }
            }

            if (result != null && result.Any())
            {
                dataResult.AddRange(result.Select(project => new DisbursementResultProject
                {
                    ProjectCode = project.ProjectCode,
                    ProjectName = project.ProjectName,
                    Items = project.TDisbursementResults.Select(item => new DisbursementResultItem
                    {
                        OrderIndex = item.OrderIndex ?? 0, // Handle nullable OrderIndex
                        ItemActivityDetail = item.ItemActivityDetail,
                        ActivityBudget = item.ActivityBudget ?? 0, // Corrected property name
                        ExpenseTypeName = item.ExpenseTypeName, // Corrected property name
                        SumEffectValue = item.SumEffectValue ?? 0, // Handle nullable SumEffectValue
                        ActionResultDetail = item.TDisbursementResultDetails.Select(x => new DisbursementActionResultDetail
                        {
                            MonthName = x.MonthName,
                            Year = x.Year != null ? x.Year : 0, // Handle null Year by providing a default value
                            TempValue = x.TempValue ?? 0, // Handle nullable TempValue
                            EffectValue = x.EffectValue ?? 0 // Handle nullable EffectValue
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
                xrerult.result = new List<DisbursementResultProject>() ;
            }

            return xrerult;
        }
        catch (Exception ex)
        {
            xrerult.responseCode = 500;
            xrerult.responseMsg = ex.Message;
			xrerult.result = new List<DisbursementResultProject>();
            return xrerult;
        }

    }

    public Task AddDisbursementResultAsync(MDisbursementResult disbursementResult)
    {
        return _repository.AddAsync(disbursementResult);
    }

    public Task UpdateDisbursementResultAsync(MDisbursementResult disbursementResult)
    {
        return _repository.UpdateAsync(disbursementResult);
    }

    public Task DeleteDisbursementResultAsync(int projectId)
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


                var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "disbursement-result" });
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
                    var apiResponse = await _serviceApi.GetDataApiAsync_DisbursementResult(apiParam, item.ProjectCode,year.ToString());
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
                                var proProduct = new MDisbursementResult
                                {
                                    ProjectCode = Subitem.ProjectCode, // Corrected from 'project.ProjectCode' to 'item.ProjectCode'
                                    ProjectName = Subitem.ProjectName,
                                    Year = year.ToString(),// Corrected from 'project.ProjectName' to 'item.ProjectName'
                                    TDisbursementResults = Subitem.Items.Select(i => new TDisbursementResult
                                    {
                                        OrderIndex = i.OrderIndex ?? 0, // Handle nullable OrderIndex
                                        ItemActivityDetail = i.ItemActivityDetail,
                                        ActivityBudget = i.ActivityBudget ?? 0, // Corrected property name
                                        ExpenseTypeName = i.ExpenseTypeName, // Corrected property name
                                        SumEffectValue = i.SumEffectValue ?? 0, // Handle nullable SumEffectValue
                                        TDisbursementResultDetails = i.ActionResultDetail.Select(x => new TDisbursementResultDetail
                                        {
                                            MonthName = x.MonthName,
                                            Year = x.Year, // Corrected to match the type
                                            TempValue = x.TempValue ?? 0, // Handle nullable TempValue
                                            EffectValue = x.EffectValue ?? 0 // Handle nullable EffectValue
                                        }).ToList()
                                    }).ToList()
                                };

                                await AddDisbursementResultAsync(proProduct);
                            }
                            else
                            {
                                // Update existing MDisbursementResult
                                resultPA.ProjectName = Subitem.ProjectName;

                                // Remove orphaned TDisbursementResults
                                var incomingActivityIds = Subitem.Items.Select(i => i.OrderIndex ?? 0).ToHashSet();
                                var toRemove = resultPA.TDisbursementResults
                                    .Where(x => !incomingActivityIds.Contains(x.OrderIndex ?? 0))
                                    .ToList();
                                foreach (var child in toRemove)
                                {
                                    resultPA.TDisbursementResults.Remove(child);
                                }

                                // Update or add TDisbursementResults
                                foreach (var i in Subitem.Items)
                                {
                                    var existingActivity = resultPA.TDisbursementResults
                                        .FirstOrDefault(x => x.OrderIndex == (i.OrderIndex ?? 0));
                                    if (existingActivity != null)
                                    {
                                        existingActivity.ItemActivityDetail = i.ItemActivityDetail;
                                        existingActivity.ActivityBudget = i.ActivityBudget ?? 0;
                                        existingActivity.ExpenseTypeName = i.ExpenseTypeName;
                                        existingActivity.SumEffectValue = i.SumEffectValue ?? 0;

                                        // Remove orphaned details
                                        var incomingDetailKeys = i.ActionResultDetail
                                            .Select(x => (x.MonthName, x.Year)).ToHashSet();
                                        var toRemoveDetails = existingActivity.TDisbursementResultDetails
                                            .Where(x => !incomingDetailKeys.Contains((x.MonthName, x.Year)))
                                            .ToList();
                                        foreach (var detail in toRemoveDetails)
                                        {
                                            existingActivity.TDisbursementResultDetails.Remove(detail);
                                        }

                                        // Update or add details
                                        foreach (var x in i.ActionResultDetail)
                                        {
                                            var existingDetail = existingActivity.TDisbursementResultDetails
                                                .FirstOrDefault(d => d.MonthName == x.MonthName && d.Year == x.Year);
                                            if (existingDetail != null)
                                            {
                                                existingDetail.TempValue = x.TempValue ?? 0;
                                                existingDetail.EffectValue = x.EffectValue ?? 0;
                                            }
                                            else
                                            {
                                                existingActivity.TDisbursementResultDetails.Add(new TDisbursementResultDetail
                                                {
                                                    MonthName = x.MonthName,
                                                    Year = x.Year,
                                                    TempValue = x.TempValue ?? 0,
                                                    EffectValue = x.EffectValue ?? 0
                                                });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // Add new TDisbursementResult
                                        var newActivity = new TDisbursementResult
                                        {
                                            OrderIndex = i.OrderIndex ?? 0,
                                            ItemActivityDetail = i.ItemActivityDetail,
                                            ActivityBudget = i.ActivityBudget ?? 0,
                                            ExpenseTypeName = i.ExpenseTypeName,
                                            SumEffectValue = i.SumEffectValue ?? 0,
                                            TDisbursementResultDetails = i.ActionResultDetail.Select(x => new TDisbursementResultDetail
                                            {
                                                MonthName = x.MonthName,
                                                Year = x.Year,
                                                TempValue = x.TempValue ?? 0,
                                                EffectValue = x.EffectValue ?? 0
                                            }).ToList()
                                        };
                                        resultPA.TDisbursementResults.Add(newActivity);
                                    }
                                }

                                await UpdateDisbursementResultAsync(resultPA);
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
