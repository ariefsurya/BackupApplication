namespace Model
{
    public class TargetBackup
    {
        public int Id { get; set; }
        public string SourceFilePath { get; set; }
        public string TargetFolderPath { get; set; }
        public string ServerName { get; set; }
        public string ServerIp { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
