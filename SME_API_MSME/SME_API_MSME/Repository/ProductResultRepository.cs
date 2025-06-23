using SME_API_MSME.Entities;
using Microsoft.EntityFrameworkCore;

public class ProductResultRepository
{
    private readonly MSMEDBContext _context;

    public ProductResultRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MProductResult>> GetAllAsync()
    {
        return await _context.MProductResults
            .Include(p => p.TProductResultOutputs)
            .ThenInclude(o => o.TProductResultOutputDetails)
            .ToListAsync();
    }

    public async Task<MProductResult?> GetByIdAsync(long? projectCode)
    {
        return await _context.MProductResults
            .Include(p => p.TProductResultOutputs)
            .ThenInclude(o => o.TProductResultOutputDetails)
            .FirstOrDefaultAsync(p => p.ProjectCode == projectCode);
    }

    public async Task AddAsync(MProductResult productResult)
    {
        _context.MProductResults.Add(productResult);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MProductResult productResult)
    {
        try
        {
            var trackedEntity = _context.MProductResults.Local
                .FirstOrDefault(e => e.ProjectCode == productResult.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            _context.MProductResults.Update(productResult);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log exception if needed
        }

      
    }

    public async Task DeleteAsync(int projectId)
    {
        var productResult = await _context.MProductResults.FindAsync(projectId);
        if (productResult != null)
        {
            _context.MProductResults.Remove(productResult);
            await _context.SaveChangesAsync();
        }
    }
}
