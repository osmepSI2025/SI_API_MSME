using SME_API_MSME.Entities;
using Microsoft.EntityFrameworkCore;

public class ProductResultRepository
{
    private readonly MSMEDBContext _context;

    public ProductResultRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MProductResult>> GetAllAsync()
    {
        return await _context.MProductResults
            .Include(p => p.TProductResultOutputs)
            .ThenInclude(o => o.TProductResultOutputDetails)
            .ToListAsync();
    }

    public async Task<MProductResult?> GetByIdAsync(long? projectCode, string pYear)
    {
        return await _context.MProductResults
            .Include(p => p.TProductResultOutputs)
            .ThenInclude(o => o.TProductResultOutputDetails)
            .FirstOrDefaultAsync(p => p.ProjectCode == projectCode && p.Year == pYear);
    }

    public async Task AddAsync(MProductResult productResult)
    {
        _context.MProductResults.Add(productResult);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MProductResult productResult)
    {
        try
        {
            // Detach any tracked entities
            var trackedEntity = _context.MProductResults.Local
                .FirstOrDefault(e => e.ProjectCode == productResult.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            var projectId = productResult.ProjectId;

            // Get existing entity with all child and grandchild records
            var existingEntity = await _context.MProductResults
                .Include(p => p.TProductResultOutputs)
                .ThenInclude(o => o.TProductResultOutputDetails)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (existingEntity != null)
            {
                // นับจำนวน Child และ GrandChild
                int existingOutputCount = existingEntity.TProductResultOutputs.Count;
                int newOutputCount = productResult.TProductResultOutputs?.Count ?? 0;

                int existingDetailCount = existingEntity.TProductResultOutputs
                    .Sum(o => o.TProductResultOutputDetails.Count);
                int newDetailCount = productResult.TProductResultOutputs?
                    .Sum(o => o.TProductResultOutputDetails?.Count ?? 0) ?? 0;

                // ⚠️ เช็คจำนวน: ถ้าเท่ากันทั้ง Output และ Detail ไม่ต้อง update
                if (existingOutputCount == newOutputCount && existingDetailCount == newDetailCount)
                {
                    // Update เฉพาะ Master properties
                    existingEntity.ProjectCode = productResult.ProjectCode;
                    existingEntity.ProjectName = productResult.ProjectName;
                    existingEntity.Year = productResult.Year;

                    await _context.SaveChangesAsync();
                    return; // ไม่ต้องลบ/เพิ่ม Child
                }

                // ถ้าจำนวนไม่เท่ากัน ให้ลบและ Insert ใหม่
                // ลบ GrandChild ก่อน (ใช้ ExecuteDelete - เร็วมาก)
                await _context.TProductResultOutputDetails
                    .Where(d => d.Output.ProjectId == projectId)
                    .ExecuteDeleteAsync();

                // ลบ Child (ใช้ ExecuteDelete - เร็วมาก)
                await _context.TProductResultOutputs
                    .Where(o => o.ProjectId == projectId)
                    .ExecuteDeleteAsync();

                // ⚠️ สำคัญ: Reload entity หลัง ExecuteDelete เพื่อ sync กับ database
                await _context.Entry(existingEntity).Collection(e => e.TProductResultOutputs).LoadAsync();

                // Update master properties
                existingEntity.ProjectCode = productResult.ProjectCode;
                existingEntity.ProjectName = productResult.ProjectName;
                existingEntity.Year = productResult.Year;

                // เพิ่ม Child และ GrandChild ใหม่ทั้งหมด
                if (productResult.TProductResultOutputs != null && productResult.TProductResultOutputs.Any())
                {
                    foreach (var output in productResult.TProductResultOutputs)
                    {
                        // ⚠️ สำคัญ: Reset Identity columns เป็น 0
                        output.OutputId = 0;
                        output.ProjectId = projectId;
                        
                        // Reset GrandChild Identity columns
                        if (output.TProductResultOutputDetails != null && output.TProductResultOutputDetails.Any())
                        {
                            foreach (var detail in output.TProductResultOutputDetails)
                            {
                                detail.MonthlyResultId = 0;
                                detail.OutputId = 0; // จะถูก set อัตโนมัติหลัง SaveChanges
                            }
                        }
                        
                        // ✅ เพิ่มผ่าน Navigation Property
                        existingEntity.TProductResultOutputs.Add(output);
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
        var productResult = await _context.MProductResults.FindAsync(projectId);
        if (productResult != null)
        {
            _context.MProductResults.Remove(productResult);
            await _context.SaveChangesAsync();
        }
    }
}
