using Microsoft.EntityFrameworkCore;

namespace Model.Services
{
    public interface ITargetBackupServices
    {
        public Task<TargetBackup> GetTargetBackupById(int id);
        public Task<TargetBackup> AddTargetBackup(TargetBackup oTargetBackup);
    }

    public class TargetBackupServices : ITargetBackupServices
    {

        private readonly PostgresDbContext _dbContext;
        public TargetBackupServices(PostgresDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TargetBackup> GetTargetBackupById(int id)
        {
            return await _dbContext.TargetBackup.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<TargetBackup> AddTargetBackup(TargetBackup oTargetBackup)
        {
            var result = _dbContext.TargetBackup.Add(oTargetBackup);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }
    }
}
