using SME_API_MSME.Entities;
using Microsoft.EntityFrameworkCore;

public class ProjectProductRepository
{
    private readonly MSMEDBContext _context;

    public ProjectProductRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MProjectsProduct>> GetAllAsync()
    {
        return await _context.MProjectsProducts.Include(p => p.TProjectsProducts).ToListAsync();
    }

    public async Task<MProjectsProduct?> GetByIdAsync(long? pProjectCode,string pYear)
    {
        return await _context.MProjectsProducts
            .Include(p => p.TProjectsProducts)
            .FirstOrDefaultAsync(p => p.ProjectCode == pProjectCode && p.Year==pYear);
    }

    public async Task AddAsync(MProjectsProduct projectProduct)
    {
        _context.MProjectsProducts.Add(projectProduct);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MProjectsProduct projectProduct)
    {
        try
        {
            var trackedEntity = _context.MProjectsProducts.Local
                .FirstOrDefault(e => e.ProjectCode == projectProduct.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            _context.MProjectsProducts.Update(projectProduct);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log exception if needed
        }



   
    }

    public async Task DeleteAsync(int projectId)
    {
        var projectProduct = await _context.MProjectsProducts.FindAsync(projectId);
        if (projectProduct != null)
        {
            _context.MProjectsProducts.Remove(projectProduct);
            await _context.SaveChangesAsync();
        }
    }
}
