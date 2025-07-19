using SME_API_MSME.Entities;
using Microsoft.EntityFrameworkCore;

public class OutcomeResultRepository
{
    private readonly MSMEDBContext _context;

    public OutcomeResultRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MOutcomeResult>> GetAllAsync()
    {
        return await _context.MOutcomeResults
            .Include(o => o.TOutcomeResults)
            .ThenInclude(r => r.TOutcomeResultDetails)
            .ToListAsync();
    }

    public async Task<MOutcomeResult?> GetByIdAsync(long? pProjectCode,string pYear)
    {
        return await _context.MOutcomeResults
            .Include(o => o.TOutcomeResults)
            .ThenInclude(r => r.TOutcomeResultDetails)
            .FirstOrDefaultAsync(o => o.ProjectCode == pProjectCode && o.Year ==pYear);
    }

    public async Task AddAsync(MOutcomeResult outcomeResult)
    {
        _context.MOutcomeResults.Add(outcomeResult);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MOutcomeResult outcomeResult)
    {
        try
        {
            var trackedEntity = _context.MOutcomeResults.Local
                .FirstOrDefault(e => e.ProjectCode == outcomeResult.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            _context.MOutcomeResults.Update(outcomeResult);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log exception if needed
        }

       
    }

    public async Task DeleteAsync(int projectId)
    {
        var outcomeResult = await _context.MOutcomeResults.FindAsync(projectId);
        if (outcomeResult != null)
        {
            _context.MOutcomeResults.Remove(outcomeResult);
            await _context.SaveChangesAsync();
        }
    }
}
