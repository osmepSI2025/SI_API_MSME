using Microsoft.EntityFrameworkCore;
using SME_API_MSME.Entities;
using SME_API_MSME.Models;

namespace SME_API_MSME.Repository
{
    public class ApiInformationRepository : IApiInformationRepository
    {
        private readonly MSMEDBContext _context;

        public ApiInformationRepository(MSMEDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MApiInformation>> GetAllAsync(MapiInformationModels param)
        {
            try
            {
                var query = _context.MApiInformations.AsQueryable();

                if (!string.IsNullOrEmpty(param.ServiceNameCode) && param.ServiceNameCode != "")
                    query = query.Where(p => p.ServiceNameCode == param.ServiceNameCode);
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<MApiInformation> GetByIdAsync(int id)
            => await _context.MApiInformations.FindAsync(id);

        public async Task AddAsync(MApiInformation service)
        {
            await _context.MApiInformations.AddAsync(service);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MApiInformation service)
        {
            _context.MApiInformations.Update(service);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var service = await _context.MApiInformations.FindAsync(id);
            if (service != null)
            {
                _context.MApiInformations.Remove(service);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateAllBearerTokensAsync(string newBearerToken)
        {
            try
            {
                // Retrieve all records from the repository
                var allRecords = await GetAllAsync(new MapiInformationModels());

                if (allRecords == null || !allRecords.Any())
                    throw new Exception("No records found to update.");

                // Update the Bearer field for each record
                foreach (var record in allRecords)
                {
                    record.Bearer = newBearerToken;
                    await UpdateAsync(record);
                }
            }
            catch (Exception ex)
            {
               
                throw new Exception("Error updating Bearer tokens: " + ex.Message, ex);
            }
        }
    }
}
