using SME_API_MSME.Entities;
using SME_API_MSME.Models;
using SME_API_MSME.Repository;
using SME_API_MSME.Services;
public class ProjectAreaService
{
    private readonly ProjectAreaRepository _repository;
    private readonly ICallAPIService _serviceApi;
    private readonly IApiInformationRepository _repositoryApi;
    private readonly ProjectService _projectService;

    public ProjectAreaService(ProjectAreaRepository repository, ICallAPIService serviceApi,
        IApiInformationRepository repositoryApi, ProjectService projectService)

    {
        _repository = repository;
        _serviceApi = serviceApi;
        _repositoryApi = repositoryApi;
        _projectService = projectService;
    }

    public Task<IEnumerable<MProjectArea>> GetAllProjectAreasAsync()
    {
        return _repository.GetAllAsync();
    }

    
    public async Task<ResultProjectAreaResponse?> GetProjectAreaByIdAsync(long? pProjectCode,string pYear)
    {
        var xrerult = new ResultProjectAreaResponse();
        var dataResult = new List<ProjectAreaResult>();
        try {

            IEnumerable<MProjectArea>? result = null; // Initialize the variable to null

            if (pProjectCode == 0)
            {
                result = await _repository.GetAllAsync();
            }
            else
            {
                var resultPA = await _repository.GetByIdAsync(pProjectCode, pYear);

                if (resultPA == null)
                {
                    var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "project-area" });
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
                        xrerult.result =new List<ProjectAreaResult>();
                        return xrerult;

                    }

                    var apiResponse = await _serviceApi.GetDataApiAsync_ProjectArea(apiParam, pProjectCode,pYear);
                   if (apiResponse == null || apiResponse.responseCode == 0 || apiResponse.result.Count ==0)
                    {
                        xrerult.responseCode = 200;
                        xrerult.responseMsg = "No data found";
                        xrerult.result = new List<ProjectAreaResult>();
                        return xrerult;

                    }
                    else
                    {
                        foreach (var item in apiResponse.result)
                        {
                            var proArea = new MProjectArea
                            {
                                ProjectCode = item.ProjectCode,
                                ProjectName = item.ProjectName,
                                Year = pYear,
                                TProjectAreas = item.Items.Select(i => new TProjectArea
                                {
                                    ProvinceId = i.ProvinceId,
                                    ProvinceName = i.ProvinceName
                                }).ToList()
                            };
                            await AddProjectAreaAsync(proArea);
                        }
                    }

                    result = pProjectCode == 0
                        ? await _repository.GetAllAsync()
                        : new List<MProjectArea> { await _repository.GetByIdAsync(pProjectCode, pYear) };
                }
                else
                {
                    result = new List<MProjectArea> { resultPA };
                }
            }

            if (result != null && result.Any())
            {
                dataResult.AddRange(result.Select(project => new ProjectAreaResult
                {
                    ProjectCode = project.ProjectCode,
                    ProjectName = project.ProjectName,
                    
                    Items = project.TProjectAreas.Select(item => new ProjectAreaItem
                    {
                        ProvinceId = item.ProvinceId,
                        ProvinceName = item.ProvinceName,
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
                xrerult.result = new List<ProjectAreaResult>() ;
            }

            return xrerult;
        }
        catch(Exception ex) 
        
        {
            xrerult.responseCode = 500;
            xrerult.responseMsg = "No data found";
            xrerult.result = new List<ProjectAreaResult>();
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
            if (Listprojects == null || Listprojects.result.Count == 0)
            {
                continue; // Skip to the next year if no projects found
            }
            else if (Listprojects.responseCode == 200)
            {


                var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "project-area" });
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
                    var apiResponse = await _serviceApi.GetDataApiAsync_ProjectArea(apiParam, item.ProjectCode, year.ToString());
                    if (apiResponse == null || apiResponse.responseCode == 0 || apiResponse.result.Count == 0)
                    {
                        continue; // Skip to the next project if no data found
                    }
                    else
                    {
                        foreach (var Subitem in apiResponse.result)
                        {
                            // Check if existing budget plan for the project
                            var resultPA = await _repository.GetByIdAsync(Subitem.ProjectCode,null);

                            var proArea = new MProjectArea
                            {
                                ProjectId = resultPA?.ProjectId ?? 0, // Assuming ProjectId is available in the item
                                ProjectCode = item.ProjectCode,
                                ProjectName = item.ProjectName,
                                TProjectAreas = Subitem.Items.Select(i => new TProjectArea
                                {
                                    ProvinceId = i.ProvinceId,
                                    ProvinceName = i.ProvinceName
                                }).ToList()
                            };
                          

                            if (resultPA == null)
                            {
                                await AddProjectAreaAsync(proArea);
                            }
                            else
                            {
                                await UpdateProjectAreaAsync(proArea);
                            }
                        }

                    }

                }


            }

            return "Batch end of day process completed successfully.";
        }

        return "Success";
    }


    public Task AddProjectAreaAsync(MProjectArea projectArea)
    {
        return _repository.AddAsync(projectArea);
    }

    public Task UpdateProjectAreaAsync(MProjectArea projectArea)
    {
        return _repository.UpdateAsync(projectArea);
    }

    public Task DeleteProjectAreaAsync(int projectId)
    {
        return _repository.DeleteAsync(projectId);
    }
}
