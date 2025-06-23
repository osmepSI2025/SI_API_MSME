using Microsoft.AspNetCore.Mvc;
using SME_API_MSME.Entities;

[ApiController]
[Route("api/SYS-MSME")]
public class BudgetPlanController : ControllerBase
{
    private readonly BudgetPlanService _service;

    public BudgetPlanController(BudgetPlanService service)
    {
        _service = service;
    }

   

    [HttpGet("bugget-plan/{projectId}")]
    public async Task<IActionResult> GetById(int projectId)
    {
        var budgetPlan = await _service.GetBudgetPlanByIdAsync(projectId);
        if (budgetPlan == null) return NotFound();
        return Ok(budgetPlan);
    }

    [HttpGet("bugget-plan-Batch")]
    public async Task<IActionResult> GetBatchEndDay()
    {
        

        var result = await _service.batchEndOfday();
        return Ok();
    }
}
