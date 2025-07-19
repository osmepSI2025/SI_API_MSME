using Microsoft.AspNetCore.Mvc;
using SME_API_MSME.Entities;

[ApiController]
[Route("api/SYS-MSME")]
public class DisbursementResultController : ControllerBase
{
    private readonly DisbursementResultService _service;

    public DisbursementResultController(DisbursementResultService service)
    {
        _service = service;
    }

 

    [HttpGet("disbursement-result")]
    public async Task<IActionResult> GetById([FromQuery] long? projectCode, [FromQuery] string year)
    {
        var disbursementResult = await _service.GetDisbursementResultByIdAsync(projectCode, year.ToString());
        if (disbursementResult == null) return NotFound();
        return Ok(disbursementResult);
    }

    [HttpGet("disbursement-result-Batch")]
    public async Task<IActionResult> GetBatchEndDay()
    {
        var result = await _service.batchEndOfday();
        return Ok();
    }


}
