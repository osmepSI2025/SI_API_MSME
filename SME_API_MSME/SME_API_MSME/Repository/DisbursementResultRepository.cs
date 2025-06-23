using SME_API_MSME.Entities;
using Microsoft.EntityFrameworkCore;

public class DisbursementResultRepository
{
    private readonly MSMEDBContext _context;

    public DisbursementResultRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MDisbursementResult>> GetAllAsync()
    {
        return await _context.MDisbursementResults
            .Include(d => d.TDisbursementResults)
            .ThenInclude(r => r.TDisbursementResultDetails)
            .ToListAsync();
    }

    public async Task<MDisbursementResult?> GetByIdAsync(long? pProjectCode)
    {
        return await _context.MDisbursementResults
            .Include(d => d.TDisbursementResults)
            .ThenInclude(r => r.TDisbursementResultDetails)
            .FirstOrDefaultAsync(d => d.ProjectCode == pProjectCode);
    }

    public async Task AddAsync(MDisbursementResult disbursementResult)
    {
        _context.MDisbursementResults.Add(disbursementResult);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MDisbursementResult disbursementResult)
    {
        _context.MDisbursementResults.Update(disbursementResult);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int projectId)
    {
        var disbursementResult = await _context.MDisbursementResults.FindAsync(projectId);
        if (disbursementResult != null)
        {
            _context.MDisbursementResults.Remove(disbursementResult);
            await _context.SaveChangesAsync();
        }
    }
}
