using Microsoft.AspNetCore.Mvc;
using SME_API_MSME.Entities;

[ApiController]
[Route("api/SYS-MSME")]
public class ExpectedOutcomeResultController : ControllerBase
{
    private readonly ExpectedOutcomeResultService _service;

    public ExpectedOutcomeResultController(ExpectedOutcomeResultService service)
    {
        _service = service;
    }

 

    [HttpGet("expected-outcome-result")]
    public async Task<IActionResult> GetById([FromQuery] long? projectCode, [FromQuery] string year)
    {
        var expectedOutcomeResult = await _service.GetExpectedOutcomeResultByIdAsync(projectCode, year);
        if (expectedOutcomeResult == null) return NotFound();
        return Ok(expectedOutcomeResult);
    }

    [HttpGet("expected-outcome-resul-Batch")]
    public async Task<IActionResult> GetBatchEndDay()
    {
        var result = await _service.batchEndOfday();
        return Ok();
    }
}
