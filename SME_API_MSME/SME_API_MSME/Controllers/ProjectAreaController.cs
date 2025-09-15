using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
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


    [HttpGet("project-area")]
    public async Task<IActionResult> GetById([FromQuery] long? projectCode, [FromQuery] string year)
    {
        var projectArea = await _service.GetProjectAreaByIdAsync(projectCode, year);
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
