using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace Model.Services
{
    public interface IBackupSchedulerServices
    {
        public Task<BackupScheduler> GetBackupSchedulerByBackupJobId(int backupJobId, int companyId);
        public Task<BackupScheduler> AddBackupScheduler(BackupScheduler oBackupScheduler);
        public Task<BackupScheduler> UpdateBackupScheduler(BackupScheduler oBackupScheduler);
    }

    public class BackupSchedulerServices : IBackupSchedulerServices
    {

        private readonly PostgresDbContext _dbContext;
        public BackupSchedulerServices(PostgresDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BackupScheduler> GetBackupSchedulerByBackupJobId(int backupJobId, int companyId)
        {
            return await _dbContext.BackupScheduler.FirstOrDefaultAsync(x => x.BackupJobId == backupJobId && x.CompanyId == companyId);
        }

        public async Task<BackupScheduler> AddBackupScheduler(BackupScheduler oBackupScheduler)
        {
            var result = _dbContext.BackupScheduler.Add(oBackupScheduler);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }
        public async Task<BackupScheduler> UpdateBackupScheduler(BackupScheduler oBackupScheduler)
        {
            var result = _dbContext.BackupScheduler.Update(oBackupScheduler);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }
    }
}
