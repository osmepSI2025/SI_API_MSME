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



    [HttpGet("project-product/{projectCode}")]
    public async Task<IActionResult> GetById(long? projectCode)
    {
        var projectProduct = await _service.GetProjectProductByIdAsync(projectCode);
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
