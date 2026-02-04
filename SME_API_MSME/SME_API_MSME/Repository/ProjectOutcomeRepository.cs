using SME_API_MSME.Entities;
using Microsoft.EntityFrameworkCore;

public class ProjectOutcomeRepository
{
    private readonly MSMEDBContext _context;

    public ProjectOutcomeRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MProjectsOutCome>> GetAllAsync()
    {
        return await _context.MProjectsOutComes.Include(p => p.TProjectsOutComes).ToListAsync();
    }

    public async Task<MProjectsOutCome?> GetByIdAsync(long? pProjectCode)
    {
        return await _context.MProjectsOutComes
            .Include(p => p.TProjectsOutComes)
            .FirstOrDefaultAsync(p => p.ProjectCode == pProjectCode);
    }

    public async Task AddAsync(MProjectsOutCome projectOutcome)
    {
        _context.MProjectsOutComes.Add(projectOutcome);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MProjectsOutCome projectOutcome)
    {
        try
        {
            // Detach any tracked entities
            var trackedEntity = _context.MProjectsOutComes.Local
                .FirstOrDefault(e => e.ProjectCode == projectOutcome.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            var projectId = projectOutcome.ProjectId;

            // Get existing entity with child records
            var existingEntity = await _context.MProjectsOutComes
                .Include(p => p.TProjectsOutComes)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (existingEntity != null)
            {
                // ⚠️ นับจำนวน Child records
                int existingOutcomeCount = existingEntity.TProjectsOutComes.Count;
                int newOutcomeCount = projectOutcome.TProjectsOutComes?.Count ?? 0;

                // ⚠️ เช็คจำนวน: ถ้าเท่ากัน ไม่ต้อง update Child
                if (existingOutcomeCount == newOutcomeCount)
                {
                    // Update เฉพาะ Master properties
                    existingEntity.ProjectCode = projectOutcome.ProjectCode;
                    existingEntity.ProjectName = projectOutcome.ProjectName;
                    existingEntity.Year = projectOutcome.Year;

                    await _context.SaveChangesAsync();
                    return; // ✅ ไม่ต้องลบ/เพิ่ม Child
                }

                // ถ้าจำนวนไม่เท่ากัน ให้ลบและ Insert ใหม่
                // ✅ ใช้ ExecuteDelete แทน RemoveRange (เร็วกว่า)
                await _context.TProjectsOutComes
                    .Where(o => o.ProjectId == projectId)
                    .ExecuteDeleteAsync();

                // Update master properties
                existingEntity.ProjectCode = projectOutcome.ProjectCode;
                existingEntity.ProjectName = projectOutcome.ProjectName;
                existingEntity.Year = projectOutcome.Year;

                // เพิ่ม Child records ใหม่ทั้งหมด
                if (projectOutcome.TProjectsOutComes != null && projectOutcome.TProjectsOutComes.Any())
                {
                    foreach (var item in projectOutcome.TProjectsOutComes)
                    {
                        item.OutcomeId = 0; // ✅ Reset Identity
                        item.ProjectId = existingEntity.ProjectId; // Set FK
                        existingEntity.TProjectsOutComes.Add(item);
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
        var projectOutcome = await _context.MProjectsOutComes.FindAsync(projectId);
        if (projectOutcome != null)
        {
            _context.MProjectsOutComes.Remove(projectOutcome);
            await _context.SaveChangesAsync();
        }
    }
}
