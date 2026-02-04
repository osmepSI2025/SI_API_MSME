using SME_API_MSME.Entities;
using Microsoft.EntityFrameworkCore;

public class DisbursementResultRepository
{
    private readonly MSMEDBContext _context;

    public DisbursementResultRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MDisbursementResult>> GetAllAsync()
    {
        return await _context.MDisbursementResults
            .Include(d => d.TDisbursementResults)
            .ThenInclude(r => r.TDisbursementResultDetails)
            .ToListAsync();
    }   

    public async Task<MDisbursementResult?> GetByIdAsync(long? pProjectCode, string pYear)
    {
        return await _context.MDisbursementResults
            .Include(d => d.TDisbursementResults)
            .ThenInclude(r => r.TDisbursementResultDetails)
            .FirstOrDefaultAsync(d => d.ProjectCode == pProjectCode && d.Year == pYear);
    }

    public async Task AddAsync(MDisbursementResult disbursementResult)
    {
        try
        {
            // Ensure child collection is not null
            if (disbursementResult.TDisbursementResults == null)
            {
                disbursementResult.TDisbursementResults = new List<TDisbursementResult>();
            }

            _context.MDisbursementResults.Add(disbursementResult);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Consider logging the exception here
            throw;
        }
    }

    public async Task UpdateAsync(MDisbursementResult disbursementResult)
    {
        try
        {
            // Detach any tracked entities
            var trackedEntity = _context.MDisbursementResults.Local
                .FirstOrDefault(e => e.ProjectCode == disbursementResult.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            var projectId = disbursementResult.ProjectId;

            // Get existing entity with all child and grandchild records
            var existingEntity = await _context.MDisbursementResults
                .Include(d => d.TDisbursementResults)
                .ThenInclude(r => r.TDisbursementResultDetails)
                .FirstOrDefaultAsync(d => d.ProjectId == projectId);

            if (existingEntity != null)
            {
                // นับจำนวน Child และ GrandChild
                int existingResultCount = existingEntity.TDisbursementResults.Count;
                int newResultCount = disbursementResult.TDisbursementResults?.Count ?? 0;

                int existingDetailCount = existingEntity.TDisbursementResults
                    .Sum(r => r.TDisbursementResultDetails.Count);
                int newDetailCount = disbursementResult.TDisbursementResults?
                    .Sum(r => r.TDisbursementResultDetails?.Count ?? 0) ?? 0;

                // ⚠️ เช็คจำนวน: ถ้าเท่ากันทั้ง Result และ Detail ไม่ต้อง update
                if (existingResultCount == newResultCount && existingDetailCount == newDetailCount)
                {
                    // Update เฉพาะ Master properties
                    existingEntity.ProjectCode = disbursementResult.ProjectCode;
                    existingEntity.ProjectName = disbursementResult.ProjectName;
                    existingEntity.Year = disbursementResult.Year;

                    await _context.SaveChangesAsync();
                    return; // ไม่ต้องลบ/เพิ่ม Child
                }

                // ถ้าจำนวนไม่เท่ากัน ให้ลบและ Insert ใหม่
                // ✅ ดึง BudgetActivityIds ก่อน แล้วใช้ ExecuteDelete โดยตรง (ไม่มี JOIN)
                var budgetActivityIds = existingEntity.TDisbursementResults
                    .Select(r => r.BudgetActivityId)
                    .ToList();

                if (budgetActivityIds.Any())
                {
                    // ลบ GrandChild โดยใช้ BudgetActivityIds (ไม่มี JOIN)
                    await _context.TDisbursementResultDetails
                        .Where(d => budgetActivityIds.Contains(d.BudgetActivityId))
                        .ExecuteDeleteAsync();
                }

                // ลบ Child (ใช้ ProjectId โดยตรง - ไม่มี JOIN)
                await _context.TDisbursementResults
                    .Where(r => r.ProjectId == projectId)
                    .ExecuteDeleteAsync();

                // ⚠️ สำคัญ: Reload entity หลัง ExecuteDelete เพื่อ sync กับ database
                await _context.Entry(existingEntity).Collection(e => e.TDisbursementResults).LoadAsync();

                // Update master properties
                existingEntity.ProjectCode = disbursementResult.ProjectCode;
                existingEntity.ProjectName = disbursementResult.ProjectName;
                existingEntity.Year = disbursementResult.Year;

                // เพิ่ม Child และ GrandChild ใหม่ทั้งหมด
                if (disbursementResult.TDisbursementResults != null && disbursementResult.TDisbursementResults.Any())
                {
                    foreach (var result in disbursementResult.TDisbursementResults)
                    {
                        // ⚠️ สำคัญ: Reset Identity columns เป็น 0
                        result.BudgetActivityId = 0;
                        result.ProjectId = projectId;
                        
                        // Reset GrandChild Identity columns
                        if (result.TDisbursementResultDetails != null && result.TDisbursementResultDetails.Any())
                        {
                            foreach (var detail in result.TDisbursementResultDetails)
                            {
                                detail.MonthlyDisbursementId = 0;
                                detail.BudgetActivityId = 0; // จะถูก set อัตโนมัติหลัง SaveChanges
                            }
                        }
                        
                        // ✅ เพิ่มผ่าน Navigation Property
                        existingEntity.TDisbursementResults.Add(result);
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
        var disbursementResult = await _context.MDisbursementResults.FindAsync(projectId);
        if (disbursementResult != null)
        {
            _context.MDisbursementResults.Remove(disbursementResult);
            await _context.SaveChangesAsync();
        }
    }
}
