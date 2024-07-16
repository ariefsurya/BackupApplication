namespace Model
{
    public class BackupJobDTO
    {
        public int Id { get; set; }
        public string BackupJobName { get; set; }
        public int CompanyId { get; set; }
        public bool IsUseScheduler { get; set; }
        public int StatusId { get; set; }
        public DateTime LastBackupDate { get; set; }
        public int LastBackupStatus { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public TargetBackup oTargetBackup { get; set; }
        public BackupScheduler oBackupScheduler { get; set; }
        public string? SourceServerIp { get; set; }
        public string? SourceFilePath { get; set; }
        public string? TargetFolderPath { get; set; }
        public int total_count { get; set; }

        public BackupJobDTO BackupJobMapToDto(BackupJob oBackupJob)
        {
            return new BackupJobDTO
            {
                Id = oBackupJob.Id,
                BackupJobName = oBackupJob.BackupJobName,
                CompanyId = oBackupJob.CompanyId,
                StatusId = oBackupJob.StatusId,
                LastBackupDate = oBackupJob.LastBackupDate,
                LastBackupStatus = oBackupJob.LastBackupStatus,
                CreatedBy = oBackupJob.CreatedBy,
                CreatedDate = oBackupJob.CreatedDate,
                UpdatedDate = oBackupJob.UpdatedDate,
            };
        }
    }
}
