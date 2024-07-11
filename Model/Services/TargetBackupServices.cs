using Microsoft.EntityFrameworkCore;

namespace Model.Services
{
    public interface ITargetBackupServices
    {
        public Task<TargetBackup> GetTargetBackupById(int id);
        public Task<TargetBackup> GetTargetBackupByBackupJobId(int backupJobId, int companyId);
        public Task<TargetBackup> AddTargetBackup(TargetBackup oTargetBackup);
        public Task<TargetBackup> UpdateTargetBackup(TargetBackup oTargetBackup);
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
            return await _dbContext.TargetBackup.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<TargetBackup> GetTargetBackupByBackupJobId(int backupJobId, int companyId)
        {
            return await _dbContext.TargetBackup.FirstOrDefaultAsync(x => x.BackupJobId == backupJobId && x.CompanyId == companyId);
        }

        public async Task<TargetBackup> AddTargetBackup(TargetBackup oTargetBackup)
        {
            var result = _dbContext.TargetBackup.Add(oTargetBackup);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }
        public async Task<TargetBackup> UpdateTargetBackup(TargetBackup oTargetBackup)
        {
            var result = _dbContext.TargetBackup.Update(oTargetBackup);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }
    }
}
