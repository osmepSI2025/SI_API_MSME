using SME_API_MSME.Entities;
using SME_API_MSME.Models;
using SME_API_MSME.Repository;
using SME_API_MSME.Services;
public class ProductResultService
{
    private readonly ProductResultRepository _repository;
    private readonly ICallAPIService _serviceApi;
    private readonly IApiInformationRepository _repositoryApi;
    private readonly ProjectService _projectService;
    public ProductResultService(ProductResultRepository repository, ICallAPIService serviceApi
        , IApiInformationRepository repositoryApi, ProjectService projectService)

    {
        _repository = repository;
        _serviceApi = serviceApi;
        _repositoryApi = repositoryApi;
        _projectService = projectService;
    }

    public Task<IEnumerable<MProductResult>> GetAllProductResultsAsync()
    {
        return _repository.GetAllAsync();
    }

    //public Task<MProductResult?> GetProductResultByIdAsync(int projectId)
    //{
    //    return _repository.GetByIdAsync(projectId);
    //}
    public async Task<ResultProductResultResponse?> GetProductResultByIdAsync(long? pProjectCode)
    {
        var xrerult = new ResultProductResultResponse();
        try
        {

            var dataResult = new List<ProductResultProject>();

            IEnumerable<MProductResult>? result = null; // Initialize the variable to null

            if (pProjectCode == 0)
            {
                result = await _repository.GetAllAsync();
            }
            else
            {
                var resultPA = await _repository.GetByIdAsync(pProjectCode);

                if (resultPA == null)
                {
                    var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "product-result" });
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
                        xrerult.Result = new List<ProductResultProject>();
                        return xrerult;

                    }

                    var apiResponse = await _serviceApi.GetDataApiAsync_ProductResult(apiParam, pProjectCode);
                   if (apiResponse == null || apiResponse.ResponseCode == 0 || apiResponse.Result.Count ==0)
                    {
                        xrerult.ResponseCode = 200;
                        xrerult.ResponseMsg = "No data found";
                        xrerult.Result =new List<ProductResultProject>();
                        return xrerult;
                    }
                    else
                    {
                        foreach (var item in apiResponse.Result)
                        {
                            var proProduct = new MProductResult
                            {
                                ProjectCode = item.ProjectCode, // Corrected from 'project.ProjectCode' to 'item.ProjectCode'
                                ProjectName = item.ProjectName, // Corrected from 'project.ProjectName' to 'item.ProjectName'
                                TProductResultOutputs = item.Items.Select(i => new TProductResultOutput
                                {
                                    OrderIndex = i.OrderIndex ?? 0, // Handle nullable OrderIndex
                                    OutputOutcomeName = i.OutputOutComeName, // Corrected property name
                                    YieldTypeName = i.YieldTypeName, // Corrected property name
                                    UnitName = i.UnitName, // Corrected property name
                                    TProductResultOutputDetails = i.ProductResult.Select(x => new TProductResultOutputDetail
                                    {
                                        MonthName = x.MonthName,
                                        Year = x.Year.ToString(), // Corrected to match the type
                                        ResultOfYear = x.ResultOfYear, // Corrected property name
                                        ResultOffEffect = x.ResultOffEffect, // Corrected property name
                                        Detail = x.Detail,
                                        Problem = x.Problem,
                                        Solution = x.Solution
                                    }).ToList()
                                }).ToList()
                            };

                            await AddProductResultAsync(proProduct);
                        }

                    }

                    result = pProjectCode == 0
          ? await _repository.GetAllAsync()
          : new List<MProductResult> { await _repository.GetByIdAsync(pProjectCode) };

                }
                else
                {
                    result = new List<MProductResult> { resultPA };
                }
            }

            if (result != null && result.Any())
            {
                dataResult.AddRange(result.Select(project => new ProductResultProject
                {
                    ProjectCode = project.ProjectCode,
                    ProjectName = project.ProjectName,
                    Items = project.TProductResultOutputs.Select(item => new ProductResultItem
                    {
                        OrderIndex = item.OrderIndex ?? 0, // Handle nullable OrderIndex
                        OutputOutComeName = item.OutputOutcomeName, // Corrected property name
                        YieldTypeName = item.YieldTypeName, // Corrected property name
                        Target = item.Target ?? 0, // Corrected property name
                        UnitName = item.UnitName, // Corrected property name
                        ProductResult = item.TProductResultOutputDetails.Select(x => new ProductResultDetail
                        {
                            MonthName = x.MonthName,
                            Year = x.Year != null ? int.Parse(x.Year) : 0, // Handle null Year by providing a default value
                            ResultOfYear = (decimal?)x.ResultOfYear ?? 0, // Corrected property name and type
                            ResultOffEffect = (decimal?)x.ResultOffEffect ?? 0, // Corrected property name and type
                            Detail = x.Detail,
                            Problem = x.Problem,
                            Solution = x.Solution
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
                xrerult.Result = new List<ProductResultProject>() ;
            }

            return xrerult;
        }
        catch (Exception ex)
        {
            xrerult.ResponseCode = 500;
            xrerult.ResponseMsg = ex.Message;
            xrerult.Result = new List<ProductResultProject>();
            return xrerult;
        }

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


                var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "product-result" });
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
                    var apiResponse = await _serviceApi.GetDataApiAsync_ProductResult(apiParam, item.ProjectCode);
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

                            var proProduct = new MProductResult
                            {
                                ProjectId = resultPA?.ProjectId ?? 0, // Assuming ProjectId is available in the item
                                ProjectCode = item.ProjectCode, // Corrected from 'project.ProjectCode' to 'item.ProjectCode'
                                ProjectName = item.ProjectName, // Corrected from 'project.ProjectName' to 'item.ProjectName'
                                TProductResultOutputs = Subitem.Items.Select(i => new TProductResultOutput
                                {
                                    OrderIndex = i.OrderIndex ?? 0, // Handle nullable OrderIndex
                                    OutputOutcomeName = i.OutputOutComeName, // Corrected property name
                                    YieldTypeName = i.YieldTypeName, // Corrected property name
                                    UnitName = i.UnitName, // Corrected property name
                                    TProductResultOutputDetails = i.ProductResult.Select(x => new TProductResultOutputDetail
                                    {
                                        MonthName = x.MonthName,
                                        Year = x.Year.ToString(), // Corrected to match the type
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
                                await AddProductResultAsync(proProduct);
                            }
                            else
                            {
                                await UpdateProductResultAsync(proProduct);
                            }
                        }

                    }

                }


            }

            return "Batch end of day process completed successfully.";
        }

        return "Success";
    }



    public Task AddProductResultAsync(MProductResult productResult)
    {
        return _repository.AddAsync(productResult);
    }

    public Task UpdateProductResultAsync(MProductResult productResult)
    {
        return _repository.UpdateAsync(productResult);
    }

    public Task DeleteProductResultAsync(int projectId)
    {
        return _repository.DeleteAsync(projectId);
    }
}
