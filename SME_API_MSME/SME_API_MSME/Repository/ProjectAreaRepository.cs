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

    public async Task<MProjectArea?> GetByIdAsync(long? projectCode)
    {
        return await _context.MProjectAreas
            .Include(p => p.TProjectAreas)
            .FirstOrDefaultAsync(p => p.ProjectCode == projectCode);
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
            var trackedEntity = _context.MProjectAreas.Local
                .FirstOrDefault(e => e.ProjectCode == projectArea.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }


            _context.MProjectAreas.Update(projectArea);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log exception if needed
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
