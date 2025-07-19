using Microsoft.AspNetCore.Mvc;
using SME_API_MSME.Entities;

[ApiController]
[Route("api/SYS-MSME")]
public class PerformanceResultController : ControllerBase
{
    private readonly PerformanceResultService _service;

    public PerformanceResultController(PerformanceResultService service)
    {
        _service = service;
    }

   

    [HttpGet("performance-result")]
    public async Task<IActionResult> GetById([FromQuery] long? projectCode, [FromQuery] string year)
    {
        var performanceResult = await _service.GetPerformanceResultByIdAsync(projectCode, year.ToString());
        if (performanceResult == null) return NotFound();
        return Ok(performanceResult);
    }

    [HttpGet("performance-result-Batch")]
    public async Task<IActionResult> GetBatchEndDay()
    {
        var result = await _service.batchEndOfday();
        return Ok();
    }
}
