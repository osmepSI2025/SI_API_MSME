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

 

    [HttpGet("disbursement-result/{projectCode}")]
    public async Task<IActionResult> GetById(long? projectCode)
    {
        var disbursementResult = await _service.GetDisbursementResultByIdAsync(projectCode);
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
