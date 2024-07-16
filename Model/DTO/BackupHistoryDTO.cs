namespace Model
{
    public class BackupHistoryDTO
    {
        public int Id { get; set; }
        public int BackupJobId { get; set; }
        public string BackupJobName { get; set; }
        public string SourceFilePath { get; set; }
        public string TargetFolderPath { get; set; }
        public string TargetServerIp { get; set; }
        public int TargetBackupId { get; set; }
        public int BackupSchedulerId { get; set; }
        public int BackupStatusId { get; set; }
        public int CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int total_count { get; set; }
    }
}
