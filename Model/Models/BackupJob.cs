namespace Model
{
    public class BackupJob
    {
        public int Id { get; set; }
        public string BackupJobName { get; set; }
        public int CompanyId { get; set; }
        public int StatusId { get; set; }
        public DateTime LastBackupDate { get; set; }
        public int LastBackupStatus { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
