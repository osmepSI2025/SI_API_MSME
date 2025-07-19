using Microsoft.AspNetCore.Mvc;
using SME_API_MSME.Entities;

[ApiController]
[Route("api/SYS-MSME")]
public class ProjectProductController : ControllerBase
{
    private readonly ProjectProductService _service;

    public ProjectProductController(ProjectProductService service)
    {
        _service = service;
    }



    [HttpGet("project-product")]
    public async Task<IActionResult> GetById([FromQuery] long? projectCode, [FromQuery] string year)
    {
        var projectProduct = await _service.GetProjectProductByIdAsync(projectCode, year);
        if (projectProduct == null) return NotFound();
        return Ok(projectProduct);
    }

    //[HttpGet("project-product-Batch")]
    //public async Task<IActionResult> GetBatchEndDay()
    //{
    //    var result = await _service.batchEndOfday();
    //    return Ok();
    //}

}
