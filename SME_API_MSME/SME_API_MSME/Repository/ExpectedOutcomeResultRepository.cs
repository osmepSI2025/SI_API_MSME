using SME_API_MSME.Entities;
using Microsoft.EntityFrameworkCore;

public class ExpectedOutcomeResultRepository
{
    private readonly MSMEDBContext _context;

    public ExpectedOutcomeResultRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MExpectedOutcomeResult>> GetAllAsync()
    {
        return await _context.MExpectedOutcomeResults
            .Include(e => e.TExpectedOutcomeResults)
            .ThenInclude(r => r.TExpectedOutcomeResultDetails)
            .ToListAsync();
    }

    public async Task<MExpectedOutcomeResult?> GetByIdAsync(long? pProjectCode,string pYear)
    {
        return await _context.MExpectedOutcomeResults
            .Include(e => e.TExpectedOutcomeResults)
            .ThenInclude(r => r.TExpectedOutcomeResultDetails)
            .FirstOrDefaultAsync(e => e.ProjectCode == pProjectCode && e.Year ==pYear);
    }

    public async Task AddAsync(MExpectedOutcomeResult expectedOutcomeResult)
    {
        _context.MExpectedOutcomeResults.Add(expectedOutcomeResult);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MExpectedOutcomeResult expectedOutcomeResult)
    {
        try
        {
            var trackedEntity = _context.MExpectedOutcomeResults.Local
                .FirstOrDefault(e => e.ProjectCode == expectedOutcomeResult.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            _context.MExpectedOutcomeResults.Update(expectedOutcomeResult);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log exception if needed
        }

    }

    public async Task DeleteAsync(int projectId)
    {
        var expectedOutcomeResult = await _context.MExpectedOutcomeResults.FindAsync(projectId);
        if (expectedOutcomeResult != null)
        {
            _context.MExpectedOutcomeResults.Remove(expectedOutcomeResult);
            await _context.SaveChangesAsync();
        }
    }
}
