using SME_API_MSME.Entities;
using SME_API_MSME.Models;
using SME_API_MSME.Repository;
using SME_API_MSME.Services;
using System.Collections.Generic;
public class ProjectService
{
    private readonly ProjectRepository _repository;
    private readonly ICallAPIService _serviceApi;
    private readonly IApiInformationRepository _repositoryApi;
  
    public ProjectService(ProjectRepository repository, ICallAPIService serviceApi,
        IApiInformationRepository repositoryApi)
    {
        _repository = repository;
        _serviceApi = serviceApi;
        _repositoryApi = repositoryApi;
       
    }

    public Task<IEnumerable<MProject>> GetAllProjectsAsync()
    {
        return _repository.GetAllAsync();
    }

   
    public async Task<ResultApiResponeProject?> GetProjectByIdAsync(string year)
    {
        var xrerult = new ResultApiResponeProject();
        var dataResult = new List<ProjectModels>();

        IEnumerable<MProject>? result;
        try 
        {
            if (year == "0")
            {
                result = await _repository.GetAllAsync();
            }
            else
            {
                result = await _repository.GetByIdAsync(year);

                if (!result.Any())
                {
                    var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "project" });
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
                        UpdateDate = x.UpdateDate
                        ,
                        Bearer = x.Bearer,
                    }).First(); // ดึงตัวแรกของ List
                    if (apiParam == null)
                    {
                        xrerult.responseCode = 500;
                        xrerult.responseMsg = "Api Service Inccorect.";
                        xrerult.result = new List<ProjectModels>();
                        return xrerult;
                    }
                    var apiResponse = await _serviceApi.GetDataApiAsync_Project(apiParam, year);
                   if (apiResponse == null || apiResponse.responseCode == 0 || apiResponse.result.Count ==0)
                    {
                        xrerult.responseCode = 200;
                        xrerult.responseMsg = "No data found";
                        xrerult.result =new List<ProjectModels>();
                        return xrerult;
                    }

                    else
                    {
                        foreach (var item in apiResponse.result)
                        {
                            if (item.ProjectCode != 0 && item.OrgId!=null) 
                            {
                                var project = new MProject
                                {
                                    ProjectCode = item.ProjectCode,
                                    BudgetYear = item.BudgetYear,
                                    DateApprove = item.DateApprove,
                                    // Change this line in the mapping from ProjectModels to MProject
                                    OrgId = item.OrgId ?? null,
                                    OrgName = item.OrgName,
                                    ProjectBudget = item.ProjectBudget,
                                    ProjectOffBudget = item.ProjectOffBudget,
                                    ProjectSumBudget = item.ProjectSumBudget,
                                    SmeProjectStatusName = item.SmeProjectStatusName,
                                    LegalGroupName = item.LegalGroupName,
                                    ProjectName = item.ProjectName,
                                    ProjectNameInitials = item.ProjectNameInitials,
                                    ProjectReason = item.ProjectReason,
                                    ProjectPurpose = item.ProjectPurpose,
                                    TypeBudget = item.TypeBudget,
                                    TypeResultMsme = item.TypeResultMsme,
                                    PlanMessage = item.PlanMessage,
                                    EndDate = item.EndDate,
                                    StartDate = item.StartDate
                                };
                                await AddProjectAsync(project);
                            }
                           
                        }
                    }

