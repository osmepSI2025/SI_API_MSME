using Microsoft.AspNetCore.Mvc;
using SME_API_MSME.Entities;

[ApiController]
[Route("api/SYS-MSME")]
public class OutcomeResultController : ControllerBase
{
    private readonly OutcomeResultService _service;

    public OutcomeResultController(OutcomeResultService service)
    {
        _service = service;
    }

  

    [HttpGet("outcome-result/{projectCode}")]
    public async Task<IActionResult> GetById(long? projectCode)
    {
        var outcomeResult = await _service.GetOutcomeResultByIdAsync(projectCode);
        if (outcomeResult == null) return NotFound();
        return Ok(outcomeResult);
    }

    [HttpGet("outcome-result-Batch")]
    public async Task<IActionResult> GetBatchEndDay()
    {
        var result = await _service.batchEndOfday();
        return Ok();
    }
}
