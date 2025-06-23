using Microsoft.AspNetCore.Mvc;
using SME_API_MSME.Entities;

[ApiController]
[Route("api/SYS-MSME")]
public class ProjectActivityController : ControllerBase
{
    private readonly ProjectActivityService _service;

    public ProjectActivityController(ProjectActivityService service)
    {
        _service = service;
    }

    [HttpGet("project-activity/{projectCode}")]
    public async Task<IActionResult> GetById(long? projectCode)
    {
        var projectActivity = await _service.GetProjectActivityByIdAsync(projectCode);
        if (projectActivity == null) return NotFound();
        return Ok(projectActivity);
    }

    [HttpGet("project-activity-Batch")]
    public async Task<IActionResult> GetBatchEndDay()
    {
        var result = await _service.batchEndOfday();
        return Ok();
    }

}