                    result = await _repository.GetByIdAsync(year);
                }
            }

            if (result != null && result.Any())
            {
                // Map MProject to ApiResponeProjectModels if necessary
                dataResult.AddRange(result.Select(project => new ProjectModels
                {
                    // Map properties from MProject to ApiResponeProjectModels
                    ProjectCode = project.ProjectCode?? 0,
                    BudgetYear = project.BudgetYear,
                    DateApprove = project.DateApprove,
                    OrgId = project.OrgId,
                    OrgName = project.OrgName,
                    ProjectBudget = project.ProjectBudget,
                    ProjectOffBudget = project.ProjectOffBudget,
                    ProjectSumBudget = project.ProjectSumBudget,
                    SmeProjectStatusName = project.SmeProjectStatusName,
                    LegalGroupName = project.LegalGroupName,
                    ProjectName = project.ProjectName,
                    ProjectNameInitials = project.ProjectNameInitials,
                    ProjectReason = project.ProjectReason,
                    ProjectPurpose = project.ProjectPurpose,
                    TypeBudget = project.TypeBudget,
                    TypeResultMsme = project.TypeResultMsme,
                    EndDate = project.EndDate,
                    StartDate = project.StartDate,
                }));

                xrerult.responseCode = 200;
                xrerult.responseMsg = "success";
                xrerult.result = dataResult;
            }
            else
            {
               xrerult.responseCode = 200; // Use 404 for "not found" or adjust as needed
                xrerult.responseMsg = "No data found";
                xrerult.result = new List<ProjectModels>();
            }

            return xrerult;
        } catch (Exception e)
        {
            xrerult.responseCode = 500; // Use 404 for "not found" or adjust as needed
            xrerult.responseMsg = e.Message;
            xrerult.result = new List<ProjectModels>();
            return xrerult;
        }

       
    }

    public async Task<string> batchEndOfday()
    {
        int currentYear = DateTime.Now.Year;
        int currentYearBE = currentYear < 2500 ? currentYear + 543 : currentYear; // แปลงเป็น พ.ศ. ถ้ายังเป็น ค.ศ.
        int currentYearTo = currentYearBE + 1;

        for (int year = currentYearBE - 0; year <= currentYearTo; year++)
        {
            //get projects by year  
            var Listprojects = await GetProjectByIdAsync(year.ToString());
            if (Listprojects == null || Listprojects.result.Count == 0)
            {
                continue; // Skip to the next year if no projects found
            }
            else if (Listprojects.responseCode == 200)
            {


                var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "project" });
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
                var apiResponse = await _serviceApi.GetDataApiAsync_Project(apiParam, year.ToString());

                var apiProjects = apiResponse.result.Where(x => x.ProjectCode != 0 && x.OrgId != null).ToList();
                // Check if the number of projects from the API matches the number in the database

                if (Listprojects.result.Count != apiProjects.Count)
                {
                    var missingProjects = apiProjects
    .Where(x => !Listprojects.result.Any(db => db.ProjectCode == x.ProjectCode))
    .ToList();
                    foreach (var subitem in missingProjects)
                    {
                        // Check if project exists by ProjectCode and BudgetYear
                        var existing = (await _repository.GetByProjectCodeAsync(subitem.BudgetYear, subitem.ProjectCode));
                         

                        var project = new MProject
                        {
                            ProjectCode = subitem.ProjectCode,
                            BudgetYear = subitem.BudgetYear,
                            DateApprove = subitem.DateApprove,
                            OrgId = subitem.OrgId,
                            OrgName = subitem.OrgName,
                            ProjectBudget = subitem.ProjectBudget,
                            ProjectOffBudget = subitem.ProjectOffBudget,
                            ProjectSumBudget = subitem.ProjectSumBudget,
                            SmeProjectStatusName = subitem.SmeProjectStatusName,
                            LegalGroupName = subitem.LegalGroupName,
                            ProjectName = subitem.ProjectName,
                            ProjectNameInitials = subitem.ProjectNameInitials,
                            ProjectReason = subitem.ProjectReason,
                            ProjectPurpose = subitem.ProjectPurpose,
                            TypeBudget = subitem.TypeBudget,
                            TypeResultMsme = subitem.TypeResultMsme,
                            PlanMessage = subitem.PlanMessage,
                            EndDate = subitem.EndDate,
                            StartDate = subitem.StartDate
                        };

                        if (existing == null || !existing.Any())
                            await AddProjectAsync(project);
                        else
                            await UpdateProjectAsync(project);
                    }
                }
            }

           // return "Batch end of day process completed successfully.";
        }

        return "Success";
    }

    public async Task<string> batchEndOfdayxxx()
    {
        int currentYear = DateTime.Now.Year;
        int currentYearBE = currentYear < 2500 ? currentYear + 543 : currentYear; // แปลงเป็น พ.ศ. ถ้ายังเป็น ค.ศ.
        int currentYearTo = currentYearBE + 1;

        for (int year = currentYearBE - 0; year <= currentYearTo; year++)
        {


            //get projects by year  
            var Listprojects = await GetProjectByIdAsync(year.ToString());
            if (Listprojects == null || Listprojects.result.Count == 0)
            {
                continue; // Skip to the next year if no projects found
            }
            else if (Listprojects.responseCode == 200)
            {


                var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "project" });
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
                var apiResponse = await _serviceApi.GetDataApiAsync_Project(apiParam, year.ToString());

                var apiProjects = apiResponse.result.Where(x => x.ProjectCode != 0).ToList();
                // Check if the number of projects from the API matches the number in the database

                if (Listprojects.result.Count != apiProjects.Count)
                {
                    var missingProjects = apiProjects
    .Where(x => !Listprojects.result.Any(db => db.ProjectCode == x.ProjectCode))
    .ToList();
                    foreach (var subitem in missingProjects)
                    {
                        // Check if project exists by ProjectCode and BudgetYear
                        var existing = (await _repository.GetByProjectCodeAsync(subitem.BudgetYear, subitem.ProjectCode));


                        var project = new MProject
                        {
                            ProjectCode = subitem.ProjectCode,
                            BudgetYear = subitem.BudgetYear,
                            DateApprove = subitem.DateApprove,
                            OrgId = subitem.OrgId,
                            OrgName = subitem.OrgName,
                            ProjectBudget = subitem.ProjectBudget,
                            ProjectOffBudget = subitem.ProjectOffBudget,
                            ProjectSumBudget = subitem.ProjectSumBudget,
                            SmeProjectStatusName = subitem.SmeProjectStatusName,
                            LegalGroupName = subitem.LegalGroupName,
                            ProjectName = subitem.ProjectName,
                            ProjectNameInitials = subitem.ProjectNameInitials,
                            ProjectReason = subitem.ProjectReason,
                            ProjectPurpose = subitem.ProjectPurpose,
                            TypeBudget = subitem.TypeBudget,
                            TypeResultMsme = subitem.TypeResultMsme,
                            PlanMessage = subitem.PlanMessage,
                            EndDate = subitem.EndDate,
                            StartDate = subitem.StartDate
                        };

                        if (existing == null || !existing.Any())
                            await AddProjectAsync(project);
                        else
                            await UpdateProjectAsync(project);
                    }
                }
            }

            // return "Batch end of day process completed successfully.";
        }

        return "Success";
    }

    public Task AddProjectAsync(MProject project)
    {
        return _repository.AddAsync(project);
    }

    public Task UpdateProjectAsync(MProject project)
    {
        return _repository.UpdateAsync(project);
    }

    public Task DeleteProjectAsync(int id)
    {
        return _repository.DeleteAsync(id);
    }
}
