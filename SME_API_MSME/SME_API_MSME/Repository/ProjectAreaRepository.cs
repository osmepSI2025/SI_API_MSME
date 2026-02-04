using SME_API_MSME.Entities;
using Microsoft.EntityFrameworkCore;

public class ProjectAreaRepository
{
    private readonly MSMEDBContext _context;

    public ProjectAreaRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MProjectArea>> GetAllAsync()
    {
        return await _context.MProjectAreas.Include(p => p.TProjectAreas).ToListAsync();
    }

    public async Task<MProjectArea?> GetByIdAsync(long? projectCode, string year)
    {
        return await _context.MProjectAreas
            .Include(p => p.TProjectAreas)
            .FirstOrDefaultAsync(p => p.ProjectCode == projectCode && p.Year == year);
    }

    public async Task AddAsync(MProjectArea projectArea)
    {
        _context.MProjectAreas.Add(projectArea);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MProjectArea projectArea)
    {
        try
        {
            // Detach any tracked entities
            var trackedEntity = _context.MProjectAreas.Local
                .FirstOrDefault(e => e.ProjectCode == projectArea.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            var projectId = projectArea.ProjectId;

            // Get existing entity (ต้อง Include เพื่อ load navigation property และนับจำนวน)
            var existingEntity = await _context.MProjectAreas
                .Include(p => p.TProjectAreas)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (existingEntity != null)
            {
                // ⚠️ นับจำนวน Child records
                int existingAreaCount = existingEntity.TProjectAreas.Count;
                int newAreaCount = projectArea.TProjectAreas?.Count ?? 0;

                // ⚠️ เช็คจำนวน: ถ้าเท่ากัน ไม่ต้อง update Child
                if (existingAreaCount == newAreaCount)
                {
                    // Update เฉพาะ Master properties
                    existingEntity.ProjectCode = projectArea.ProjectCode;
                    existingEntity.ProjectName = projectArea.ProjectName;
                    existingEntity.Year = projectArea.Year;

                    await _context.SaveChangesAsync();
                    return; // ✅ ไม่ต้องลบ/เพิ่ม Child
                }

                // ถ้าจำนวนไม่เท่ากัน ให้ลบและ Insert ใหม่
                // ลบ Child records เก่าทั้งหมด (ใช้ ExecuteDelete - เร็วมาก)
                await _context.TProjectAreas
                    .Where(a => a.ProjectId == projectId)
                    .ExecuteDeleteAsync();

                // Reload collection หลัง Delete
                await _context.Entry(existingEntity).Collection(e => e.TProjectAreas).LoadAsync();

                // Update master properties
                existingEntity.ProjectCode = projectArea.ProjectCode;
                existingEntity.ProjectName = projectArea.ProjectName;
                existingEntity.Year = projectArea.Year;

                // เพิ่ม Child records ใหม่ทั้งหมด
                if (projectArea.TProjectAreas != null && projectArea.TProjectAreas.Any())
                {
                    foreach (var area in projectArea.TProjectAreas)
                    {
                        // ⚠️ สำคัญ: Reset Identity column เป็น 0
                        area.Id = 0;
                        area.ProjectId = projectId;

                        // ✅ เพิ่มผ่าน Navigation Property
                        existingEntity.TProjectAreas.Add(area);
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
        var projectArea = await _context.MProjectAreas.FindAsync(projectId);
        if (projectArea != null)
        {
            _context.MProjectAreas.Remove(projectArea);
            await _context.SaveChangesAsync();
        }
    }
}
