using SME_API_MSME.Entities;
using Microsoft.EntityFrameworkCore;

public class OutcomeResultRepository
{
    private readonly MSMEDBContext _context;

    public OutcomeResultRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MOutcomeResult>> GetAllAsync()
    {
        return await _context.MOutcomeResults
            .Include(o => o.TOutcomeResults)
            .ThenInclude(r => r.TOutcomeResultDetails)
            .ToListAsync();
    }

    public async Task<MOutcomeResult?> GetByIdAsync(long? pProjectCode, string pYear)
    {
        return await _context.MOutcomeResults
            .Include(o => o.TOutcomeResults)
            .ThenInclude(r => r.TOutcomeResultDetails)
            .FirstOrDefaultAsync(o => o.ProjectCode == pProjectCode && o.Year == pYear);
    }

    public async Task AddAsync(MOutcomeResult outcomeResult)
    {
        _context.MOutcomeResults.Add(outcomeResult);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MOutcomeResult outcomeResult)
    {
        try
        {
            // Detach any tracked entities
            var trackedEntity = _context.MOutcomeResults.Local
                .FirstOrDefault(e => e.ProjectCode == outcomeResult.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            var projectId = outcomeResult.ProjectId;

            // Get existing entity with all child and grandchild records
            var existingEntity = await _context.MOutcomeResults
                .Include(o => o.TOutcomeResults)
                .ThenInclude(r => r.TOutcomeResultDetails)
                .FirstOrDefaultAsync(o => o.ProjectId == projectId);

            if (existingEntity != null)
            {
                // นับจำนวน Child และ GrandChild
                int existingResultCount = existingEntity.TOutcomeResults.Count;
                int newResultCount = outcomeResult.TOutcomeResults?.Count ?? 0;

                int existingDetailCount = existingEntity.TOutcomeResults
                    .Sum(r => r.TOutcomeResultDetails.Count);
                int newDetailCount = outcomeResult.TOutcomeResults?
                    .Sum(r => r.TOutcomeResultDetails?.Count ?? 0) ?? 0;

                // ⚠️ เช็คจำนวน: ถ้าเท่ากันทั้ง Result และ Detail ไม่ต้อง update
                if (existingResultCount == newResultCount && existingDetailCount == newDetailCount)
                {
                    // Update เฉพาะ Master properties
                    existingEntity.ProjectCode = outcomeResult.ProjectCode;
                    existingEntity.ProjectName = outcomeResult.ProjectName;
                    existingEntity.Year = outcomeResult.Year;

                    await _context.SaveChangesAsync();
                    return; // ไม่ต้องลบ/เพิ่ม Child
                }

                // ถ้าจำนวนไม่เท่ากัน ให้ลบและ Insert ใหม่
                // ✅ ดึง OutcomeIds ก่อน แล้วใช้ ExecuteDelete โดยตรง (ไม่มี JOIN)
                var outcomeIds = existingEntity.TOutcomeResults
                    .Select(r => r.OutcomeId)
                    .ToList();

                if (outcomeIds.Any())
                {
                    // ลบ GrandChild โดยใช้ OutcomeIds (ไม่มี JOIN)
                    await _context.TOutcomeResultDetails
                        .Where(d => outcomeIds.Contains(d.OutcomeId))
                        .ExecuteDeleteAsync();
                }

                // ลบ Child (ใช้ ProjectId โดยตรง - ไม่มี JOIN)
                await _context.TOutcomeResults
                    .Where(r => r.ProjectId == projectId)
                    .ExecuteDeleteAsync();

                // ⚠️ สำคัญ: Reload entity หลัง ExecuteDelete เพื่อ sync กับ database
                await _context.Entry(existingEntity).Collection(e => e.TOutcomeResults).LoadAsync();

                // Update master properties
                existingEntity.ProjectCode = outcomeResult.ProjectCode;
                existingEntity.ProjectName = outcomeResult.ProjectName;
                existingEntity.Year = outcomeResult.Year;

                // เพิ่ม Child และ GrandChild ใหม่ทั้งหมด
                if (outcomeResult.TOutcomeResults != null && outcomeResult.TOutcomeResults.Any())
                {
                    foreach (var result in outcomeResult.TOutcomeResults)
                    {
                        // ⚠️ สำคัญ: Reset Identity columns เป็น 0
                        result.OutcomeId = 0;
                        result.ProjectId = projectId;
                        
                        // Reset GrandChild Identity columns
                        if (result.TOutcomeResultDetails != null && result.TOutcomeResultDetails.Any())
                        {
                            foreach (var detail in result.TOutcomeResultDetails)
                            {
                                detail.MonthlyOutcomeResultId = 0;
                                detail.OutcomeId = 0; // จะถูก set อัตโนมัติหลัง SaveChanges
                            }
                        }
                        
                        // ✅ เพิ่มผ่าน Navigation Property
                        existingEntity.TOutcomeResults.Add(result);
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
        var outcomeResult = await _context.MOutcomeResults.FindAsync(projectId);
        if (outcomeResult != null)
        {
            _context.MOutcomeResults.Remove(outcomeResult);
            await _context.SaveChangesAsync();
        }
    }
}
