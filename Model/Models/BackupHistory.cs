namespace Model
{
    public class BackupHistory
    {
        public int Id { get; set; }
        public string BackupJobId { get; set; }
        public string SourceFilePath { get; set; }
        public string TargetFolderPath { get; set; }
        public string TargetServerName { get; set; }
        public string TargetServerIp { get; set; }
        public int TargetBackupId { get; set; }
        public int CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
