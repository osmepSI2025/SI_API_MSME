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
                        xrerult.ResponseCode = 500;
                        xrerult.ResponseMsg = "Api Service Inccorect.";
                        xrerult.Result = new List<ProjectModels>();
                        return xrerult;
                    }
                    var apiResponse = await _serviceApi.GetDataApiAsync_Project(apiParam, year);
                   if (apiResponse == null || apiResponse.ResponseCode == 0 || apiResponse.Result.Count ==0)
                    {
                        xrerult.ResponseCode = 200;
                        xrerult.ResponseMsg = "No data found";
                        xrerult.Result =new List<ProjectModels>();
                        return xrerult;
                    }

                    else
                    {
                        foreach (var item in apiResponse.Result)
                        {
                            var project = new MProject
                            {
                                ProjectCode = item.ProjectCode,
                                BudgetYear = item.BudgetYear,
                                DateApprove = item.DateApprove,
                                OrgId = item.OrgId,
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

                xrerult.ResponseCode = 200;
                xrerult.ResponseMsg = "success";
                xrerult.Result = dataResult;
            }
            else
            {
               xrerult.ResponseCode = 200; // Use 404 for "not found" or adjust as needed
                xrerult.ResponseMsg = "No data found";
                xrerult.Result = new List<ProjectModels>();
            }

            return xrerult;
        } catch (Exception e)
        {
            xrerult.ResponseCode = 500; // Use 404 for "not found" or adjust as needed
            xrerult.ResponseMsg = e.Message;
            xrerult.Result = new List<ProjectModels>();
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
            var Listprojects = await GetProjectByIdAsync(year.ToString());
            if (Listprojects == null || Listprojects.Result.Count == 0)
            {
                continue; // Skip to the next year if no projects found
            }
            else if (Listprojects.ResponseCode == 200)
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

                foreach (var item in Listprojects.Result)
                {
                    var apiResponse = await _serviceApi.GetDataApiAsync_Project(apiParam, item.BudgetYear);
                    if (apiResponse == null || apiResponse.ResponseCode == 0 || apiResponse.Result.Count == 0)
                    {
                        continue; // Skip to the next project if no data found
                    }
                    else
                    {
                        foreach (var Subitem in apiResponse.Result)
                        {
                            // Check if existing budget plan for the project
                            var resultPA = await _repository.GetByIdAsync(Subitem.BudgetYear);

                            var project = new MProject
                            {
                                ProjectCode = item.ProjectCode,
                                BudgetYear = item.BudgetYear,
                                DateApprove = item.DateApprove,
                                OrgId = item.OrgId,
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

                            if (resultPA == null)
                            {
                                await AddProjectAsync(project);
                            }
                            else
                            {
                                await UpdateProjectAsync(project);
                            }
                        }

                    }

                }


            }

            return "Batch end of day process completed successfully.";
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
