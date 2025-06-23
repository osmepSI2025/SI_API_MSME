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

    public async Task<MProjectsActivity?> GetByIdAsync(long? pProjectCode)
    {
        try {
            return await _context.MProjectsActivities
               .Include(p => p.TProjectActivities)
               .ThenInclude(a => a.TProjectActivityPlans)
               .FirstOrDefaultAsync(p => p.ProjectCode == pProjectCode);
        } catch (Exception ex) 
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
            var trackedEntity = _context.MProjectsActivities.Local
                .FirstOrDefault(e => e.ProjectCode == projectActivity.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            _context.MProjectsActivities.Update(projectActivity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log exception if needed
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
