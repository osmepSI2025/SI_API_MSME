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

    public async Task<MProjectsProduct?> GetByIdAsync(long? pProjectCode, string pYear)
    {
        return await _context.MProjectsProducts
            .Include(p => p.TProjectsProducts)
            .FirstOrDefaultAsync(p => p.ProjectCode == pProjectCode && p.Year == pYear);
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
            // Detach any tracked entities
            var trackedEntity = _context.MProjectsProducts.Local
                .FirstOrDefault(e => e.ProjectCode == projectProduct.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            var projectId = projectProduct.ProjectId;

            // Get existing entity with child records
            var existingEntity = await _context.MProjectsProducts
                .Include(p => p.TProjectsProducts)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (existingEntity != null)
            {
                // ⚠️ นับจำนวน Child records
                int existingProductCount = existingEntity.TProjectsProducts.Count;
                int newProductCount = projectProduct.TProjectsProducts?.Count ?? 0;

                // ⚠️ เช็คจำนวน: ถ้าเท่ากัน ไม่ต้อง update Child
                if (existingProductCount == newProductCount)
                {
                    // Update เฉพาะ Master properties
                    existingEntity.ProjectCode = projectProduct.ProjectCode;
                    existingEntity.ProjectName = projectProduct.ProjectName;
                    existingEntity.Year = projectProduct.Year;

                    await _context.SaveChangesAsync();
                    return; // ✅ ไม่ต้องลบ/เพิ่ม Child
                }

                // ถ้าจำนวนไม่เท่ากัน ให้ลบและ Insert ใหม่
                // ✅ ใช้ ExecuteDelete แทน RemoveRange (เร็วกว่า)
                await _context.TProjectsProducts
                    .Where(p => p.ProjectId == projectId)
                    .ExecuteDeleteAsync();

                // Update master properties
                existingEntity.ProjectCode = projectProduct.ProjectCode;
                existingEntity.ProjectName = projectProduct.ProjectName;
                existingEntity.Year = projectProduct.Year;

                // เพิ่ม Child records ใหม่ทั้งหมด
                if (projectProduct.TProjectsProducts != null && projectProduct.TProjectsProducts.Any())
                {
                    foreach (var item in projectProduct.TProjectsProducts)
                    {
                        item.OutputId = 0; // ✅ Reset Identity
                        item.ProjectId = existingEntity.ProjectId; // Set FK
                        existingEntity.TProjectsProducts.Add(item);
                    }
                }

                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            // Log exception if needed
            throw;
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
