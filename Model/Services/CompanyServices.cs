using Microsoft.EntityFrameworkCore;

namespace Model.Services
{
    public interface ICompanyServices
    {
        public Task<List<Company>> GeCompanyList();
        public Task<Company> GetCompanyById(int id);
        public Task<Company> AddCompany(Company oCompany);
        public Task<Company> UpdateCompany(Company oCompany);
    }

    public class CompanyServices : ICompanyServices
    {

        private readonly PostgresDbContext _dbContext;
        public CompanyServices(PostgresDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Company>> GeCompanyList()
        {
            return await _dbContext.Company.ToListAsync();
        }

        public async Task<Company> GetCompanyById(int id)
        {
            return await _dbContext.Company.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Company> AddCompany(Company oCompany)
        {
            var result = _dbContext.Company.Add(oCompany);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Company> UpdateCompany(Company oCompany)
        {
            var result = _dbContext.Company.Update(oCompany);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }
    }
}
