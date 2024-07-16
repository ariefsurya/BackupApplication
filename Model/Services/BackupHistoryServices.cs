using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Model.Services
{
    public interface IBackupHistoryServices
    {
        public Task<List<BackupHistoryDTO>> GetBackupHistoryByJobId(string searchCriteria, int pageIndex, int numRows, string sortBy);
        public Task<BackupHistory> AddBackupHistory(BackupHistory oBackupHistory);
    }

    public class BackupHistoryServices : IBackupHistoryServices
    {

        private readonly PostgresDbContext _dbContext;
        private readonly string _connectionString;
        public BackupHistoryServices(PostgresDbContext dbContext, string connectionString)
        {
            _dbContext = dbContext;
            _connectionString = connectionString;
        }

        public async Task<List<BackupHistoryDTO>> GetBackupHistoryByJobId(string searchCriteria, int pageIndex, int numRows, string sortBy)
        {
            var query = @"SELECT * FROM public.get_backupjobhistory(
                    _searchcriteria := @searchCriteria, 
                    _pageindex := @pageIndex, 
                    _numrows := @numRows, 
                    _sortby := @sortBy)";
            var querytest = "SELECT * FROM public.get_backupjobhistory('', 0, 10, 'backuphistory.\"Id\"')";


            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@searchCriteria", searchCriteria);
                    command.Parameters.AddWithValue("@pageIndex", pageIndex - 1);
                    command.Parameters.AddWithValue("@numRows", numRows);
                    command.Parameters.AddWithValue("@sortBy", sortBy);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var result = new List<BackupHistoryDTO>();

                        while (await reader.ReadAsync())
                        {
                            var backupJob = new BackupHistoryDTO
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                BackupJobId = reader.GetInt32(reader.GetOrdinal("backupjobid")),
                                BackupJobName = reader.GetString(reader.GetOrdinal("backupjobname")),
                                SourceFilePath = reader.GetString(reader.GetOrdinal("sourcefilepath")),
                                TargetFolderPath = reader.GetString(reader.GetOrdinal("targetfolderpath")),
                                TargetServerIp = reader.GetString(reader.GetOrdinal("targetserverip")),
                                TargetBackupId = reader.GetInt32(reader.GetOrdinal("targetbackupid")),
                                BackupSchedulerId = reader.GetInt32(reader.GetOrdinal("backupschedulerid")),
                                BackupStatusId = reader.GetInt32(reader.GetOrdinal("backupstatusid")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("createddate")),
                                UpdatedDate = reader.GetDateTime(reader.GetOrdinal("updateddate")),
                                total_count = reader.GetInt32(reader.GetOrdinal("total_count"))
                            };
                            result.Add(backupJob);
                        }

                        return result;
                    }
                }
            }
        }

        public async Task<BackupHistory> AddBackupHistory(BackupHistory oBackupHistory)
        {
            var result = _dbContext.BackupHistory.Add(oBackupHistory);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

    }
}
