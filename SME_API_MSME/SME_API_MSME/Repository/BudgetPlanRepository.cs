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

    public async Task<MBudgetPlan?> GetByIdAsync(long? pProjectCode, string pYear)
    {
        try
        {
            return await _context.MBudgetPlans
           .Include(p => p.TBudgetPlans)
           .ThenInclude(b => b.TBudgeMonthlyPlanDetails)
           .FirstOrDefaultAsync(p => p.ProjectCode == pProjectCode && p.Year == pYear);
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

    public async Task UpdateAsync(MBudgetPlan budgetPlan)
    {
        try
        {
            // Detach any tracked entities
            var trackedEntity = _context.MBudgetPlans.Local
                .FirstOrDefault(e => e.ProjectCode == budgetPlan.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            var projectId = budgetPlan.ProjectId;

            // Get existing entity with all child and grandchild records
            var existingEntity = await _context.MBudgetPlans
                .Include(p => p.TBudgetPlans)
                .ThenInclude(b => b.TBudgeMonthlyPlanDetails)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (existingEntity != null)
            {
                // นับจำนวน Child และ GrandChild
                int existingPlanCount = existingEntity.TBudgetPlans.Count;
                int newPlanCount = budgetPlan.TBudgetPlans?.Count ?? 0;

                int existingDetailCount = existingEntity.TBudgetPlans
                    .Sum(b => b.TBudgeMonthlyPlanDetails.Count);
                int newDetailCount = budgetPlan.TBudgetPlans?
                    .Sum(b => b.TBudgeMonthlyPlanDetails?.Count ?? 0) ?? 0;

                // ⚠️ เช็คจำนวน: ถ้าเท่ากันทั้ง Plan และ Detail ไม่ต้อง update
                if (existingPlanCount == newPlanCount && existingDetailCount == newDetailCount)
                {
                    // Update เฉพาะ Master properties
                    existingEntity.ProjectCode = budgetPlan.ProjectCode;
                    existingEntity.ProjectName = budgetPlan.ProjectName;
                    existingEntity.Year = budgetPlan.Year;

                    await _context.SaveChangesAsync();
                    return; // ไม่ต้องลบ/เพิ่ม Child
                }

                // ถ้าจำนวนไม่เท่ากัน ให้ลบและ Insert ใหม่
                // ✅ ดึง BudgetPlanIds ก่อน แล้วใช้ ExecuteDelete โดยตรง (ไม่มี JOIN)
                var budgetPlanIds = existingEntity.TBudgetPlans
                    .Select(b => b.BudgetPlanId)
                    .ToList();

                if (budgetPlanIds.Any())
                {
                    // ลบ GrandChild โดยใช้ BudgetPlanIds (ไม่มี JOIN)
                    await _context.TBudgeMonthlyPlanDetails
                        .Where(d => budgetPlanIds.Contains(d.BudgetPlanId))
                        .ExecuteDeleteAsync();
                }

                // ลบ Child (ใช้ ProjectId โดยตรง - ไม่มี JOIN)
                await _context.TBudgetPlans
                    .Where(b => b.ProjectId == projectId)
                    .ExecuteDeleteAsync();

                // ⚠️ สำคัญ: Reload entity หลัง ExecuteDelete เพื่อ sync กับ database
                await _context.Entry(existingEntity).Collection(e => e.TBudgetPlans).LoadAsync();

                // Update master properties
                existingEntity.ProjectCode = budgetPlan.ProjectCode;
                existingEntity.ProjectName = budgetPlan.ProjectName;
                existingEntity.Year = budgetPlan.Year;

                // เพิ่ม Child และ GrandChild ใหม่ทั้งหมด
                if (budgetPlan.TBudgetPlans != null && budgetPlan.TBudgetPlans.Any())
                {
                    foreach (var plan in budgetPlan.TBudgetPlans)
                    {
                        // ⚠️ สำคัญ: Reset Identity columns เป็น 0
                        plan.BudgetPlanId = 0;
                        plan.ProjectId = projectId;
                        
                        // Reset GrandChild Identity columns
                        if (plan.TBudgeMonthlyPlanDetails != null && plan.TBudgeMonthlyPlanDetails.Any())
                        {
                            foreach (var detail in plan.TBudgeMonthlyPlanDetails)
                            {
                                detail.MonthlyDetailId = 0;
                                detail.BudgetPlanId = 0; // จะถูก set อัตโนมัติหลัง SaveChanges
                            }
                        }
                        
                        // ✅ เพิ่มผ่าน Navigation Property
                        existingEntity.TBudgetPlans.Add(plan);
                    }
                }

                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            // Log exception if needed
            throw;
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
