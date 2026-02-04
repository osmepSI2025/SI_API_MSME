using Microsoft.EntityFrameworkCore;
using SME_API_MSME.Entities;

public class ExpectedOutcomeRepository
{
    private readonly MSMEDBContext _context;

    public ExpectedOutcomeRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MExpectedOutcome>> GetAllAsync()
    {
        return await _context.MExpectedOutcomes
            .Include(e => e.TExpectedFinalOutcomes)
            .ThenInclude(f => f.TExpectedFinalOutcomeMonthlyPlans)
            .ToListAsync();
    }

    public async Task<MExpectedOutcome?> GetByIdAsync(long? pProjectCode, string pyear)
    {
        return await _context.MExpectedOutcomes
            .Include(e => e.TExpectedFinalOutcomes)
            .ThenInclude(f => f.TExpectedFinalOutcomeMonthlyPlans)
            .FirstOrDefaultAsync(e => e.ProjectCode == pProjectCode && e.Year == pyear);
    }

    public async Task AddAsync(MExpectedOutcome expectedOutcome)
    {
        _context.MExpectedOutcomes.Add(expectedOutcome);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MExpectedOutcome expectedOutcome)
    {
        try
        {
            // Detach any tracked entities
            var trackedEntity = _context.MExpectedOutcomes.Local
                .FirstOrDefault(e => e.ProjectId == expectedOutcome.ProjectId);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            var projectId = expectedOutcome.ProjectId;

            // Get existing entity with all child and grandchild records
            var existingEntity = await _context.MExpectedOutcomes
                .Include(e => e.TExpectedFinalOutcomes)
                .ThenInclude(f => f.TExpectedFinalOutcomeMonthlyPlans)
                .FirstOrDefaultAsync(e => e.ProjectId == projectId);

            if (existingEntity != null)
            {
                // นับจำนวน Child และ GrandChild
                int existingOutcomeCount = existingEntity.TExpectedFinalOutcomes.Count;
                int newOutcomeCount = expectedOutcome.TExpectedFinalOutcomes?.Count ?? 0;

                int existingPlanCount = existingEntity.TExpectedFinalOutcomes
                    .Sum(f => f.TExpectedFinalOutcomeMonthlyPlans.Count);
                int newPlanCount = expectedOutcome.TExpectedFinalOutcomes?
                    .Sum(f => f.TExpectedFinalOutcomeMonthlyPlans?.Count ?? 0) ?? 0;

                // ⚠️ เช็คจำนวน: ถ้าเท่ากันทั้ง Outcome และ Plan ไม่ต้อง update
                if (existingOutcomeCount == newOutcomeCount && existingPlanCount == newPlanCount)
                {
                    // Update เฉพาะ Master properties
                    existingEntity.ProjectCode = expectedOutcome.ProjectCode;
                    existingEntity.ProjectName = expectedOutcome.ProjectName;
                    existingEntity.Year = expectedOutcome.Year;

                    await _context.SaveChangesAsync();
                    return; // ไม่ต้องลบ/เพิ่ม Child
                }

                // ถ้าจำนวนไม่เท่ากัน ให้ลบและ Insert ใหม่
                // ✅ ดึง FinalOutcomeIds ก่อน แล้วใช้ ExecuteDelete โดยตรง (ไม่มี JOIN)
                var finalOutcomeIds = existingEntity.TExpectedFinalOutcomes
                    .Select(f => f.FinalOutcomeId)
                    .ToList();

                if (finalOutcomeIds.Any())
                {
                    // ลบ GrandChild โดยใช้ FinalOutcomeIds (ไม่มี JOIN)
                    await _context.TExpectedFinalOutcomeMonthlyPlans
                        .Where(p => finalOutcomeIds.Contains(p.FinalOutcomeId))
                        .ExecuteDeleteAsync();
                }

                // ลบ Child (ใช้ ProjectId โดยตรง - ไม่มี JOIN)
                await _context.TExpectedFinalOutcomes
                    .Where(f => f.ProjectId == projectId)
                    .ExecuteDeleteAsync();

                // ⚠️ สำคัญ: Reload entity หลัง ExecuteDelete เพื่อ sync กับ database
                await _context.Entry(existingEntity).Collection(e => e.TExpectedFinalOutcomes).LoadAsync();

                // Update master properties
                existingEntity.ProjectCode = expectedOutcome.ProjectCode;
                existingEntity.ProjectName = expectedOutcome.ProjectName;
                existingEntity.Year = expectedOutcome.Year;

                // เพิ่ม Child และ GrandChild ใหม่ทั้งหมด
                if (expectedOutcome.TExpectedFinalOutcomes != null && expectedOutcome.TExpectedFinalOutcomes.Any())
                {
                    foreach (var outcome in expectedOutcome.TExpectedFinalOutcomes)
                    {
                        // ⚠️ สำคัญ: Reset Identity columns เป็น 0
                        outcome.FinalOutcomeId = 0;
                        outcome.ProjectId = projectId;
                        
                        // Reset GrandChild Identity columns
                        if (outcome.TExpectedFinalOutcomeMonthlyPlans != null && outcome.TExpectedFinalOutcomeMonthlyPlans.Any())
                        {
                            foreach (var plan in outcome.TExpectedFinalOutcomeMonthlyPlans)
                            {
                                plan.MonthlyPlanId = 0;
                                plan.FinalOutcomeId = 0; // จะถูก set อัตโนมัติหลัง SaveChanges
                            }
                        }
                        
                        // ✅ เพิ่มผ่าน Navigation Property
                        existingEntity.TExpectedFinalOutcomes.Add(outcome);
                    }
                }

                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            // Log exception
            throw;
        }
    }

    public async Task DeleteAsync(int projectId)
    {
        var expectedOutcome = await _context.MExpectedOutcomes.FindAsync(projectId);
        if (expectedOutcome != null)
        {
            _context.MExpectedOutcomes.Remove(expectedOutcome);
            await _context.SaveChangesAsync();
        }
    }
}
