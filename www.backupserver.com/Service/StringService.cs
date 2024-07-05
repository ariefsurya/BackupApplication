namespace www.backupserver.com.Service
{
    public interface IStringService
    {
        public string FormatDateTime(DateTime date);
    }

    public class StringService : IStringService
    {
        public string FormatDateTime(DateTime date)
        {
            return date.ToString("d/M/yyyy");
        }
    }
}
