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
            if (date == DateTime.MinValue)
                return "-";
            return date.ToString("d/M/yyyy");
        }
    }
}
