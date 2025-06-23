using SME_API_MSME.Entities;
using Microsoft.EntityFrameworkCore;

public class ProjectRepository
{
    private readonly MSMEDBContext _context;

    public ProjectRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MProject>> GetAllAsync()
    {
        return await _context.MProjects.ToListAsync();
    }

    public async Task<IEnumerable<MProject>?> GetByIdAsync(string year)
    {
       
        return await _context.MProjects.Where(e => e.BudgetYear == year).ToListAsync();
    }

    public async Task AddAsync(MProject project)
    {
        _context.MProjects.Add(project);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MProject project)
    {
        try
        {
            var trackedEntity = _context.MProjects.Local
                .FirstOrDefault(e => e.ProjectCode == project.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            _context.MProjects.Update(project);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log exception if needed
        }


        
    }

    public async Task DeleteAsync(int id)
    {
        var project = await _context.MProjects.FindAsync(id);
        if (project != null)
        {
            _context.MProjects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }
}
