using Microsoft.AspNetCore.Mvc;
using SME_API_MSME.Entities;
using SME_API_MSME.Models;
[ApiController]
[Route("api/SYS-MSME")]
public class ProjectController : ControllerBase
{
    private readonly ProjectService _service;

    public ProjectController(ProjectService service)
    {
        _service = service;
    }

 

    [HttpGet("project/{year}")]
    public async Task<ResultApiResponeProject> GetById(string year)
    {
        var project = await _service.GetProjectByIdAsync(year);
       
        return project;
    }

    [HttpGet("project-Batch")]
    public async Task<IActionResult> GetBatchEndDay()
    {
        var result = await _service.batchEndOfday();
        return Ok();
    }

}
