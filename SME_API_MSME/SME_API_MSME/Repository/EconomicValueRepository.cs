using Microsoft.EntityFrameworkCore;
using SME_API_MSME.Entities;

public class EconomicValueRepository
{
    private readonly MSMEDBContext _context;

    public EconomicValueRepository(MSMEDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MEconomicValueProject>> GetAllAsync()
    {
        return await _context.MEconomicValueProjects.ToListAsync();
    }

    public async Task<MEconomicValueProject?> GetByIdAsync(long? pProjectCode, int? year)
    {
        try
        {
            return await _context.MEconomicValueProjects
                .FirstOrDefaultAsync(e => e.BudgetYear == year && e.ProjectCode == pProjectCode);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<MEconomicValueProject?> GetCheckByIdAsync(long? ProjectCode, int year)
    {
        return await _context.MEconomicValueProjects
            .FirstOrDefaultAsync(e => e.BudgetYear == year && e.ProjectCode == ProjectCode);
    }

    public async Task AddAsync(MEconomicValueProject economicValue, List<TEconomicValue> tecom)
    {
        try
        {
            await _context.MEconomicValueProjects.AddAsync(economicValue);
            
            // Set ProjectCode for all TEconomicValue
            foreach (var item in tecom)
            {
                item.ProjectCode = economicValue.ProjectCode;
            }
            await _context.TEconomicValues.AddRangeAsync(tecom);
            
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task UpdateAsync(MEconomicValueProject economicValue, List<TEconomicValue> tecom)
    {
        try
        {
            var existingProject = await _context.MEconomicValueProjects
                .FirstOrDefaultAsync(e => e.ProjectCode == economicValue.ProjectCode && e.BudgetYear == economicValue.BudgetYear);

            if (existingProject != null)
            {
                existingProject.ProjectName = economicValue.ProjectName;
                existingProject.Budget = economicValue.Budget;
                existingProject.BudgetYear = economicValue.BudgetYear;

                // ลบ TEconomicValue เก่าทั้งหมด
                await _context.TEconomicValues
                    .Where(e => e.ProjectCode == economicValue.ProjectCode)
                    .ExecuteDeleteAsync();

                // เพิ่ม TEconomicValue ใหม่
                if (tecom != null && tecom.Any())
                {
                    foreach (var item in tecom)
                    {
                        item.ProjectCode = economicValue.ProjectCode;
                        item.EconomicValueId = item.EconomicValueId; // Reset Identity
                    }
                    await _context.TEconomicValues.AddRangeAsync(tecom);
                }

                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task DeleteAsync(int projectCode)
    {
        var economicValue = await _context.MEconomicValueProjects.FirstOrDefaultAsync(e => e.ProjectCode == projectCode);
        if (economicValue != null)
        {
            _context.MEconomicValueProjects.Remove(economicValue);
            await _context.SaveChangesAsync();
        }
    }

    #region Sheet2
    public async Task<TEconomicValueSheets2?> GetByIdSheet2Async(long? pProjectCode)
    {
        return await _context.TEconomicValueSheets2s
            .Include(s => s.TEconomicPromoteds)
            .Include(s => s.TSmeEconomicDevelops)
            .Include(s => s.TSmeEconomicFactors)
            .Include(s => s.TSmeEconomicDevelopResults)
            .FirstOrDefaultAsync(s => s.ProjectCode == pProjectCode);
    }

    public async Task AddSheet2Async(TEconomicValueSheets2 economicValue)
    {
        try
        {
            await _context.TEconomicValueSheets2s.AddAsync(economicValue);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task UpdateSheet2Async(TEconomicValueSheets2 economicValue)
    {
        try
        {
            // Detach any tracked entities
            var trackedEntity = _context.TEconomicValueSheets2s.Local
                .FirstOrDefault(e => e.ProjectCode == economicValue.ProjectCode);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            var projectCode = economicValue.ProjectCode;

            // Get existing entity with all children
            var existingEntity = await _context.TEconomicValueSheets2s
                .Include(s => s.TEconomicPromoteds)
                .Include(s => s.TSmeEconomicDevelops)
                .Include(s => s.TSmeEconomicFactors)
                .Include(s => s.TSmeEconomicDevelopResults)
                .FirstOrDefaultAsync(s => s.ProjectCode == projectCode);

            if (existingEntity != null)
            {
                var sheetId = existingEntity.SheetId;

                // ลบ Children ทั้งหมด (ใช้ ExecuteDelete)
                await _context.TEconomicPromoteds
                    .Where(e => e.SheetId == sheetId)
                    .ExecuteDeleteAsync();

                await _context.TSmeEconomicDevelops
                    .Where(e => e.SheetId == sheetId)
                    .ExecuteDeleteAsync();

                await _context.TSmeEconomicFactors
                    .Where(e => e.SheetId == sheetId)
                    .ExecuteDeleteAsync();

                await _context.TSmeEconomicDevelopResults
                    .Where(e => e.SheetId == sheetId)
                    .ExecuteDeleteAsync();

                // Reload collections
                await _context.Entry(existingEntity).Collection(e => e.TEconomicPromoteds).LoadAsync();
                await _context.Entry(existingEntity).Collection(e => e.TSmeEconomicDevelops).LoadAsync();
                await _context.Entry(existingEntity).Collection(e => e.TSmeEconomicFactors).LoadAsync();
                await _context.Entry(existingEntity).Collection(e => e.TSmeEconomicDevelopResults).LoadAsync();

                // Update master properties
                existingEntity.Province = economicValue.Province;
                existingEntity.InterestedBusiness = economicValue.InterestedBusiness;

                // เพิ่ม Children ใหม่
                if (economicValue.TEconomicPromoteds != null && economicValue.TEconomicPromoteds.Any())
                {
                    foreach (var item in economicValue.TEconomicPromoteds)
                    {
                        item.PromotedId = 0; // Reset Identity
                        item.SheetId = sheetId;
                        existingEntity.TEconomicPromoteds.Add(item);
                    }
                }

                if (economicValue.TSmeEconomicDevelops != null && economicValue.TSmeEconomicDevelops.Any())
                {
                    foreach (var item in economicValue.TSmeEconomicDevelops)
                    {
                        item.DevelopId = 0; // Reset Identity
                        item.SheetId = sheetId;
                        existingEntity.TSmeEconomicDevelops.Add(item);
                    }
                }

                if (economicValue.TSmeEconomicFactors != null && economicValue.TSmeEconomicFactors.Any())
                {
                    foreach (var item in economicValue.TSmeEconomicFactors)
                    {
                        item.FactorId = 0; // Reset Identity
                        item.SheetId = sheetId;
                        existingEntity.TSmeEconomicFactors.Add(item);
                    }
                }

                if (economicValue.TSmeEconomicDevelopResults != null && economicValue.TSmeEconomicDevelopResults.Any())
                {
                    foreach (var item in economicValue.TSmeEconomicDevelopResults)
                    {
                        item.ResultId = 0; // Reset Identity
                        item.SheetId = sheetId;
                        existingEntity.TSmeEconomicDevelopResults.Add(item);
                    }
                }

                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    #endregion

    #region Get Sheet1
    public async Task<IEnumerable<TEconomicValue>> GetTEconomicAsync(long? pProjectCode)
    {
        return await _context.TEconomicValues
            .Where(e => e.ProjectCode == pProjectCode)
            .ToListAsync();
    }
    #endregion
}
