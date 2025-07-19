using Microsoft.AspNetCore.Mvc;
using SME_API_MSME.Entities;

[ApiController]
[Route("api/SYS-MSME")]
public class ProjectOutcomeController : ControllerBase
{
    private readonly ProjectOutcomeService _service;

    public ProjectOutcomeController(ProjectOutcomeService service)
    {
        _service = service;
    }


    [HttpGet("project-outcome")]
    public async Task<IActionResult> GetById([FromQuery] long? projectCode, [FromQuery] string year)
    {
        var projectOutcome = await _service.GetProjectOutcomeByIdAsync(projectCode, year);
        if (projectOutcome == null) return NotFound();
        return Ok(projectOutcome);
    }


    //[HttpGet("project-outcome-Batch")]
    //public async Task<IActionResult> GetBatchEndDay()
    //{
    //    var result = await _service.batchEndOfday();
    //    return Ok();
    //}

}
