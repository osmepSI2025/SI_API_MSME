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

    // แก้ไขส่วน batchEndOfday - ลบ logic manual update ที่ซับซ้อน
    public async Task<string> batchEndOfday()
    {
        int currentYear = DateTime.Now.Year;
        int currentYearBE = currentYear < 2500 ? currentYear + 543 : currentYear;
        int currentYearTo = currentYearBE + 1;

        for (int year = currentYearBE - 3; year <= currentYearTo; year++)
        {
           // string year ="2566";
            var Listprojects = await _projectService.GetProjectByIdAsync(year.ToString());
            if (Listprojects == null || Listprojects.result.Count == 0)
            {
              continue;
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
                    var apiResponse = await _serviceApi.GetDataApiAsync_DisbursementResult(apiParam, item.ProjectCode, year.ToString());
                    if (apiResponse == null || apiResponse.responseCode == 0 || apiResponse.result.Count == 0)
                    {
                        continue;
                    }
                    else
                    {
                        foreach (var Subitem in apiResponse.result)
                        {
                            // Check if existing record
                            var resultPA = await _repository.GetByIdAsync(Subitem.ProjectCode, year.ToString());

                            var proProduct = new MDisbursementResult
                            {
                                ProjectId = resultPA?.ProjectId ?? 0,
                                ProjectCode = Subitem.ProjectCode,
                                ProjectName = Subitem.ProjectName,
                                Year = year.ToString(),
                                TDisbursementResults = Subitem.Items.Select(i => new TDisbursementResult
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
                                }).ToList()
                            };

                            if (resultPA == null)
                            {
                                await AddDisbursementResultAsync(proProduct);
                            }
                            else
                            {
                                await UpdateDisbursementResultAsync(proProduct);
                            }
                        }
                    }
                }
            }
        }

        return "Success";
    }
}
