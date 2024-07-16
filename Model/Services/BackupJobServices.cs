using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace Model.Services
{
    public interface IBackupJobServices
    {
        public Task<BackupJob> GetBackupJobById(int id, int companyId);
        public Task<List<BackupJob>> GetBackupJobSearch(int companyId, string search, int iPage, int iTake);
        public Task<List<BackupJobDTO>> GetAllBackupJobsAsync(string searchCriteria, int pageIndex, int numRows, string sortBy);
        public Task<BackupJob> AddBackupJob(BackupJob oBackupJob);
        public Task<BackupJob> UpdateBackupJob(BackupJob oBackupJob);
    }

    public class BackupJobServices : IBackupJobServices
    {

        private readonly PostgresDbContext _dbContext;
        private readonly string _connectionString;
        public BackupJobServices(PostgresDbContext dbContext, string connectionString)
        {
            _dbContext = dbContext;
            _connectionString = connectionString;
        }

        public async Task<BackupJob> GetBackupJobById(int id, int companyId)
        {
            return await _dbContext.BackupJob.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == companyId);
        }
        public async Task<List<BackupJob>> GetBackupJobSearch(int companyId, string search, int iPage, int iTake)
        {
            return await _dbContext.BackupJob.Where(x => x.CompanyId == companyId && x.BackupJobName.Contains(search)).Skip((iPage-1) * iTake).Take(iTake).ToListAsync();
        }
        public async Task<List<BackupJobDTO>> GetAllBackupJobsAsync(string searchCriteria, int pageIndex, int numRows, string sortBy)
        {
            var query = @"SELECT * FROM public.get_all_backupjobs(
                    _searchcriteria := @searchCriteria, 
                    _pageindex := @pageIndex, 
                    _numrows := @numRows, 
                    _sortby := @sortBy)";
            var querytest = "SELECT * FROM public.get_all_backupjobs('', 0, 10, 'backupjob.\"Id\"')";


            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@searchCriteria", searchCriteria);
                    command.Parameters.AddWithValue("@pageIndex", pageIndex-1);
                    command.Parameters.AddWithValue("@numRows", numRows);
                    command.Parameters.AddWithValue("@sortBy", sortBy);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var result = new List<BackupJobDTO>();

                        while (await reader.ReadAsync())
                        {
                            var backupJob = new BackupJobDTO
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                BackupJobName = reader.GetString(reader.GetOrdinal("backupjobname")),
                                StatusId = reader.GetInt32(reader.GetOrdinal("statusid")),
                                LastBackupDate = reader.GetDateTime(reader.GetOrdinal("lastbackupdate")),
                                LastBackupStatus = reader.GetInt32(reader.GetOrdinal("lastbackupstatus")),
                                SourceServerIp = reader.GetString(reader.GetOrdinal("sourceserverip")),
                                SourceFilePath = reader.GetString(reader.GetOrdinal("sourcefilepath")),
                                total_count = reader.GetInt32(reader.GetOrdinal("total_count"))
                            };
                            result.Add(backupJob);
                        }

                        return result;
                    }
                }
            }
        }

        public async Task<BackupJob> AddBackupJob(BackupJob oBackupJob)
        {
            var result = _dbContext.BackupJob.Add(oBackupJob);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }
        public async Task<BackupJob> UpdateBackupJob(BackupJob oBackupJob)
        {
            var result = _dbContext.BackupJob.Update(oBackupJob);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }
    }
}
