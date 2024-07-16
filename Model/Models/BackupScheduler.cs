namespace Model
{
    public class BackupScheduler
    {
        public int Id { get; set; }
        public int BackupJobId { get; set; }
        public int CompanyId { get; set; }
        public int BackupSchedulerType { get; set; }
        public string SchedulerDateDaySet { get; set; }
        public TimeOnly SchedulerClockTimeSet { get; set; }
        public DateTime SchedulerStartDate { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    //expected time setter:
    //1. Daily -> Sun,Mon,Tue only -> at 10 PM
    //2. Weekly -> Sun only -> at 11 PM
    //3. Monthly -> Every Date 1, Every Date 15 -> at 01 AM
}
