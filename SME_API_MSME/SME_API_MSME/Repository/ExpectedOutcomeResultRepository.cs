using SME_API_MSME.Entities;
using Microsoft.EntityFrameworkCore;

public class ExpectedOutcomeResultRepository
{
    private readonly MSMEDBContext _context;

    public ExpectedOutcomeResultRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MExpectedOutcomeResult>> GetAllAsync()
    {
        return await _context.MExpectedOutcomeResults
            .Include(e => e.TExpectedOutcomeResults)
            .ThenInclude(r => r.TExpectedOutcomeResultDetails)
            .ToListAsync();
    }

    public async Task<MExpectedOutcomeResult?> GetByIdAsync(long? pProjectCode, string pYear)
    {
        return await _context.MExpectedOutcomeResults
            .Include(e => e.TExpectedOutcomeResults)
            .ThenInclude(r => r.TExpectedOutcomeResultDetails)
            .FirstOrDefaultAsync(e => e.ProjectCode == pProjectCode && e.Year == pYear);
    }

    public async Task AddAsync(MExpectedOutcomeResult expectedOutcomeResult)
    {
        _context.MExpectedOutcomeResults.Add(expectedOutcomeResult);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MExpectedOutcomeResult expectedOutcomeResult)
    {
        try
        {
            // Detach any tracked entities
            var trackedEntity = _context.MExpectedOutcomeResults.Local
                .FirstOrDefault(e => e.ProjectCode == expectedOutcomeResult.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            var projectId = expectedOutcomeResult.ProjectId;

            // Get existing entity with all child and grandchild records
            var existingEntity = await _context.MExpectedOutcomeResults
                .Include(e => e.TExpectedOutcomeResults)
                .ThenInclude(r => r.TExpectedOutcomeResultDetails)
                .FirstOrDefaultAsync(e => e.ProjectId == projectId);

            if (existingEntity != null)
            {
                // นับจำนวน Child และ GrandChild
                int existingResultCount = existingEntity.TExpectedOutcomeResults.Count;
                int newResultCount = expectedOutcomeResult.TExpectedOutcomeResults?.Count ?? 0;

                int existingDetailCount = existingEntity.TExpectedOutcomeResults
                    .Sum(r => r.TExpectedOutcomeResultDetails.Count);
                int newDetailCount = expectedOutcomeResult.TExpectedOutcomeResults?
                    .Sum(r => r.TExpectedOutcomeResultDetails?.Count ?? 0) ?? 0;

                // ⚠️ เช็คจำนวน: ถ้าเท่ากันทั้ง Result และ Detail ไม่ต้อง update
                if (existingResultCount == newResultCount && existingDetailCount == newDetailCount)
                {
                    // Update เฉพาะ Master properties
                    existingEntity.ProjectCode = expectedOutcomeResult.ProjectCode;
                    existingEntity.ProjectName = expectedOutcomeResult.ProjectName;
                    existingEntity.Year = expectedOutcomeResult.Year;

                    await _context.SaveChangesAsync();
                    return; // ไม่ต้องลบ/เพิ่ม Child
                }

                // ถ้าจำนวนไม่เท่ากัน ให้ลบและ Insert ใหม่
                // ✅ ดึง OutcomeIds ก่อน แล้วใช้ ExecuteDelete โดยตรง (ไม่มี JOIN)
                var outcomeIds = existingEntity.TExpectedOutcomeResults
                    .Select(r => r.OutcomeId)
                    .ToList();

                if (outcomeIds.Any())
                {
                    // ลบ GrandChild โดยใช้ OutcomeIds (ไม่มี JOIN)
                    await _context.TExpectedOutcomeResultDetails
                        .Where(d => outcomeIds.Contains(d.OutcomeId))
                        .ExecuteDeleteAsync();
                }

                // ลบ Child (ใช้ ProjectId โดยตรง - ไม่มี JOIN)
                await _context.TExpectedOutcomeResults
                    .Where(r => r.ProjectId == projectId)
                    .ExecuteDeleteAsync();

                // ⚠️ สำคัญ: Reload entity หลัง ExecuteDelete เพื่อ sync กับ database
                await _context.Entry(existingEntity).Collection(e => e.TExpectedOutcomeResults).LoadAsync();

                // Update master properties
                existingEntity.ProjectCode = expectedOutcomeResult.ProjectCode;
                existingEntity.ProjectName = expectedOutcomeResult.ProjectName;
                existingEntity.Year = expectedOutcomeResult.Year;

                // เพิ่ม Child และ GrandChild ใหม่ทั้งหมด
                if (expectedOutcomeResult.TExpectedOutcomeResults != null && expectedOutcomeResult.TExpectedOutcomeResults.Any())
                {
                    foreach (var result in expectedOutcomeResult.TExpectedOutcomeResults)
                    {
                        // ⚠️ สำคัญ: Reset Identity columns เป็น 0
                        result.OutcomeId = 0;
                        result.ProjectId = projectId;
                        
                        // Reset GrandChild Identity columns
                        if (result.TExpectedOutcomeResultDetails != null && result.TExpectedOutcomeResultDetails.Any())
                        {
                            foreach (var detail in result.TExpectedOutcomeResultDetails)
                            {
                                detail.MonthlyOutcomeId = 0;
                                detail.OutcomeId = 0; // จะถูก set อัตโนมัติหลัง SaveChanges
                            }
                        }
                        
                        // ✅ เพิ่มผ่าน Navigation Property
                        existingEntity.TExpectedOutcomeResults.Add(result);
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
        var expectedOutcomeResult = await _context.MExpectedOutcomeResults.FindAsync(projectId);
        if (expectedOutcomeResult != null)
        {
            _context.MExpectedOutcomeResults.Remove(expectedOutcomeResult);
            await _context.SaveChangesAsync();
        }
    }
}
