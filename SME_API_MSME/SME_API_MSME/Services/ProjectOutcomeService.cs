using SME_API_MSME.Entities;
using SME_API_MSME.Models;
using SME_API_MSME.Repository;
using SME_API_MSME.Services;
public class ProjectOutcomeService
{
    private readonly ProjectOutcomeRepository _repository;
    private readonly ICallAPIService _serviceApi;
    private readonly IApiInformationRepository _repositoryApi;
    private readonly ProjectService _projectService;


    public ProjectOutcomeService(ProjectOutcomeRepository repository, ICallAPIService serviceApi
        , IApiInformationRepository repositoryApi, ProjectService projectService)

    {
        _repository = repository; _serviceApi = serviceApi;
        _repositoryApi = repositoryApi;

        _projectService = projectService;

    }

    public Task<IEnumerable<MProjectsOutCome>> GetAllProjectOutcomesAsync()
    {
        return _repository.GetAllAsync();
    }

    //public Task<MProjectsOutCome?> GetProjectOutcomeByIdAsync(int projectId)
    //{
    //    return _repository.GetByIdAsync(projectId);
    //}
    public async Task<ResultProjectOutcomeResponse?> GetProjectOutcomeByIdAsync(long? pProjectCode,string pyear)
    {
        var xrerult = new ResultProjectOutcomeResponse();
        try
        {

            var dataResult = new List<ProjectOutcomeResult>();

            IEnumerable<MProjectsOutCome>? result = null; // Initialize the variable to null

            if (pProjectCode == 0)
            {
                result = await _repository.GetAllAsync();
            }
            else
            {
                var resultPA = await _repository.GetByIdAsync(pProjectCode);

                if (resultPA == null)
                {
                    var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "project-outcome" });
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
                        xrerult.result = new List<ProjectOutcomeResult>();
                        return xrerult;

                    }

                    var apiResponse = await _serviceApi.GetDataApiAsync_ProjectOutcome(apiParam, pProjectCode,pyear);
                   if (apiResponse == null || apiResponse.responseCode == 0 || apiResponse.result.Count ==0)
                    {
                        xrerult.responseCode = 200;
                        xrerult.responseMsg = "No data found";
                        xrerult.result = new List<ProjectOutcomeResult>();
                        return xrerult;

                    }
                    else
                    {
                        foreach (var item in apiResponse.result)
                        {
                            var proProduct = new MProjectsOutCome
                            {
                                ProjectCode = item.ProjectCode,
                                ProjectName = item.ProjectName,
                                Year = pyear, // Assuming pyear is the year for the project
                                TProjectsOutComes = item.Items.Select(i => new TProjectsOutCome
                                {
                                    OrderIndex = i.OrderIndex,
                                    OutputOutcomeName = i.OutputOutComeName,
                                    YieldTypeName = i.YieldTypeName,
                                    MeasureResult = i.MeasureResult,
                                    Target = i.Target,
                                    CountUnitName = i.CountUnitName,
                                }).ToList()

                            };
                            await AddProjectOutcomeAsync(proProduct);
                        }
                    }

                    result = pProjectCode == 0
                        ? await _repository.GetAllAsync()
                        : new List<MProjectsOutCome> { await _repository.GetByIdAsync(pProjectCode) };
                }
                else
                {
                    result = new List<MProjectsOutCome> { resultPA };
                }
            }

            if (result != null && result.Any())
            {
                dataResult.AddRange(result.Select(project => new ProjectOutcomeResult
                {
                    ProjectCode = project.ProjectCode,
                    ProjectName = project.ProjectName,
                    Items = project.TProjectsOutComes.Select(item => new ProjectOutcomeItem
                    {
                        OrderIndex = item.OrderIndex,
                        OutputOutComeName = item.OutputOutcomeName,
                        YieldTypeName = item.YieldTypeName,
                        MeasureResult = item.MeasureResult,
                        Target = item.Target,
                        CountUnitName = item.CountUnitName
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
                xrerult.result = new List<ProjectOutcomeResult>();
            }

            return xrerult;
        }
        catch (Exception ex)
        {
            xrerult.responseCode = 500;
            xrerult.responseMsg = "No data found";
            xrerult.result = new List<ProjectOutcomeResult>() ;
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


                var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "project-outcome" });
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
                    var apiResponse = await _serviceApi.GetDataApiAsync_ProjectOutcome(apiParam, item.ProjectCode,year.ToString());
                    if (apiResponse == null || apiResponse.responseCode == 0 || apiResponse.result.Count == 0)
                    {
                        continue; // Skip to the next project if no data found
                    }
                    else
                    {
                        foreach (var Subitem in apiResponse.result)
                        {
                            // Check if existing budget plan for the project
                            var resultPA = await _repository.GetByIdAsync(Subitem.ProjectCode);

                            var proProduct = new MProjectsOutCome
                            {
                                ProjectId = resultPA?.ProjectId ?? 0, // Assuming ProjectId is available in the item

                                ProjectCode = item.ProjectCode,
                                ProjectName = item.ProjectName,
                                TProjectsOutComes = Subitem.Items.Select(i => new TProjectsOutCome
                                {
                                    OrderIndex = i.OrderIndex,
                                    OutputOutcomeName = i.OutputOutComeName,
                                    YieldTypeName = i.YieldTypeName,
                                    MeasureResult = i.MeasureResult,
                                    Target = i.Target,
                                    CountUnitName = i.CountUnitName,
                                }).ToList()

                            };

                            if (resultPA == null)
                            {
                                await AddProjectOutcomeAsync(proProduct);
                            }
                            else
                            {
                                await UpdateProjectOutcomeAsync(proProduct);
                            }
                        }

                    }

                }


            }

            return "Batch end of day process completed successfully.";
        }

        return "Success";
    }

    public Task AddProjectOutcomeAsync(MProjectsOutCome projectOutcome)
    {
        return _repository.AddAsync(projectOutcome);
    }

    public Task UpdateProjectOutcomeAsync(MProjectsOutCome projectOutcome)
    {
        return _repository.UpdateAsync(projectOutcome);
    }

    public Task DeleteProjectOutcomeAsync(int projectId)
    {
        return _repository.DeleteAsync(projectId);
    }
}
