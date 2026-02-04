using SME_API_MSME.Entities;
using Microsoft.EntityFrameworkCore;

public class ProjectActivityRepository
{
    private readonly MSMEDBContext _context;

    public ProjectActivityRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MProjectsActivity>> GetAllAsync()
    {
        return await _context.MProjectsActivities
            .Include(p => p.TProjectActivities)
            .ThenInclude(a => a.TProjectActivityPlans)
            .ToListAsync();
    }

    public async Task<MProjectsActivity?> GetByIdAsync(long? pProjectCode, string pYear)
    {
        try
        {
            return await _context.MProjectsActivities
               .Include(p => p.TProjectActivities)
               .ThenInclude(a => a.TProjectActivityPlans)
               .FirstOrDefaultAsync(p => p.ProjectCode == pProjectCode);
        }
        catch (Exception ex)
        {
            return new MProjectsActivity();
        }
    }

    public async Task AddAsync(MProjectsActivity projectActivity)
    {
        _context.MProjectsActivities.Add(projectActivity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MProjectsActivity projectActivity)
    {
        try
        {
            // Detach any tracked entities
            var trackedEntity = _context.MProjectsActivities.Local
                .FirstOrDefault(e => e.ProjectCode == projectActivity.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            var projectId = projectActivity.ProjectId;

            // Get existing entity with all child and grandchild records
            var existingEntity = await _context.MProjectsActivities
                .Include(p => p.TProjectActivities)
                .ThenInclude(a => a.TProjectActivityPlans)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (existingEntity != null)
            {
                // ⚠️ นับจำนวน Child และ GrandChild
                int existingActivityCount = existingEntity.TProjectActivities.Count;
                int newActivityCount = projectActivity.TProjectActivities?.Count ?? 0;

                int existingPlanCount = existingEntity.TProjectActivities
                    .Sum(a => a.TProjectActivityPlans.Count);
                int newPlanCount = projectActivity.TProjectActivities?
                    .Sum(a => a.TProjectActivityPlans?.Count ?? 0) ?? 0;

                // ⚠️ เช็คจำนวน: ถ้าเท่ากันทั้ง Activity และ Plan ไม่ต้อง update
                if (existingActivityCount == newActivityCount && existingPlanCount == newPlanCount)
                {
                    // Update เฉพาะ Master properties
                    existingEntity.ProjectCode = projectActivity.ProjectCode;
                    existingEntity.ProjectName = projectActivity.ProjectName;
                    existingEntity.Year = projectActivity.Year;

                    await _context.SaveChangesAsync();
                    return; // ✅ ไม่ต้องลบ/เพิ่ม Child
                }

                // ถ้าจำนวนไม่เท่ากัน ให้ลบและ Insert ใหม่
                // ลบ GrandChild ก่อน (ใช้ Raw SQL - เร็วมาก)
                await _context.Database.ExecuteSqlRawAsync(
                    @"DELETE FROM T_ProjectActivityPlans 
                      WHERE ActivityId IN (
                          SELECT ActivityId FROM T_ProjectActivities WHERE ProjectId = {0}
                      )", projectId);

                // ลบ Child (ใช้ Raw SQL - เร็วมาก)
                await _context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM T_ProjectActivities WHERE ProjectId = {0}", projectId);

                // Reload entity หลัง Delete
                await _context.Entry(existingEntity).Collection(e => e.TProjectActivities).LoadAsync();

                // Update master properties
                existingEntity.ProjectCode = projectActivity.ProjectCode;
                existingEntity.ProjectName = projectActivity.ProjectName;
                existingEntity.Year = projectActivity.Year;

                // เพิ่ม Child และ GrandChild ใหม่ผ่าน Navigation Property
                if (projectActivity.TProjectActivities != null && projectActivity.TProjectActivities.Any())
                {
                    foreach (var activity in projectActivity.TProjectActivities)
                    {
                        // ⚠️ สำคัญ: Reset Identity columns เป็น 0
                        activity.ActivityId = 0;
                        activity.ProjectId = projectId;
                        
                        // Reset GrandChild Identity columns
                        if (activity.TProjectActivityPlans != null && activity.TProjectActivityPlans.Any())
                        {
                            foreach (var plan in activity.TProjectActivityPlans)
                            {
                                plan.PlanId = 0;
                                plan.ActivityId = 0; // จะถูก set อัตโนมัติหลัง SaveChanges
                            }
                        }
                        
                        // ✅ เพิ่มผ่าน Navigation Property
                        existingEntity.TProjectActivities.Add(activity);
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
        var projectActivity = await _context.MProjectsActivities.FindAsync(projectId);
        if (projectActivity != null)
        {
            _context.MProjectsActivities.Remove(projectActivity);
            await _context.SaveChangesAsync();
        }
    }
}
