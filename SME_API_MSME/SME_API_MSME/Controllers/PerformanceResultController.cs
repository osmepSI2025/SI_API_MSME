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

   

    [HttpGet("performance-result/{projectCode}")]
    public async Task<IActionResult> GetById(long? projectCode)
    {
        var performanceResult = await _service.GetPerformanceResultByIdAsync(projectCode);
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
