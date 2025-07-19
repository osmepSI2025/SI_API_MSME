using Microsoft.AspNetCore.Mvc;
using SME_API_MSME.Entities;

[ApiController]
[Route("api/SYS-MSME")]
public class ExpectedOutcomeController : ControllerBase
{
    private readonly ExpectedOutcomeService _service;

    public ExpectedOutcomeController(ExpectedOutcomeService service)
    {
        _service = service;
    }

   

    [HttpGet("expected-outcome")]
    public async Task<IActionResult> GetById([FromQuery] long? projectCode, [FromQuery] string year)
    {
        var expectedOutcome = await _service.GetExpectedOutcomeByIdAsync(projectCode,year);
        if (expectedOutcome == null) return NotFound();
        return Ok(expectedOutcome);
    }

    [HttpGet("expected-outcome-Batch")]
    public async Task<IActionResult> GetBatchEndDay()
    {
        var result = await _service.batchEndOfday();
        return Ok();
    }

}
