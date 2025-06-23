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

    public async Task<MPerformanceResult?> GetByIdAsync(long? pProjectCode)
    {
        return await _context.MPerformanceResults
            .Include(p => p.TPerformanceResults)
            .ThenInclude(r => r.TPerformanceResultDetails)
            .FirstOrDefaultAsync(p => p.ProjectCode == pProjectCode);
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
            var trackedEntity = _context.MPerformanceResults.Local
                .FirstOrDefault(e => e.ProjectCode == performanceResult.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            _context.MPerformanceResults.Update(performanceResult);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log exception if needed
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
