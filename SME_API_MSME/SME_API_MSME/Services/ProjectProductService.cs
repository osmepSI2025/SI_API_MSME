using Microsoft.AspNetCore.Http;
using SME_API_MSME.Entities;
using SME_API_MSME.Models;
using SME_API_MSME.Repository;
using SME_API_MSME.Services;
using System.Collections.Generic;
public class ProjectProductService
{
    private readonly ProjectProductRepository _repository;
    private readonly ICallAPIService _serviceApi;
    private readonly IApiInformationRepository _repositoryApi;
    private readonly ProjectService _projectService;
    public ProjectProductService(ProjectProductRepository repository, ICallAPIService serviceApi
        , IApiInformationRepository repositoryApi, ProjectService projectService)

    {
        _repository = repository;
        _serviceApi = serviceApi;
        _repositoryApi = repositoryApi;
        _projectService = projectService;
    }

    public Task<IEnumerable<MProjectsProduct>> GetAllProjectProductsAsync()
    {
        return _repository.GetAllAsync();
    }

    //public Task<MProjectsProduct?> GetProjectProductByIdAsync(int projectId)
    //{
    //    return _repository.GetByIdAsync(projectId);
    //}
    public async Task<ResultProjectProductResponse?> GetProjectProductByIdAsync(long? pProjectCode)
    {
        var xrerult = new ResultProjectProductResponse();
        try
        {
           
            var dataResult = new List<ProjectProductResult>();

            IEnumerable<MProjectsProduct>? result = null; // Initialize the variable to null

            if (pProjectCode == 0)
            {
                result = await _repository.GetAllAsync();
            }
            else
            {
                var resultPA = await _repository.GetByIdAsync(pProjectCode);

                if (resultPA == null)
                {
                    var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "project-product" });
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
                        xrerult.Result = new List<ProjectProductResult>();
                        return xrerult;

                    }

                    var apiResponse = await _serviceApi.GetDataApiAsync_ProjectProduct(apiParam, pProjectCode);
                  if (apiResponse == null || apiResponse.ResponseCode == 0 || apiResponse.Result.Count ==0)
                    {
                        xrerult.ResponseCode = 200;
                        xrerult.ResponseMsg = "No data found";
                        xrerult.Result =new List<ProjectProductResult>();
                        return xrerult;

                    }
                    else
                    {
                        foreach (var item in apiResponse.Result)
                        {
                            var proProduct = new MProjectsProduct
                            {
                                ProjectCode = item.ProjectCode,
                                ProjectName = item.ProjectName,
                                TProjectsProducts = item.Items.Select(i => new TProjectsProduct
                                {
                                    OrderIndex = i.OrderIndex,
                                    OutputOutcomeName = i.OutputOutComeName,
                                    YieldTypeName = i.YieldTypeName,
                                    MeasureResult = i.MeasureResult,
                                    Target = i.Target,
                                    UnitName = i.CountUnitName,
                                }).ToList()

                            };
                            await AddProjectProductAsync(proProduct);
                        }
                    }

                    result = pProjectCode == 0
                        ? await _repository.GetAllAsync()
                        : new List<MProjectsProduct> { await _repository.GetByIdAsync(pProjectCode) };
                }
                else
                {
                    result = new List<MProjectsProduct> { resultPA };
                }
            }

            if (result != null && result.Any())
            {
                dataResult.AddRange(result.Select(project => new ProjectProductResult
                {
                    ProjectCode = project.ProjectCode,
                    ProjectName = project.ProjectName,
                    Items = project.TProjectsProducts.Select(item => new ProjectProductItem
                    {
                        OrderIndex = item.OrderIndex,
                        OutputOutComeName = item.OutputOutcomeName,
                        YieldTypeName = item.YieldTypeName,
                        MeasureResult = item.MeasureResult,
                        Target = item.Target,
                        CountUnitName = item.UnitName
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
                xrerult.Result = new List<ProjectProductResult>();
            }

            return xrerult;
        } catch (Exception ex) 
        {
            xrerult.ResponseCode = 500;
            xrerult.ResponseMsg = ex.Message;
            xrerult.Result = new List<ProjectProductResult>();
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


                var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "project-product" });
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
                    var apiResponse = await _serviceApi.GetDataApiAsync_ProjectProduct(apiParam, item.ProjectCode);
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

                            var proProduct = new MProjectsProduct
                            {
                                ProjectId = resultPA?.ProjectId ?? 0, // Assuming ProjectId is available in the item

                                ProjectCode = item.ProjectCode,
                                ProjectName = item.ProjectName,
                                TProjectsProducts = Subitem.Items.Select(i => new TProjectsProduct
                                {
                                    OrderIndex = i.OrderIndex,
                                    OutputOutcomeName = i.OutputOutComeName,
                                    YieldTypeName = i.YieldTypeName,
                                    MeasureResult = i.MeasureResult,
                                    Target = i.Target,
                                    UnitName = i.CountUnitName,
                                }).ToList()

                            };

                            if (resultPA == null)
                            {
                                await AddProjectProductAsync(proProduct);
                            }
                            else
                            {
                                await UpdateProjectProductAsync(proProduct);
                            }
                        }

                    }

                }


            }

            return "Batch end of day process completed successfully.";
        }

        return "Success";
    }


    public Task AddProjectProductAsync(MProjectsProduct projectProduct)
    {
        return _repository.AddAsync(projectProduct);
    }

    public Task UpdateProjectProductAsync(MProjectsProduct projectProduct)
    {
        return _repository.UpdateAsync(projectProduct);
    }

    public Task DeleteProjectProductAsync(int projectId)
    {
        return _repository.DeleteAsync(projectId);
    }
}
