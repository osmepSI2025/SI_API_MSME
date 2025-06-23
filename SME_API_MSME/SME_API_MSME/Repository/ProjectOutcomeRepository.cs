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
            var trackedEntity = _context.MProjectsOutComes.Local
                .FirstOrDefault(e => e.ProjectCode == projectOutcome.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            _context.MProjectsOutComes.Update(projectOutcome);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log exception if needed
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
