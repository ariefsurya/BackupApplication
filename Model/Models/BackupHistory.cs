namespace Model
{
    public class BackupHistory
    {
        public int Id { get; set; }
        public string SourceFilePath { get; set; }
        public string TargetFolderPath { get; set; }
        public int ServerId { get; set; }
        public string CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
