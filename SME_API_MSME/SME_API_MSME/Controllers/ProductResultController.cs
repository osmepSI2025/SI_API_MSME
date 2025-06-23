using Microsoft.AspNetCore.Mvc;
using SME_API_MSME.Entities;

[ApiController]
[Route("api/SYS-MSME")]
public class ProductResultController : ControllerBase
{
    private readonly ProductResultService _service;

    public ProductResultController(ProductResultService service)
    {
        _service = service;
    }

   

    [HttpGet("product-result/{projectCode}")]
    public async Task<IActionResult> GetById(long? projectCode)
    {
        var productResult = await _service.GetProductResultByIdAsync(projectCode);
        if (productResult == null) return NotFound();
        return Ok(productResult);
    }

    [HttpGet("product-result-Batch")]
    public async Task<IActionResult> GetBatchEndDay()
    {
        var result = await _service.batchEndOfday();
        return Ok();
    }
}
