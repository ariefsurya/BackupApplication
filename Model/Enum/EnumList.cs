using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Enum
{
    public enum EnumBackupScheduler
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3
    }
    public enum EnumStatus
    {
        Active = 1,
        Deleted= 2,
        Paused = 3
    }
    public enum EnumBackupStatus
    {
        Started = 1,
        Cancelled = 2,
        Failed = 3,
        Success = 4
    }
}
