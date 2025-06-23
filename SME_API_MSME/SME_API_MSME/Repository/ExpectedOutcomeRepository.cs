using Microsoft.EntityFrameworkCore;
using SME_API_MSME.Entities;

public class ExpectedOutcomeRepository
{
    private readonly MSMEDBContext _context;

    public ExpectedOutcomeRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MExpectedOutcome>> GetAllAsync()
    {
        return await _context.MExpectedOutcomes
            .Include(e => e.TExpectedFinalOutcomes)
            .ThenInclude(f => f.TExpectedFinalOutcomeMonthlyPlans)
            .ToListAsync();
    }

    public async Task<MExpectedOutcome?> GetByIdAsync(long? pProjectCode)
    {
        return await _context.MExpectedOutcomes
            .Include(e => e.TExpectedFinalOutcomes)
            .ThenInclude(f => f.TExpectedFinalOutcomeMonthlyPlans)
            .FirstOrDefaultAsync(e => e.ProjectCode == pProjectCode);
    }

    public async Task AddAsync(MExpectedOutcome expectedOutcome)
    {
        _context.MExpectedOutcomes.Add(expectedOutcome);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MExpectedOutcome expectedOutcome)
    {
        try
        {
            var existingOutcome = await _context.MExpectedOutcomes
                .FirstOrDefaultAsync(e => e.ProjectCode == expectedOutcome.ProjectCode && e.ProjectId != expectedOutcome.ProjectId);
            if (existingOutcome != null)
            {
                throw new InvalidOperationException($"A record with ProjectCode {expectedOutcome.ProjectCode} already exists.");
            }

            var trackedEntity = _context.MExpectedOutcomes.Local
                .FirstOrDefault(e => e.ProjectId == expectedOutcome.ProjectId);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            _context.MExpectedOutcomes.Update(expectedOutcome);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log exception
        }

    }

    public async Task DeleteAsync(int projectId)
    {
        var expectedOutcome = await _context.MExpectedOutcomes.FindAsync(projectId);
        if (expectedOutcome != null)
        {
            _context.MExpectedOutcomes.Remove(expectedOutcome);
            await _context.SaveChangesAsync();
        }
    }
}
