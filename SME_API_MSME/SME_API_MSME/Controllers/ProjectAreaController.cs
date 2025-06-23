using Microsoft.AspNetCore.Mvc;
using SME_API_MSME.Entities;

[ApiController]
[Route("api/SYS-MSME")]
public class ProjectAreaController : ControllerBase
{
    private readonly ProjectAreaService _service;

    public ProjectAreaController(ProjectAreaService service)
    {
        _service = service;
    }


    [HttpGet("project-area/{projectCode}")]
    public async Task<IActionResult> GetById(long? projectCode)
    {
        var projectArea = await _service.GetProjectAreaByIdAsync(projectCode);
        if (projectArea == null) return NotFound();
        return Ok(projectArea);
    }
    [HttpGet("project-area-Batch")]
    public async Task<IActionResult> GetBatchEndDay()
    {
        var result = await _service.batchEndOfday();
        return Ok();
    }
}
