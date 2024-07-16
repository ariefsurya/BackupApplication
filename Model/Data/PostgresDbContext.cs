using Microsoft.EntityFrameworkCore;

namespace Model
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> option) : base(option)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<TargetBackup> TargetBackup { get; set; }
        public DbSet<BackupHistory> BackupHistory { get; set; }
        public DbSet<BackupJob> BackupJob { get; set; }
        public DbSet<BackupScheduler> BackupScheduler { get; set; }
    }
}
