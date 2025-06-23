using SME_API_MSME.Entities;
using SME_API_MSME.Models;
using SME_API_MSME.Repository;
using SME_API_MSME.Services;
public class ProjectActivityService
{
    private readonly ProjectActivityRepository _repository;
    private readonly ICallAPIService _serviceApi;
    private readonly IApiInformationRepository _repositoryApi;
    private readonly ProjectService _projectService;
    public ProjectActivityService(ProjectActivityRepository repository, ICallAPIService serviceApi,
        IApiInformationRepository repositoryApi, ProjectService projectService)

    {
        _repository = repository;
        _serviceApi = serviceApi;
        _repositoryApi = repositoryApi;
        _projectService = projectService;
    }

    public Task<IEnumerable<MProjectsActivity>> GetAllProjectActivitiesAsync()
    {
        return _repository.GetAllAsync();
    }

    //public Task<MProjectsActivity?> GetProjectActivityByIdAsync(int projectId)
    //{
    //    return _repository.GetByIdAsync(projectId);
    //}
    public async Task<ResultProjectActivityResponse?> GetProjectActivityByIdAsync(long? pProjectCode)
    {
        var xrerult = new ResultProjectActivityResponse();
        try
        {

            var dataResult = new List<ProjectActivityResult>();

            IEnumerable<MProjectsActivity>? result = null; // Initialize the variable to null

            if (pProjectCode == 0)
            {
                result = await _repository.GetAllAsync();
            }
            else
            {
                var resultPA = await _repository.GetByIdAsync(pProjectCode);

                if (resultPA == null)
                {
                    var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "project-activity" });
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
                        xrerult.Result = new List<ProjectActivityResult>();
                        return xrerult;

                    }

                    var apiResponse = await _serviceApi.GetDataApiAsync_ProjectActivity(apiParam, pProjectCode);
                   if (apiResponse == null || apiResponse.ResponseCode == 0 || apiResponse.Result.Count ==0)
                    {
                        xrerult.ResponseCode = 200;
                        xrerult.ResponseMsg = "No data found";
                        xrerult.Result = new List<ProjectActivityResult>();
                        return xrerult;

                    }
                    else
                    {
                        foreach (var item in apiResponse.Result)
                        {
                            var proProduct = new MProjectsActivity
                            {
                                ProjectCode = item.ProjectCode, // Corrected from 'project.ProjectCode' to 'item.ProjectCode'
                                ProjectName = item.ProjectName, // Corrected from 'project.ProjectName' to 'item.ProjectName'
                                TProjectActivities = item.Items.Select(i => new TProjectActivity
                                {
                                    OrderIndex = i.OrderIndex,
                                    ItemActivityDetail = i.ItemActivityDetail,
                                    ActivityWeight = i.ActivityWeight,
                                    Target = i.Target,
                                    UnitName = i.UnitName,
                                    TProjectActivityPlans = i.ActionResultDetail.Select(x => new TProjectActivityPlan
                                    {
                                        MonthName = x.MonthName,
                                        Year = x.Year.ToString(),
                                        ResultValue = x.ResultValue
                                    }).ToList()
                                }).ToList()
                            };

                            await AddProjectActivityAsync(proProduct);
                        }
                    }

                    result = pProjectCode == 0
          ? await _repository.GetAllAsync()
          : new List<MProjectsActivity> { await _repository.GetByIdAsync(pProjectCode) };

                }
                else
                {
                    result = new List<MProjectsActivity> { resultPA };
                }
            }

            if (result != null && result.Any())
            {
                dataResult.AddRange(result.Select(project => new ProjectActivityResult
                {
                    ProjectCode = project.ProjectCode,
                    ProjectName = project.ProjectName,
                    Items = project.TProjectActivities.Select(item => new ProjectActivityItem
                    {
                        OrderIndex = item.OrderIndex ?? 0, // Handle nullable OrderIndex
                        ItemActivityDetail = item.ItemActivityDetail,
                        ActivityWeight = item.ActivityWeight ?? 0, // Handle nullable ActivityWeight
                        Target = item.Target ?? 0, // Handle nullable Target
                        UnitName = item.UnitName,
                        ActionResultDetail = item.TProjectActivityPlans.Select(x => new ActionResultDetail
                        {
                            MonthName = x.MonthName,
                            Year = x.Year != null ? int.Parse(x.Year) : 0, // Handle null Year by providing a default value
                            ResultValue = x.ResultValue ?? 0 // Handle nullable ResultValue
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
                xrerult.Result = null;
            }

            return xrerult;
        }
        catch (Exception ex)
        {
            xrerult.ResponseCode = 500;
            xrerult.ResponseMsg = "No data found";
            xrerult.Result = new List<ProjectActivityResult>();
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


                var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "project-activity" });
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
                    var apiResponse = await _serviceApi.GetDataApiAsync_ProjectActivity(apiParam, item.ProjectCode);
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

                            var proProduct = new MProjectsActivity
                            {
                                ProjectId = resultPA?.ProjectId ?? 0, // Assuming ProjectId is available in the item
                                ProjectCode = item.ProjectCode, // Corrected from 'project.ProjectCode' to 'item.ProjectCode'
                                ProjectName = item.ProjectName, // Corrected from 'project.ProjectName' to 'item.ProjectName'
                                TProjectActivities = Subitem.Items.Select(i => new TProjectActivity
                                {
                                    OrderIndex = i.OrderIndex,
                                    ItemActivityDetail = i.ItemActivityDetail,
                                    ActivityWeight = i.ActivityWeight,
                                    Target = i.Target,
                                    UnitName = i.UnitName,
                                    TProjectActivityPlans = i.ActionResultDetail.Select(x => new TProjectActivityPlan
                                    {
                                        MonthName = x.MonthName,
                                        Year = x.Year.ToString(),
                                        ResultValue = x.ResultValue
                                    }).ToList()
                                }).ToList()
                            };

                            if (resultPA == null)
                            {
                                await AddProjectActivityAsync(proProduct);
                            }
                            else
                            {
                                await UpdateProjectActivityAsync(proProduct);
                            }
                        }

                    }

                }


            }

            return "Batch end of day process completed successfully.";
        }

        return "Success";
    }

    public Task AddProjectActivityAsync(MProjectsActivity projectActivity)
    {
        return _repository.AddAsync(projectActivity);
    }

    public Task UpdateProjectActivityAsync(MProjectsActivity projectActivity)
    {
        return _repository.UpdateAsync(projectActivity);
    }

    public Task DeleteProjectActivityAsync(int projectId)
    {
        return _repository.DeleteAsync(projectId);
    }
}
