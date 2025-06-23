using Microsoft.EntityFrameworkCore;
using SME_API_MSME.Entities;

public class BudgetPlanRepository
{
    private readonly MSMEDBContext _context;

    public BudgetPlanRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MBudgetPlan>> GetAllAsync()
    {
        return await _context.MBudgetPlans
            .Include(p => p.TBudgetPlans)
            .ThenInclude(b => b.TBudgeMonthlyPlanDetails)
            .ToListAsync();
    }

    public async Task<MBudgetPlan?> GetByIdAsync(long? pProjectCode)
    {
        try
        {
            return await _context.MBudgetPlans
           .Include(p => p.TBudgetPlans)
           .ThenInclude(b => b.TBudgeMonthlyPlanDetails)
           .FirstOrDefaultAsync(p => p.ProjectCode == pProjectCode);
        }
        catch (Exception ex)
        {
            return null;
        }

    }

    public async Task AddAsync(MBudgetPlan budgetPlan)
    {
        _context.MBudgetPlans.Add(budgetPlan);
        await _context.SaveChangesAsync();
    }

    //public async Task UpdateAsync(MBudgetPlan budgetPlan)
    //{
    //    _context.MBudgetPlans.Update(budgetPlan);
    //    await _context.SaveChangesAsync();
    //}
    public async Task UpdateAsync(MBudgetPlan budgetPlan)
    {
        try {
            var existing = await _context.MBudgetPlans
         .Include(x => x.TBudgetPlans)
         .ThenInclude(x => x.TBudgeMonthlyPlanDetails)
         .FirstOrDefaultAsync(x => x.ProjectCode == budgetPlan.ProjectCode);

            if (existing != null)
            {
                // Remove orphaned TBudgetPlans
                var toRemove = existing.TBudgetPlans
                    .Where(x => !budgetPlan.TBudgetPlans.Any(y => y.BudgetPlanId == x.BudgetPlanId))
                    .ToList();
                foreach (var child in toRemove)
                {
                    _context.TBudgetPlans.Remove(child);
                }

                // Update or add TBudgetPlans
                foreach (var plan in budgetPlan.TBudgetPlans)
                {
                    var existingPlan = existing.TBudgetPlans.FirstOrDefault(x => x.BudgetPlanId == plan.BudgetPlanId);
                    if (existingPlan != null)
                    {
                        // Update properties
                        _context.Entry(existingPlan).CurrentValues.SetValues(plan);

                        // Handle TBudgeMonthlyPlanDetails similarly if needed
                    }
                    else
                    {
                        existing.TBudgetPlans.Add(plan);
                    }
                }

                // Update parent properties
                _context.Entry(existing).CurrentValues.SetValues(budgetPlan);

                await _context.SaveChangesAsync();
            }
        } catch (Exception ex) 
        {
        
        }
 
    }

    public async Task DeleteAsync(int projectId)
    {
        var budgetPlan = await _context.MBudgetPlans.FindAsync(projectId);
        if (budgetPlan != null)
        {
            _context.MBudgetPlans.Remove(budgetPlan);
            await _context.SaveChangesAsync();
        }
    }
}
