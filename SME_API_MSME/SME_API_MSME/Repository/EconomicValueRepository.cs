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
        return await _context.MEconomicValueProjects
            //  .Include(p => p.TEconomicValues)
            //.Include(p => p.TEconomicValueSheets2s)
            .ToListAsync();
    }

    public async Task<MEconomicValueProject?> GetByIdAsync(long? pProjectCode, int? year)
    {
        try
        {
            return await _context.MEconomicValueProjects
           //.Include(p => p.TEconomicValues)
           //.Include(p => p.TEconomicValueSheets2s)
           //    .ThenInclude(s => s.TEconomicPromoteds)
           //.Include(p => p.TEconomicValueSheets2s)
           //    .ThenInclude(s => s.TSmeEconomicDevelops)
           //.Include(p => p.TEconomicValueSheets2s)
           //    .ThenInclude(s => s.TSmeEconomicFactors)
           //.Include(p => p.TEconomicValueSheets2s)
           //    .ThenInclude(s => s.TSmeEconomicDevelopResults)
       .FirstOrDefaultAsync(e => e.BudgetYear == year && e.ProjectCode == pProjectCode);
        }
        catch (Exception ex)
        {
            return null;
        }

    }


    public async Task<MEconomicValueProject?> GetCheckByIdAsync(long? ProjectCode ,int year)
    {
        return await _context.MEconomicValueProjects

       .FirstOrDefaultAsync(e => e.BudgetYear == year && e.ProjectCode == ProjectCode);
    }
    public async Task AddAsync(MEconomicValueProject economicValue,List<TEconomicValue> tecom)
    {
        try
        {

            await _context.MEconomicValueProjects.AddAsync(economicValue);
            await _context.TEconomicValues.AddRangeAsync(tecom);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) { }

    }




    public async Task UpdateAsync(MEconomicValueProject economicValue,List<TEconomicValue> tecom)
    {
        try
        {
            _context.MEconomicValueProjects.Update(economicValue);
            _context.TEconomicValues.UpdateRange(tecom);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Handle exception (e.g., log it)  
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
    #region sheet2
    public async Task<TEconomicValueSheets2?> GetByIdSheet2Async(int sheetId)
    {
        return await _context.TEconomicValueSheets2s
            .Include(s => s.TEconomicPromoteds)
            .Include(s => s.TSmeEconomicDevelops)
            .Include(s => s.TSmeEconomicFactors)
            .Include(s => s.TSmeEconomicDevelopResults)
            .FirstOrDefaultAsync(s => s.SheetId == sheetId);
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

        }
    }
    public async Task UpdateSheet2Async(TEconomicValueSheets2 economicValue)
    {
        try
        {
            _context.TEconomicValueSheets2s.Update(economicValue);
            
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Handle exception (e.g., log it)  
        }
    }
    #endregion sheet2
}
