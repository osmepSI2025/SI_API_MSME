using Microsoft.AspNetCore.Mvc;
using SME_API_MSME.Entities;
using SME_API_MSME.Services;

[ApiController]
[Route("api/SYS-MSME")]
public class EconomicValueController : ControllerBase
{
    private readonly EconomicValueService _service;

    public EconomicValueController(EconomicValueService service)
    {
        _service = service;
    }



    [HttpGet("economic-value/{year}")]
    public async Task<IActionResult> GetById(string? year)
    {
        var economicValue = await _service.GetEconomicValueByIdAsync(year);
        if (economicValue == null) return NotFound();
        return Ok(economicValue);
    }

    [HttpGet("economic-value-Batch")]
    public async Task<IActionResult> GetBatchEndDay()
    {
        var result = await _service.batchEndOfday();
        return Ok();
    }
}
