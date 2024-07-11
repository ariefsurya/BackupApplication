namespace Model
{
    public class TargetBackupDTO
    {
        public int Id { get; set; }
        public int BackupJobId { get; set; }
        public string SourceServerIp { get; set; }
        public string SourceFilePath { get; set; }
        public string TargetFolderPath { get; set; }
        public string TargetFileName { get; set; }
        public string TargetServerIp { get; set; }
        public int CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
