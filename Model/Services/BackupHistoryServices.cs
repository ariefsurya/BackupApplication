using Microsoft.EntityFrameworkCore;

namespace Model.Services
{
    public interface IBackupHistoryServices
    {
        public Task<List<BackupHistory>> GeBackupHistory();
        public Task<BackupHistory> AddBackupHistory(BackupHistory oBackupHistory);
    }

    public class BackupHistoryServices : IBackupHistoryServices
    {

        private readonly PostgresDbContext _dbContext;
        public BackupHistoryServices(PostgresDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<BackupHistory>> GeBackupHistory()
        {
            return await _dbContext.BackupHistory.ToListAsync();
        }

        public async Task<BackupHistory> AddBackupHistory(BackupHistory oBackupHistory)
        {
            var result = _dbContext.BackupHistory.Add(oBackupHistory);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

    }
}
