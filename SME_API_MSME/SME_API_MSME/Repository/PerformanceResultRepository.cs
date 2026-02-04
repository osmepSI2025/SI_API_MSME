using SME_API_MSME.Entities;
using Microsoft.EntityFrameworkCore;

public class PerformanceResultRepository
{
    private readonly MSMEDBContext _context;

    public PerformanceResultRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MPerformanceResult>> GetAllAsync()
    {
        return await _context.MPerformanceResults
            .Include(p => p.TPerformanceResults)
            .ThenInclude(r => r.TPerformanceResultDetails)
            .ToListAsync();
    }

    public async Task<MPerformanceResult?> GetByIdAsync(long? pProjectCode, string pYear)
    {
        return await _context.MPerformanceResults
            .Include(p => p.TPerformanceResults)
            .ThenInclude(r => r.TPerformanceResultDetails)
            .FirstOrDefaultAsync(p => p.ProjectCode == pProjectCode && p.Year == pYear);
    }

    public async Task AddAsync(MPerformanceResult performanceResult)
    {
        _context.MPerformanceResults.Add(performanceResult);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MPerformanceResult performanceResult)
    {
        try
        {
            // Detach any tracked entities
            var trackedEntity = _context.MPerformanceResults.Local
                .FirstOrDefault(e => e.ProjectCode == performanceResult.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            var projectId = performanceResult.ProjectId;

            // Get existing entity with all child and grandchild records
            var existingEntity = await _context.MPerformanceResults
                .Include(p => p.TPerformanceResults)
                .ThenInclude(r => r.TPerformanceResultDetails)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (existingEntity != null)
            {
                // นับจำนวน Child และ GrandChild
                int existingResultCount = existingEntity.TPerformanceResults.Count;
                int newResultCount = performanceResult.TPerformanceResults?.Count ?? 0;

                int existingDetailCount = existingEntity.TPerformanceResults
                    .Sum(r => r.TPerformanceResultDetails.Count);
                int newDetailCount = performanceResult.TPerformanceResults?
                    .Sum(r => r.TPerformanceResultDetails?.Count ?? 0) ?? 0;

                // ⚠️ เช็คจำนวน: ถ้าเท่ากันทั้ง Result และ Detail ไม่ต้อง update
                if (existingResultCount == newResultCount && existingDetailCount == newDetailCount)
                {
                    // Update เฉพาะ Master properties
                    existingEntity.ProjectCode = performanceResult.ProjectCode;
                    existingEntity.ProjectName = performanceResult.ProjectName;
                    existingEntity.Year = performanceResult.Year;

                    await _context.SaveChangesAsync();
                    return; // ไม่ต้องลบ/เพิ่ม Child
                }

                // ถ้าจำนวนไม่เท่ากัน ให้ลบและ Insert ใหม่
                // ลบ GrandChild ก่อน (ใช้ ExecuteDelete - เร็วมาก)
                await _context.TPerformanceResultDetails
                    .Where(d => d.Activity.ProjectId == projectId)
                    .ExecuteDeleteAsync();

                // ลบ Child (ใช้ ExecuteDelete - เร็วมาก)
                await _context.TPerformanceResults
                    .Where(r => r.ProjectId == projectId)
                    .ExecuteDeleteAsync();

                // ⚠️ สำคัญ: Reload entity หลัง ExecuteDelete เพื่อ sync กับ database
                await _context.Entry(existingEntity).Collection(e => e.TPerformanceResults).LoadAsync();

                // Update master properties
                existingEntity.ProjectCode = performanceResult.ProjectCode;
                existingEntity.ProjectName = performanceResult.ProjectName;
                existingEntity.Year = performanceResult.Year;

                // เพิ่ม Child และ GrandChild ใหม่ทั้งหมด
                if (performanceResult.TPerformanceResults != null && performanceResult.TPerformanceResults.Any())
                {
                    foreach (var result in performanceResult.TPerformanceResults)
                    {
                        // ⚠️ สำคัญ: Reset Identity columns เป็น 0
                        result.ActivityId = 0;
                        result.ProjectId = projectId;
                        
                        // Reset GrandChild Identity columns
                        if (result.TPerformanceResultDetails != null && result.TPerformanceResultDetails.Any())
                        {
                            foreach (var detail in result.TPerformanceResultDetails)
                            {
                                detail.MonthlyActivityResultId = 0;
                                detail.ActivityId = 0; // จะถูก set อัตโนมัติหลัง SaveChanges
                            }
                        }
                        
                        // ✅ เพิ่มผ่าน Navigation Property
                        existingEntity.TPerformanceResults.Add(result);
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
        var performanceResult = await _context.MPerformanceResults.FindAsync(projectId);
        if (performanceResult != null)
        {
            _context.MPerformanceResults.Remove(performanceResult);
            await _context.SaveChangesAsync();
        }
    }
}
